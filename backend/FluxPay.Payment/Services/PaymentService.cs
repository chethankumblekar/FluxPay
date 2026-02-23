using System.Text.Json;
using FluxPay.Payment.Domain;
using FluxPay.Payment.Domain.Exceptions;
using FluxPay.Payment.Infrastructure;
using FluxPay.Payment.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxPay.Payment.Services;

public class PaymentService(PaymentDbContext db, IIdempotencyService idempotencyService, IFraudService fraudService) : IPaymentService
{
    public async Task<Domain.Payment> CreateAsync(
        string tenantId,
        string idempotencyKey,
        CreatePaymentRequest request)
    {
        var redisKey = $"payment:{tenantId}:{idempotencyKey}";

        var cached = await idempotencyService.GetAsync(redisKey);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            var cachedPayment =
                JsonSerializer.Deserialize<Domain.Payment>(cached);

            if (cachedPayment != null)
                return cachedPayment;
        }

        var existing = await db.Payments
            .FirstOrDefaultAsync(p =>
                p.TenantId == tenantId &&
                p.IdempotencyKey == idempotencyKey);

        if (existing != null)
        {
            await idempotencyService.SetAsync(
                redisKey,
                JsonSerializer.Serialize(existing));

            return existing;
        }

        var riskScore = await fraudService.EvaluateRiskAsync(request); 

        await using var transaction =
            await db.Database.BeginTransactionAsync();

        try
        {
            var payment = new Domain.Payment
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Amount = request.Amount,
                Currency = request.Currency,
                IdempotencyKey = idempotencyKey,
                Status = PaymentStatus.Created,
                RiskScore = riskScore
            };

            db.Payments.Add(payment);

            var merchantCredit = new LedgerEntry
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                TenantId = tenantId,
                AccountType = "MERCHANT",
                EntryType = "CREDIT",
                Amount = request.Amount
            };

            var platformDebit = new LedgerEntry
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                TenantId = tenantId,
                AccountType = "PLATFORM",
                EntryType = "DEBIT",
                Amount = request.Amount
            };

            db.LedgerEntries.AddRange(
                merchantCredit,
                platformDebit);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            await idempotencyService.SetAsync(
                redisKey,
                JsonSerializer.Serialize(payment));

            return payment;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();

            // Handle race condition:
            var fallback = await db.Payments
                .FirstOrDefaultAsync(p =>
                    p.TenantId == tenantId &&
                    p.IdempotencyKey == idempotencyKey);

            if (fallback != null)
            {
                await idempotencyService.SetAsync(
                    redisKey,
                    JsonSerializer.Serialize(fallback));

                return fallback;
            }

            throw;
        }
    }

    public async Task<Domain.Payment> CaptureAsync(string tenantId, Guid paymentId)
    {
        var payment = await db.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.TenantId == tenantId);

        if (payment == null)
            throw new NotFoundException("Payment not found");

        if (!PaymentStateMachine.CanCapture(payment.Status))
            throw new InvalidStateException("Invalid state transition");

        payment.Status = PaymentStatus.Captured;

        await db.SaveChangesAsync();

        return payment;
    }

    public async Task<Domain.Payment> RefundAsync(string tenantId, Guid paymentId)
    {
        var payment = await db.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.TenantId == tenantId);

        if (payment == null)
            throw new NotFoundException("Payment not found");

        if (!PaymentStateMachine.CanRefund(payment.Status))
            throw new InvalidStateException("Invalid state transition");

        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            payment.Status = PaymentStatus.Refunded;

            var refundLedger = new LedgerEntry
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                TenantId = tenantId,
                AccountType = "MERCHANT",
                EntryType = "DEBIT",
                Amount = payment.Amount
            };

            db.LedgerEntries.Add(refundLedger);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return payment;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}