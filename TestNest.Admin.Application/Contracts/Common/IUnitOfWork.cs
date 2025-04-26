namespace TestNest.Admin.Application.Contracts.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}
