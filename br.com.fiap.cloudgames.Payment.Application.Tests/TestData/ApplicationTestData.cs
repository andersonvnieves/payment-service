using br.com.fiap.cloudgames.Payment.Application.Events;

namespace br.com.fiap.cloudgames.Payment.Application.Tests.TestData;

public static class ApplicationTestData
{
    public static OrderCreatedEvent ValidOrderCreatedEvent(Guid? orderId = null, Guid? userId = null, decimal totalAmount = 150.50m)
    {
        return new OrderCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OrderId = orderId ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            TotalAmount = totalAmount,
            Name = "John Doe",
            Email = "john.doe@example.com"
        };
    }
}
