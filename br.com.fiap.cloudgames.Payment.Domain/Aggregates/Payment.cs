using br.com.fiap.cloudgames.Payment.Domain.Enums;
using br.com.fiap.cloudgames.Payment.Domain.Exceptions;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Payment.Domain.Aggregates;

public class Payment
{
    public Payment() { } //ORM

    #region  FactoryMethod
    public static Payment Create(Guid orderId, Guid userId, Price amount)
    {
        return new Payment()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId, 
            UserId = userId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }
    #endregion
    
    #region  Properties
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Price Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    #endregion

    public void PaymentApproved()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Payment already processed.");
            
        Status = PaymentStatus.Approved;
    }
    
    public void PaymentDeclined()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Payment already processed.");
            
        Status = PaymentStatus.Rejected;
    }
}