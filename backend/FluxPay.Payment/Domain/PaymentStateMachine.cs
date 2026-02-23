namespace FluxPay.Payment.Domain;

public static class PaymentStateMachine
{
    public static bool CanCapture(string status)
        => status == PaymentStatus.Created;

    public static bool CanRefund(string status)
        => status == PaymentStatus.Captured;
}