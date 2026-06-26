namespace br.com.fiap.cloudgames.Payment.Application.UnitsOfWork;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();
}