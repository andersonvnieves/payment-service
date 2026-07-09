using br.com.fiap.cloudgames.Payment.Domain.Exceptions;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Payment.Domain.Tests.ValueObjects;

public class PriceTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(99.9)]
    [InlineData(199.99)]
    public void ShouldCreatePrice_WhenValidPriceProvided(decimal priceValue)
    {
        var price = new Price(priceValue);

        Assert.Equal(priceValue, price.PriceValue);
    }

    [Fact]
    public void WhenPriceIsNegative_ShouldThrowDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Price(-0.01m));

        Assert.Contains("Price can't be negative.", ex.Errors);
    }

    [Theory]
    [InlineData(10.123)]
    [InlineData(0.001)]
    [InlineData(99.999)]
    public void WhenPriceHasMoreThanTwoDecimals_ShouldThrowDomainException(decimal priceValue)
    {
        var ex = Assert.Throws<DomainException>(() => new Price(priceValue));

        Assert.Contains("Price cannot have more than two decimal places.", ex.Errors);
    }
}
