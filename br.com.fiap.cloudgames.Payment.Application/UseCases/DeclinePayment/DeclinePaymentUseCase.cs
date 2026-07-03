using br.com.fiap.cloudgames.Payment.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Payment.Domain.Repositories;

namespace br.com.fiap.cloudgames.Payment.Application.UseCases.DeclinePayment;

public class DeclinePaymentUseCase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeclinePaymentUseCase(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ExecuteAsync(Guid orderId, Guid userId)
    {
      await _unitOfWork.BeginTransactionAsync();
      var payment = await _paymentRepository.GetByOrderIdAndUserIdAsync(orderId, userId);
      if (payment == null)
          throw new ApplicationException("Payment not found");

      try
      {
          payment.PaymentDeclined();
          await _paymentRepository.UpdateAsync(payment);
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