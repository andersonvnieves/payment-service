using br.com.fiap.cloudgames.Payment.Application.Abstractions;
using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.Publishers;
using br.com.fiap.cloudgames.Payment.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Payment.Domain.Repositories;

namespace br.com.fiap.cloudgames.Payment.Application.UseCases.ApprovePayment;

public class ApprovePaymentUseCase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentProcessedEventPublisher _paymentProcessedEventPublisher;
    private readonly ICurrentUser _currentUser;

    public ApprovePaymentUseCase(IPaymentRepository paymentRepository, 
        IUnitOfWork unitOfWork,
        IPaymentProcessedEventPublisher paymentProcessedEventPublisher,
        ICurrentUser currentUser)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _paymentProcessedEventPublisher = paymentProcessedEventPublisher;
        _currentUser = currentUser;
    }
    
    public async Task ExecuteAsync(Guid orderId)
    {
        await _unitOfWork.BeginTransactionAsync();
        var payment = await _paymentRepository.GetByOrderIdAndUserIdAsync(orderId, _currentUser.UserId);
        if (payment == null)
            throw new ApplicationException("Payment not found");

        try
        {
            payment.PaymentApproved();
            await _paymentRepository.UpdateAsync(payment);
            await _unitOfWork.CommitAsync();
            await _paymentProcessedEventPublisher.PublishAsync(new PaymentProcessedEvent()
            {
                EventId = Guid.NewGuid(),
                UserId = _currentUser.UserId,
                OrderId = payment.OrderId,
                PaymentStatus = payment.Status,
                Name = _currentUser.Name,
                Email = _currentUser.Email
            });            
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        
    }
}