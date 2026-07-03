namespace br.com.fiap.cloudgames.Payment.Application.Events;

public class PaymentProcessedEvent
{
    public Guid EventId { get; init; }
    public Guid OrderId { get; set; }
}