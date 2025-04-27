using System.Transactions;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.Application.Mappings;
using TestNest.Admin.Application.Services.Base;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.Application.Services;

public class EstablishmentService(
    IEstablishmentRepository establishmentRepository,
    IUnitOfWork unitOfWork,
    IDatabaseExceptionHandlerFactory exceptionHandlerFactory,
    ILogger<EstablishmentService> logger) : BaseService(unitOfWork, logger, exceptionHandlerFactory), IEstablishmentService
{
    private readonly IEstablishmentRepository _establishmentRepository = establishmentRepository;

    public async Task<Result<EstablishmentResponse>> CreateEstablishmentAsync(
        EstablishmentForCreationRequest establishmentForCreationRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentName> establishmentNameResult = EstablishmentName
            .Create(establishmentForCreationRequest.EstablishmentName);
        Result<EmailAddress> establishmentEmailResult = EmailAddress
            .Create(establishmentForCreationRequest.EstablishmentEmail);

        var combinedValidationResult = Result.Combine(
            establishmentNameResult.ToResult(),
            establishmentEmailResult.ToResult());

        if (!combinedValidationResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                ErrorType.Validation,
                [.. combinedValidationResult.Errors]);
        }

        Result<bool> uniquenessCheckResult = await EstablishmentCombinationExistsAsync(
            establishmentNameResult.Value!,
            establishmentEmailResult.Value!);

        if (!uniquenessCheckResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                ErrorType.Conflict,
                [.. uniquenessCheckResult.Errors]);
        }

        Result<Establishment> establishmentResult = Establishment
            .Create(establishmentNameResult.Value!,
                establishmentEmailResult.Value!);

        if (!establishmentResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                ErrorType.Validation,
                [.. establishmentResult.Errors]);
        }

        Establishment establishment = establishmentResult.Value!;
        _ = await _establishmentRepository.AddAsync(establishment);

        Result<Establishment> commitResult = await SafeCommitAsync(
            () => Result<Establishment>.Success(establishment));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EstablishmentResponse>.Success(
                establishment.ToEstablishmentResponse());
        }
        return Result<EstablishmentResponse>.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    public async Task<Result<EstablishmentResponse>> UpdateEstablishmentAsync(
        EstablishmentId establishmentId,
        EstablishmentForUpdateRequest establishmentForUpdateRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<Establishment> validatedEstablishment = await _establishmentRepository
            .GetByIdAsync(establishmentId);
        if (!validatedEstablishment.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                 validatedEstablishment.ErrorType,
                 validatedEstablishment.Errors);
        }

        Establishment establishment = validatedEstablishment.Value!;
        await _establishmentRepository.DetachAsync(establishment);

        Result<EstablishmentName> establishmentName = EstablishmentName
            .Create(establishmentForUpdateRequest.EstablishmentName);
        Result<EmailAddress> establishmentEmail = EmailAddress
            .Create(establishmentForUpdateRequest.EstablishmentEmail);
        Result<EstablishmentStatus> establishmentStatusResult = EstablishmentStatus
            .FromId(establishmentForUpdateRequest.EstablishmentStatusId);

        if (!establishmentStatusResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                establishmentStatusResult.ErrorType,
                establishmentStatusResult.Errors);
        }

        var combinedValidationResult = Result.Combine(
            establishmentName.ToResult(),
            establishmentEmail.ToResult(),
            establishmentStatusResult.ToResult());

        if (!combinedValidationResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                ErrorType.Validation,
                [.. combinedValidationResult.Errors]);
        }

        Result<Establishment> updatedEstablishmentResult = establishment
            .WithName(establishmentName.Value!)
            .Bind(e => e.WithEmail(establishmentEmail.Value!))
            .Bind(e => e.WithStatus(establishmentStatusResult.Value!));

        if (!updatedEstablishmentResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                ErrorType.Validation,
                [.. updatedEstablishmentResult.Errors]);
        }

        Result<bool> uniquenessCheckResult = await EstablishmentCombinationExistsAsync(
            establishmentName.Value!,
            establishmentEmail.Value!,
            establishmentId);

        if (!uniquenessCheckResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                ErrorType.Conflict,
                [.. uniquenessCheckResult.Errors]);
        }

        Result<Establishment> updateResult = await _establishmentRepository
            .UpdateAsync(updatedEstablishmentResult.Value!);
        if (!updateResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                updateResult.ErrorType,
                updateResult.Errors);
        }

        Result<Establishment> commitResult = await SafeCommitAsync(
            () => Result<Establishment>.Success(updatedEstablishmentResult.Value!));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EstablishmentResponse>.Success(
                commitResult.Value!.ToEstablishmentResponse());
        }
        return Result<EstablishmentResponse>.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    public async Task<Result<EstablishmentResponse>> PatchEstablishmentAsync(
        EstablishmentId establishmentId,
        EstablishmentPatchRequest establishmentPatchRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<Establishment> validatedEstablishment = await _establishmentRepository
            .GetByIdAsync(establishmentId);
        if (!validatedEstablishment.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                 validatedEstablishment.ErrorType,
                 validatedEstablishment.Errors);
        }

        Establishment establishment = validatedEstablishment.Value!;
        await _establishmentRepository.DetachAsync(establishment);

        if (establishmentPatchRequest.EstablishmentName != null)
        {
            Result<EstablishmentName> establishmentNameResult = EstablishmentName
                .Create(establishmentPatchRequest.EstablishmentName);
            if (!establishmentNameResult.IsSuccess)
            {
                return Result<EstablishmentResponse>.Failure(
                    establishmentNameResult.ErrorType,
                    establishmentNameResult.Errors);
            }

            establishment = establishment
                .WithName(establishmentNameResult.Value!).Value!;
        }

        if (establishmentPatchRequest.EmailAddress != null)
        {
            Result<EmailAddress> emailResult = EmailAddress
                .Create(establishmentPatchRequest.EmailAddress);
            if (!emailResult.IsSuccess)
            {
                return Result<EstablishmentResponse>.Failure(
                    emailResult.ErrorType,
                    emailResult.Errors);
            }

            establishment = establishment
                .WithEmail(emailResult.Value!).Value!;
        }

        if (establishmentPatchRequest.EstablishmentStatus != null)
        {
            EstablishmentStatus currentStatus = establishment.EstablishmentStatus;
            Result<EstablishmentStatus> newStatusResult = EstablishmentStatus
                .FromId(establishmentPatchRequest.EstablishmentStatus.Value);
            if (!newStatusResult.IsSuccess)
            {
                return Result<EstablishmentResponse>.Failure(
                    newStatusResult.ErrorType,
                    newStatusResult.Errors);
            }

            EstablishmentStatus? newStatus = newStatusResult.Value;
            Result transitionResult = EstablishmentStatus
                .ValidateTransition(currentStatus, newStatus!);
            if (!transitionResult.IsSuccess)
            {
                return Result<EstablishmentResponse>.Failure(
                    transitionResult.ErrorType,
                    transitionResult.Errors);
            }

            establishment = establishment.WithStatus(newStatus!).Value!;
        }

        if (establishmentPatchRequest.EstablishmentName != null || establishmentPatchRequest.EmailAddress != null)
        {
            Result<bool> uniquenessCheckResult = await EstablishmentCombinationExistsAsync(
                establishment.EstablishmentName,
                establishment.EstablishmentEmail,
                establishmentId);

            if (!uniquenessCheckResult.IsSuccess)
            {
                var exception = EstablishmentException.DuplicateResource();
                return Result<EstablishmentResponse>.Failure(ErrorType.Conflict,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }
        }

        Result<Establishment> updateResult = await _establishmentRepository
            .UpdateAsync(establishment);
        if (!updateResult.IsSuccess)
        {
            return Result<EstablishmentResponse>.Failure(
                updateResult.ErrorType,
                updateResult.Errors);
        }

        Result<Establishment> commitResult = await SafeCommitAsync(
            () => Result<Establishment>.Success(establishment));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EstablishmentResponse>.Success(
                commitResult.Value!.ToEstablishmentResponse());
        }

        return Result<EstablishmentResponse>.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    public async Task<Result> DeleteEstablishmentAsync(EstablishmentId establishmentId)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result result = await _establishmentRepository
            .DeleteAsync(establishmentId);
        if (!result.IsSuccess)
        {
            return result;
        }

        Result<bool> commitResult = await SafeCommitAsync(
            () => Result<bool>.Success(true));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result.Success();
        }
        return Result.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    private async Task<Result<bool>> EstablishmentCombinationExistsAsync(
        EstablishmentName establishmentName,
        EmailAddress emailAddress,
        EstablishmentId? establishmentId = null)
    {
        Result<EstablishmentId> idResult = establishmentId == null
            ? IdHelper.ValidateAndCreateId<EstablishmentId>(Guid.NewGuid().ToString())
            : IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId.Value.ToString());

        if (!idResult.IsSuccess)
        {
            return Result<bool>.Failure(
                idResult.ErrorType, idResult.Errors);
        }

        EstablishmentId idToCheck = idResult.Value!;

        bool exists = await _establishmentRepository
            .ExistsWithNameAndEmailAsync(establishmentName, emailAddress, idToCheck);
        if (exists)
        {
            return Result<bool>.Failure(ErrorType.Conflict,
                new Error(EstablishmentException.DuplicateResource().Code.ToString(),
                    EstablishmentException.DuplicateResource().Message.ToString()));
        }

        return Result<bool>.Success(false);
    }

    public async Task<Result<int>> CountAsync(ISpecification<Establishment> spec)
        => await _establishmentRepository.CountAsync(spec);

    public async Task<Result<EstablishmentResponse>> GetEstablishmentByIdAsync(EstablishmentId establishmentId)
    {
        Result<Establishment> establishmentResult = await _establishmentRepository.GetByIdAsync(establishmentId);
        return establishmentResult.IsSuccess
            ? Result<EstablishmentResponse>.Success(establishmentResult.Value!.ToEstablishmentResponse())
            : Result<EstablishmentResponse>.Failure(establishmentResult.ErrorType, establishmentResult.Errors);
    }

    public async Task<Result<IEnumerable<EstablishmentResponse>>> GetEstablishmentsAsync(ISpecification<Establishment> spec)
    {
        Result<IEnumerable<Establishment>> establishmentsResult = await _establishmentRepository.ListAsync(spec);
        return establishmentsResult.IsSuccess
            ? Result<IEnumerable<EstablishmentResponse>>.Success(establishmentsResult.Value!.Select(e => e.ToEstablishmentResponse()))
            : Result<IEnumerable<EstablishmentResponse>>.Failure(establishmentsResult.ErrorType, establishmentsResult.Errors);
    }
}
