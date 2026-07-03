using br.com.fiap.cloudgames.Payment.Application.Consumers;
using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.Handlers;
using br.com.fiap.cloudgames.Payment.Infrastructure.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Messagging.Consumers;

public class OrderCreatedEventConsumer : RabbitMqMessageConsumer<OrderCreatedEvent>, IOrderCreatedEventConsumer
{
    private readonly OrderCreatedEventHandler _handler;
    private readonly IOptions<RabbitMqSettings> _options;
    
    public OrderCreatedEventConsumer(RabbitMqConnection rabbitConnection, ILogger logger, string exchange, string routingKey) 
        : base(rabbitConnection, logger, exchange, routingKey)
    {
    }

    protected override async Task HandleMessageAsync(OrderCreatedEvent message)
    {
        await _handler.HandleAsync(message);
    }
}