using br.com.fiap.cloudgames.Payment.Domain.Exceptions;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Payment.Domain.Tests.ValueObjects;

public class AmountTests
{
    [Fact]
    public void ShouldCreateAmount_WhenValidDataProvided()
    {
        var amount = new Amount(100.50m, "usd");

        Assert.Equal(100.50m, amount.Value);
        Assert.Equal("USD", amount.Currency);
    }

    [Theory]
    [InlineData("usd", "USD")]
    [InlineData("  brl  ", "BRL")]
    [InlineData("Eur", "EUR")]
    public void ShouldNormalizeCurrencyToUppercaseAndTrimmed(string currency, string expectedNormalized)
    {
        var amount = new Amount(50m, currency);

        Assert.Equal(expectedNormalized, amount.Currency);
    }

    [Fact]
    public void WhenValueIsNegative_ShouldThrowDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Amount(-0.01m, "USD"));

        Assert.Contains("Amount cannot be negative.", ex.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WhenCurrencyIsBlank_ShouldThrowDomainException(string? currency)
    {
        var ex = Assert.Throws<DomainException>(() => new Amount(100m, currency!));

        Assert.Contains("Currency is required.", ex.Errors);
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDT")]
    [InlineData("1")]
    public void WhenCurrencyIsNotThreeCharacters_ShouldThrowDomainException(string currency)
    {
        var ex = Assert.Throws<DomainException>(() => new Amount(100m, currency));

        Assert.Contains("Currency must be a valid ISO 4217 code.", ex.Errors);
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var amount = new Amount(1234.567m, "BRL");
        var expectedValue = 1234.57m.ToString("F2");

        Assert.Equal($"{expectedValue} BRL", amount.ToString());
    }
}
