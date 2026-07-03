using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.Publishers;
using br.com.fiap.cloudgames.Payment.Infrastructure.Config;
using br.com.fiap.cloudgames.Payment.Infrastructure.Messagging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Messaging.Publishers
{
    public class PaymentFailedEventPublisher : RabbitMqMessagePublisher, IPaymentFailedEventPublisher
    {
        public PaymentFailedEventPublisher(RabbitMqConnection connection, IOptions<RabbitMqSettings> options) 
            : base(connection, options.Value.UserCreatedEvent.Exchange, options.Value.UserCreatedEvent.RoutingKey)
        { }

        public async Task PublishAsync(PaymentFailedEvent message)
        {
            await base.PublishAsync<PaymentFailedEvent>(message);
        }
    }
}
