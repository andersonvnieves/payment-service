using br.com.fiap.cloudgames.Payment.Domain.Enums;

namespace br.com.fiap.cloudgames.Payment.Application.Events;

public class PaymentProcessedEvent
{
    public Guid EventId { get; init; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
}