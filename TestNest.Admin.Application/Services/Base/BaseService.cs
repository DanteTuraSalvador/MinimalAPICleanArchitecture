using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.Application.Services.Base;

public class BaseService(IUnitOfWork unitOfWork, ILogger logger, IDatabaseExceptionHandlerFactory exceptionHandlerFactory)
{
    protected readonly IUnitOfWork _UnitOfWork = unitOfWork;
    protected readonly ILogger _logger = logger;
    private readonly IDatabaseExceptionHandlerFactory _exceptionHandlerFactory = exceptionHandlerFactory;

    protected async Task<Result> SafeCommitAsync()
    {
        try
        {
            await _UnitOfWork.CommitTransactionAsync();
            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            return _exceptionHandlerFactory.HandleDbUpdateException(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "General error during commit.");
            return Result.Failure(ErrorType.Internal, new Error("ServiceError", "An unexpected error occurred."));
        }
    }

    protected async Task<Result<T>> SafeCommitAsync<T>(Func<Result<T>> successResultFunc, string errorMessage = "Error saving changes.")
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            _ = await _UnitOfWork.SaveChangesAsync();
            scope.Complete();
            return successResultFunc();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error during SaveChangesAsync.");
            Result dbErrorResult = _exceptionHandlerFactory.HandleDbUpdateException(ex);
            return Result<T>.Failure(dbErrorResult.ErrorType, dbErrorResult.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, errorMessage);
            return Result<T>.Failure(ErrorType.Internal, new Error("ServiceError", ex.Message));
        }
    }

    protected async Task<Result> SafeTransactionAsync(Func<Task<Result>> operation)
    {
        await _UnitOfWork.BeginTransactionAsync();
        try
        {
            Result result = await operation();
            if (!result.IsSuccess)
            {
                await _UnitOfWork.RollbackTransactionAsync();
                return result;
            }

            Result commitResult = await SafeCommitAsync();
            if (!commitResult.IsSuccess)
            {
                await _UnitOfWork.RollbackTransactionAsync();
                return commitResult;
            }

            return result;
        }
        catch (Exception ex)
        {
            await _UnitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Transaction failed");
            return Result.Failure(ErrorType.Internal, new Error("TransactionError", ex.Message));
        }
    }

    protected async Task<Result<T>> SafeTransactionAsync<T>(Func<Task<Result<T>>> operation)
    {
        await _UnitOfWork.BeginTransactionAsync();
        try
        {
            Result<T> result = await operation();
            if (!result.IsSuccess)
            {
                await _UnitOfWork.RollbackTransactionAsync();
                return result;
            }

            Result commitResult = await SafeCommitAsync();
            if (!commitResult.IsSuccess)
            {
                await _UnitOfWork.RollbackTransactionAsync();
                return Result<T>.Failure(commitResult.ErrorType, commitResult.Errors);
            }

            return result;
        }
        catch (Exception ex)
        {
            await _UnitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Transaction failed");
            return Result<T>.Failure(ErrorType.Internal, new Error("TransactionError", ex.Message));
        }
    }
}
