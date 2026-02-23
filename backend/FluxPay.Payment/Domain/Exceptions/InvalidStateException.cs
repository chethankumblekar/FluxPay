namespace FluxPay.Payment.Domain.Exceptions;

public class InvalidStateException(string message) : DomainException(message, "invalid_state_transition");