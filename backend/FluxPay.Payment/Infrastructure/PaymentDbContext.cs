using FluxPay.Payment.Domain;
using Microsoft.EntityFrameworkCore;

namespace FluxPay.Payment.Infrastructure;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options) { }

    public DbSet<Domain.Payment> Payments => Set<Domain.Payment>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Payment>()
            .HasIndex(p => new { p.TenantId, p.IdempotencyKey })
            .IsUnique();

        modelBuilder.Entity<LedgerEntry>()
            .HasIndex(l => l.PaymentId);
        
        modelBuilder.Entity<ApiKey>()
            .HasIndex(a => a.Key)
            .IsUnique();
    }
}