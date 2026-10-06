using br.com.fiap.cloudgames.Payment.Domain.Aggregates;
using br.com.fiap.cloudgames.Payment.Domain.Enums;
using br.com.fiap.cloudgames.Payment.Domain.Exceptions;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;
using Reqnroll;
using System;
using Xunit;

namespace br.com.fiap.cloudgames.Payment.Specs.StepsDefinition;

[Binding]
public class PaymentProcessingSteps
{
    private Guid _orderId;
    private Guid _userId;
    private decimal _amount;
    private Domain.Aggregates.Payment? _payment;
    private Exception? _thrownException;

    [Given("an order {string} for user {string} with amount {decimal}")]
    public void GivenAnOrderForUserWithAmount(string orderIdStr, string userIdStr, decimal amount)
    {
        _orderId = Guid.Parse(orderIdStr);
        _userId = Guid.Parse(userIdStr);
        _amount = amount;
    }

    [Given("a pending payment exists for order {string}")]
    public void GivenAPendingPaymentExistsForOrder(string orderIdStr)
    {
        _orderId = Guid.Parse(orderIdStr);
        _userId = Guid.NewGuid();
        _amount = 100.00m;
        _payment = Domain.Aggregates.Payment.Create(_orderId, _userId, new Price(_amount));
    }

    [Given("an approved payment exists for order {string}")]
    public void GivenAnApprovedPaymentExistsForOrder(string orderIdStr)
    {
        _orderId = Guid.Parse(orderIdStr);
        _userId = Guid.NewGuid();
        _amount = 100.00m;
        _payment = Domain.Aggregates.Payment.Create(_orderId, _userId, new Price(_amount));
        _payment.PaymentApproved();
    }

    [When("the payment is created")]
    public void WhenThePaymentIsCreated()
    {
        _payment = Domain.Aggregates.Payment.Create(_orderId, _userId, new Price(_amount));
    }

    [When("the payment is approved")]
    public void WhenThePaymentIsApproved()
    {
        _payment!.PaymentApproved();
    }

    [When("the payment is declined")]
    public void WhenThePaymentIsDeclined()
    {
        _payment!.PaymentDeclined();
    }

    [When("the payment is approved again")]
    public void WhenThePaymentIsApprovedAgain()
    {
        try
        {
            _payment!.PaymentApproved();
        }
        catch (Exception ex)
        {
            _thrownException = ex;
        }
    }

    [Then("the payment status should be pending")]
    public void ThenThePaymentStatusShouldBePending()
    {
        Assert.NotNull(_payment);
        Assert.Equal(PaymentStatus.Pending, _payment.Status);
    }

    [Then("the payment status should be approved")]
    public void ThenThePaymentStatusShouldBeApproved()
    {
        Assert.NotNull(_payment);
        Assert.Equal(PaymentStatus.Approved, _payment.Status);
    }

    [Then("the payment status should be rejected")]
    public void ThenThePaymentStatusShouldBeRejected()
    {
        Assert.NotNull(_payment);
        Assert.Equal(PaymentStatus.Rejected, _payment.Status);
    }

    [Then("the payment amount should be {decimal}")]
    public void ThenThePaymentAmountShouldBe(decimal expectedAmount)
    {
        Assert.NotNull(_payment);
        Assert.Equal(expectedAmount, _payment.Amount.PriceValue);
    }

    [Then("a domain exception should be thrown with message {string}")]
    public void ThenADomainExceptionShouldBeThrownWithMessage(string expectedMessage)
    {
        Assert.NotNull(_thrownException);
        var domainEx = Assert.IsType<DomainException>(_thrownException);
        Assert.Contains(expectedMessage, domainEx.Errors);
    }
}
