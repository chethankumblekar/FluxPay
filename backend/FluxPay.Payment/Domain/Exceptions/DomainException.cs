namespace FluxPay.Payment.Domain.Exceptions;

public class DomainException(string message, string errorCode) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
}