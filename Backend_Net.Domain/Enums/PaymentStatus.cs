namespace Backend_Net.Domain.Enums;

public enum PaymentStatus
{
    Created = 1,
    Pending = 2,
    Processing = 3,
    Success = 4,
    Failed = 5,
    Canceled = 6,
    Refunded = 7
}