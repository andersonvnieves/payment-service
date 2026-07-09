using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.Publishers;
using br.com.fiap.cloudgames.Payment.Infrastructure.Config;
using br.com.fiap.cloudgames.Payment.Infrastructure.Messagging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Messaging.Publishers;

public class PaymentProcessedEventPublisher: RabbitMqMessagePublisher, IPaymentProcessedEventPublisher
{
    public PaymentProcessedEventPublisher(RabbitMqConnection connection, IOptions<RabbitMqSettings> options) 
        : base(connection, options.Value.PaymentProcessedEvent.Exchange, options.Value.PaymentProcessedEvent.RoutingKey)
    { }

    public async Task PublishAsync(PaymentProcessedEvent message)
    {
        await base.PublishAsync<PaymentProcessedEvent>(message);
    }
}