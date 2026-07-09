using br.com.fiap.cloudgames.Payment.Domain.Aggregates;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Payment.Domain.Tests.TestData;

public static class DomainTestData
{
    public static Amount ValidAmount() => new(100.00m, "USD");
    
    public static Price ValidPrice() => new(150.50m);

    public static Guid ValidOrderId() => Guid.NewGuid();

    public static Guid ValidUserId() => Guid.NewGuid();

    public static br.com.fiap.cloudgames.Payment.Domain.Aggregates.Payment ValidPayment() =>
        br.com.fiap.cloudgames.Payment.Domain.Aggregates.Payment.Create(ValidOrderId(), ValidUserId(), ValidPrice());
}
