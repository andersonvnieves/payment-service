using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Payment.Domain.Repositories;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace br.com.fiap.cloudgames.Payment.Application.Handlers;

public class OrderCreatedEventHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public OrderCreatedEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        using var scope = serviceScopeFactory.CreateScope();

        _paymentRepository =
            scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

        _unitOfWork =
            scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
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
        catch (Exception e)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        }
}