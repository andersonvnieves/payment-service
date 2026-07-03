using br.com.fiap.cloudgames.Payment.Domain.Aggregates;
using br.com.fiap.cloudgames.Payment.Domain.Repositories;
using br.com.fiap.cloudgames.Payment.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Domain.Aggregates.Payment>  _payments;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
        _payments = _context.Set<Domain.Aggregates.Payment>();
    }


    public async Task AddAsync(Domain.Aggregates.Payment payment)
    {
        await _payments.AddAsync(payment);
    }

    public Task UpdateAsync(Domain.Aggregates.Payment payment)
    {
        _payments.Update(payment);
        return Task.CompletedTask;
    }

    public async Task<Domain.Aggregates.Payment> GetByOrderIdAndUserIdAsync(Guid orderId, Guid userId)
    {
        return await _payments.FirstOrDefaultAsync(p => p.OrderId == orderId && p.UserId == userId);
    }
}