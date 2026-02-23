namespace FluxPay.Payment.Domain.Exceptions;

public class NotFoundException(string message) : DomainException(message, "not_found");