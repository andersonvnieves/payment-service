using br.com.fiap.cloudgames.Payment.Domain.Aggregates;
using br.com.fiap.cloudgames.Payment.Domain.Enums;
using br.com.fiap.cloudgames.Payment.Domain.Exceptions;
using br.com.fiap.cloudgames.Payment.Domain.Tests.TestData;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Payment.Domain.Tests.Aggregates;

public class PaymentTests
{
    [Fact]
    public void Create_ShouldInstantiatePaymentWithPendingStatusAndCorrectData()
    {
        var orderId = DomainTestData.ValidOrderId();
        var userId = DomainTestData.ValidUserId();
        var amount = DomainTestData.ValidPrice();

        var payment = br.com.fiap.cloudgames.Payment.Domain.Aggregates.Payment.Create(orderId, userId, amount);

        Assert.NotEqual(Guid.Empty, payment.Id);
        Assert.Equal(orderId, payment.OrderId);
        Assert.Equal(userId, payment.UserId);
        Assert.Equal(amount, payment.Amount);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.True(payment.CreatedAt <= DateTime.Now);
        Assert.True(payment.UpdatedAt <= DateTime.Now);
    }

    [Fact]
    public void PaymentApproved_WhenStatusIsPending_ShouldChangeStatusToApproved()
    {
        var payment = DomainTestData.ValidPayment();

        payment.PaymentApproved();

        Assert.Equal(PaymentStatus.Approved, payment.Status);
    }

    [Theory]
    [InlineData(PaymentStatus.Approved)]
    [InlineData(PaymentStatus.Rejected)]
    public void PaymentApproved_WhenStatusIsNotPending_ShouldThrowDomainException(PaymentStatus nonPendingStatus)
    {
        var payment = DomainTestData.ValidPayment();
        if (nonPendingStatus == PaymentStatus.Approved)
            payment.PaymentApproved();
        else
            payment.PaymentDeclined();

        var ex = Assert.Throws<DomainException>(() => payment.PaymentApproved());
        Assert.Contains("Payment already processed.", ex.Errors);
    }

    [Fact]
    public void PaymentDeclined_WhenStatusIsPending_ShouldChangeStatusToRejected()
    {
        var payment = DomainTestData.ValidPayment();

        payment.PaymentDeclined();

        Assert.Equal(PaymentStatus.Rejected, payment.Status);
    }

    [Theory]
    [InlineData(PaymentStatus.Approved)]
    [InlineData(PaymentStatus.Rejected)]
    public void PaymentDeclined_WhenStatusIsNotPending_ShouldThrowDomainException(PaymentStatus nonPendingStatus)
    {
        var payment = DomainTestData.ValidPayment();
        if (nonPendingStatus == PaymentStatus.Approved)
            payment.PaymentApproved();
        else
            payment.PaymentDeclined();

        var ex = Assert.Throws<DomainException>(() => payment.PaymentDeclined());
        Assert.Contains("Payment already processed.", ex.Errors);
    }
}
