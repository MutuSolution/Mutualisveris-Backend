using Domain.Enums;

namespace Common.Responses.Payments;

public record PaymentResponse
{
    public int Id { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod Method { get; init; }
    public DateTime PaymentDate { get; init; }
    public PaymentStatus Status { get; init; }
}
