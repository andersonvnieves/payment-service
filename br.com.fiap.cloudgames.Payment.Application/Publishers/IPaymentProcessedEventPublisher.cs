using br.com.fiap.cloudgames.Payment.Application.Events;

namespace br.com.fiap.cloudgames.Payment.Application.Publishers;

public interface IPaymentProcessedEventPublisher
{
    Task PublishAsync(PaymentProcessedEvent message);
}