using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Payment.Domain.Repositories;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Payment.Application.Handlers;

public class OrderCreatedEventHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public OrderCreatedEventHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task HandleAsync(OrderCreatedEvent orderCreatedEvent)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var amount = new Price(orderCreatedEvent.TotalAmount);
            var payment = Payment.Domain.Aggregates.Payment.Create(orderCreatedEvent.OrderId, orderCreatedEvent.UserId, amount);
            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.CommitAsync();
            return;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        }
}
