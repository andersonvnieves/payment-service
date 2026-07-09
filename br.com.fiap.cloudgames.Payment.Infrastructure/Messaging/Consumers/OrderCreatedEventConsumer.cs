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
    
    public OrderCreatedEventConsumer(RabbitMqConnection rabbitConnection, OrderCreatedEventHandler handler, ILogger<OrderCreatedEvent> logger, IOptions<RabbitMqSettings> options) 
        : base(rabbitConnection, logger, options.Value.OrderCreatedEvent.Exchange, options.Value.OrderCreatedEvent.RoutingKey)
    {
        _handler = handler;
    }

    public async Task ConsumeAsync()
    {
        await base.ConsumeAsync();
    }

    protected override async Task HandleMessageAsync(OrderCreatedEvent message)
    {
        await _handler.HandleAsync(message);
    }
}