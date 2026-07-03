using br.com.fiap.cloudgames.Payment.Domain.Aggregates;

namespace br.com.fiap.cloudgames.Payment.Domain.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Aggregates.Payment payment);
    Task UpdateAsync(Aggregates.Payment payment);
    Task<Aggregates.Payment> GetByOrderIdAndUserIdAsync(Guid orderId, Guid userId);
}