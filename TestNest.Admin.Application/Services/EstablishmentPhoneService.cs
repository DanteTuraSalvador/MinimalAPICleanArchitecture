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
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Services;

public class EstablishmentPhoneService(
    IEstablishmentPhoneRepository establishmentPhoneRepository,
    IEstablishmentRepository establishmentRepository,
    IUnitOfWork unitOfWork,
    IDatabaseExceptionHandlerFactory exceptionHandlerFactory,
    ILogger<EstablishmentPhoneService> logger) : BaseService(unitOfWork, logger, exceptionHandlerFactory), IEstablishmentPhoneService
{
    private readonly IEstablishmentPhoneRepository _establishmentPhoneRepository = establishmentPhoneRepository;
    private readonly IEstablishmentRepository _establishmentRepository = establishmentRepository;
    private readonly ILogger<EstablishmentPhoneService> _logger = logger;

    public async Task<Result<EstablishmentPhoneResponse>> GetEstablishmentPhoneByIdAsync(EstablishmentPhoneId establishmentPhoneId)
    {
        Result<EstablishmentPhone> establishmentPhoneResult = await _establishmentPhoneRepository.GetByIdAsync(establishmentPhoneId);
        return establishmentPhoneResult.IsSuccess
            ? Result<EstablishmentPhoneResponse>.Success(establishmentPhoneResult.Value!.ToEstablishmentPhoneResponse())
            : Result<EstablishmentPhoneResponse>.Failure(establishmentPhoneResult.ErrorType, establishmentPhoneResult.Errors);
    }

    public async Task<Result<EstablishmentPhoneResponse>> CreateEstablishmentPhoneAsync(EstablishmentPhoneForCreationRequest establishmentPhoneForCreationRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            Result<EstablishmentId> establishmentIdResult = IdHelper
                .ValidateAndCreateId<EstablishmentId>(establishmentPhoneForCreationRequest.EstablishmentId);

            Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(establishmentPhoneForCreationRequest.PhoneNumber);

            var combinedValidationResult = Result.Combine(
                establishmentIdResult.ToResult(),
                phoneNumberResult.ToResult());

            if (!combinedValidationResult.IsSuccess)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    ErrorType.Validation,
                    [.. combinedValidationResult.Errors]);
            }

            Result<Establishment> establishmentResult = await _establishmentRepository
                .GetByIdAsync(establishmentIdResult.Value!);

            if (!establishmentResult.IsSuccess)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    establishmentResult.ErrorType,
                    [.. establishmentResult.Errors]);
            }

            Result<bool> uniquenessCheckResult = await EstablishmentPhoneCombinationExistsAsync(
                phoneNumberResult.Value!,
                establishmentIdResult.Value!);

            if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    ErrorType.Conflict,
                    new Error("Validation", $"Phone number '{phoneNumberResult.Value!.PhoneNo}' already exists for this establishment."));
            }

            Result<EstablishmentPhone> establishmentPhoneResult = EstablishmentPhone.Create(
                establishmentIdResult.Value!,
                phoneNumberResult.Value!,
                establishmentPhoneForCreationRequest.IsPrimary);

            if (!establishmentPhoneResult.IsSuccess)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    establishmentPhoneResult.ErrorType,
                    [.. establishmentPhoneResult.Errors]);
            }

            EstablishmentPhone newPhone = establishmentPhoneResult.Value!;
            if (newPhone.IsPrimary)
            {
                Result setNonPrimaryResult = await _establishmentPhoneRepository
                    .SetNonPrimaryForEstablishmentAsync(newPhone.EstablishmentId, EstablishmentPhoneId.Empty());

                if (!setNonPrimaryResult.IsSuccess)
                {
                    return Result<EstablishmentPhoneResponse>.Failure(setNonPrimaryResult.ErrorType, [.. setNonPrimaryResult.Errors]);
                }
            }

            _ = await _establishmentPhoneRepository.AddAsync(newPhone);
            Result<EstablishmentPhone> commitResult = await SafeCommitAsync(() => Result<EstablishmentPhone>.Success(newPhone));

            if (commitResult.IsSuccess)
            {
                scope.Complete();
                return Result<EstablishmentPhoneResponse>.Success(commitResult.Value!.ToEstablishmentPhoneResponse());
            }

            return Result<EstablishmentPhoneResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating establishment phone number.");
            return Result<EstablishmentPhoneResponse>.Failure(ErrorType.Internal, [new Error("ServiceError", ex.Message)]);
        }
    }

    public async Task<Result<EstablishmentPhoneResponse>> UpdateEstablishmentPhoneAsync(
        EstablishmentPhoneId establishmentPhoneId,
        EstablishmentPhoneForUpdateRequest establishmentPhoneForUpdateRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
                                               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                               TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            Result<EstablishmentId> establishmentIdResult = IdHelper
                .ValidateAndCreateId<EstablishmentId>(establishmentPhoneForUpdateRequest.EstablishmentId);
            if (!establishmentIdResult.IsSuccess)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    ErrorType.Validation, establishmentIdResult.Errors);
            }

            EstablishmentId updateEstablishmentId = establishmentIdResult.Value!;
            bool establishmentExists = await _establishmentRepository.ExistsAsync(updateEstablishmentId);
            if (!establishmentExists)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    ErrorType.NotFound, new Error("NotFound", $"Establishment with ID '{updateEstablishmentId}' not found."));
            }

            Result<EstablishmentPhone> existingPhoneResult = await _establishmentPhoneRepository
                .GetByIdAsync(establishmentPhoneId);
            if (!existingPhoneResult.IsSuccess)
            {
                return Result<EstablishmentPhoneResponse>.Failure(existingPhoneResult.ErrorType, existingPhoneResult.Errors);
            }
            EstablishmentPhone existingPhone = existingPhoneResult.Value!;
            await _establishmentPhoneRepository.DetachAsync(existingPhone);

            if (existingPhone.EstablishmentId != updateEstablishmentId)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    ErrorType.Unauthorized,
                    new Error("Unauthorized", $"Cannot update phone. The provided EstablishmentId '{updateEstablishmentId}' does not match the existing phone's EstablishmentId '{existingPhone.EstablishmentId}'."));
            }

            Result<PhoneNumber> phoneNumberResult = PhoneNumber.Update(establishmentPhoneForUpdateRequest.PhoneNumber);
            if (!phoneNumberResult.IsSuccess)
            {
                return Result<EstablishmentPhoneResponse>.Failure(ErrorType.Validation, phoneNumberResult.Errors);
            }

            PhoneNumber updatedPhoneNumber = phoneNumberResult.Value!;
            EstablishmentPhone updatedPhone = existingPhone.WithPhoneNumber(updatedPhoneNumber).Value!;

            // Check for uniqueness of phone number within the establishment (excluding the current one)
            Result<bool> uniquenessCheckResult = await EstablishmentPhoneCombinationExistsAsync(
                updatedPhoneNumber,
                updateEstablishmentId,
                establishmentPhoneId);

            if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    ErrorType.Conflict,
                    new Error("Validation", $"Phone number '{updatedPhoneNumber.PhoneNo}' already exists for this establishment."));
            }

            if (establishmentPhoneForUpdateRequest.IsPrimary != existingPhone.IsPrimary)
            {
                updatedPhone = updatedPhone.WithIsPrimary(establishmentPhoneForUpdateRequest.IsPrimary).Value!;
                if (updatedPhone.IsPrimary)
                {
                    _ = await _establishmentPhoneRepository.SetNonPrimaryForEstablishmentAsync(updatedPhone.EstablishmentId, updatedPhone.EstablishmentPhoneId);
                }
            }

            Result<EstablishmentPhone> updateResult = await _establishmentPhoneRepository.UpdateAsync(updatedPhone);
            Result<EstablishmentPhone> commitResult = await SafeCommitAsync(() => updateResult);

            if (commitResult.IsSuccess)
            {
                scope.Complete();
                return Result<EstablishmentPhoneResponse>.Success(commitResult.Value!.ToEstablishmentPhoneResponse());
            }
            return Result<EstablishmentPhoneResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating establishment phone number.");
            return Result<EstablishmentPhoneResponse>.Failure(ErrorType.Internal, new Error("ServiceError", ex.Message));
        }
    }

    public async Task<Result<EstablishmentPhoneResponse>> PatchEstablishmentPhoneAsync(
        EstablishmentPhoneId establishmentPhoneId,
        EstablishmentPhonePatchRequest establishmentPhonePatchRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
                                               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                               TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            Result<EstablishmentPhone> existingPhoneResult = await _establishmentPhoneRepository
                .GetByIdAsync(establishmentPhoneId);
            if (!existingPhoneResult.IsSuccess)
            {
                return Result<EstablishmentPhoneResponse>.Failure(existingPhoneResult.ErrorType, existingPhoneResult.Errors);
            }
            EstablishmentPhone existingPhone = existingPhoneResult.Value!;
            await _establishmentPhoneRepository.DetachAsync(existingPhone);

            EstablishmentPhone updatedPhone = existingPhone;
            bool hasChanges = false;
            PhoneNumber? updatedPhoneNumber = null;

            if (!string.IsNullOrEmpty(establishmentPhonePatchRequest.PhoneNumber) && establishmentPhonePatchRequest.PhoneNumber != existingPhone.EstablishmentPhoneNumber.PhoneNo)
            {
                Result<PhoneNumber> phoneNumberResult = PhoneNumber.Update(establishmentPhonePatchRequest.PhoneNumber);
                if (!phoneNumberResult.IsSuccess)
                {
                    return Result<EstablishmentPhoneResponse>.Failure(ErrorType.Validation, phoneNumberResult.Errors);
                }
                updatedPhoneNumber = phoneNumberResult.Value!;
                updatedPhone = updatedPhone.WithPhoneNumber(updatedPhoneNumber).Value!;
                hasChanges = true;
            }
            else
            {
                updatedPhoneNumber = existingPhone.EstablishmentPhoneNumber;
            }

            Result<bool> uniquenessCheckResult = await EstablishmentPhoneCombinationExistsAsync(
                updatedPhoneNumber,
                existingPhone.EstablishmentId,
                establishmentPhoneId);

            if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
            {
                return Result<EstablishmentPhoneResponse>.Failure(
                    ErrorType.Conflict,
                    new Error("Validation", $"Phone number '{updatedPhoneNumber.PhoneNo}' already exists for this establishment."));
            }

            if (establishmentPhonePatchRequest.IsPrimary.HasValue && establishmentPhonePatchRequest.IsPrimary != existingPhone.IsPrimary)
            {
                updatedPhone = updatedPhone.WithIsPrimary(establishmentPhonePatchRequest.IsPrimary.Value).Value!;
                if (updatedPhone.IsPrimary)
                {
                    _ = await _establishmentPhoneRepository.SetNonPrimaryForEstablishmentAsync(updatedPhone.EstablishmentId, updatedPhone.EstablishmentPhoneId);
                }
                hasChanges = true;
            }

            if (hasChanges)
            {
                Result<EstablishmentPhone> updateResult = await _establishmentPhoneRepository.UpdateAsync(updatedPhone);
                Result<EstablishmentPhone> commitResult = await SafeCommitAsync(() => updateResult);
                if (commitResult.IsSuccess)
                {
                    scope.Complete();
                    return Result<EstablishmentPhoneResponse>.Success(commitResult.Value!.ToEstablishmentPhoneResponse());
                }
                return Result<EstablishmentPhoneResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
            }

            return Result<EstablishmentPhoneResponse>.Success(existingPhone.ToEstablishmentPhoneResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error patching establishment phone number.");
            return Result<EstablishmentPhoneResponse>.Failure(ErrorType.Internal, [new Error("ServiceError", ex.Message)]);
        }
    }

    public async Task<Result> DeleteEstablishmentPhoneAsync(EstablishmentPhoneId establishmentPhoneId)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
                                               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                               TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            Result<EstablishmentPhone> existingPhoneResult = await _establishmentPhoneRepository
                .GetByIdAsync(establishmentPhoneId);
            if (!existingPhoneResult.IsSuccess)
            {
                return Result.Failure(ErrorType.NotFound,
                                      new Error("NotFound", $"EstablishmentPhone with ID '{establishmentPhoneId}' not found."));
            }

            Result deleteResult = await _establishmentPhoneRepository.DeleteAsync(establishmentPhoneId);
            Result<bool> commitResult = await SafeCommitAsync(() => Result<bool>.Success(true));

            if (commitResult.IsSuccess)
            {
                scope.Complete();
                return Result.Success();
            }
            return Result.Failure(commitResult.ErrorType, commitResult.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting establishment phone number.");
            return Result.Failure(ErrorType.Internal, [new Error("ServiceError", ex.Message)]);
        }
    }

    public async Task<Result<IEnumerable<EstablishmentPhoneResponse>>> ListAsync(ISpecification<EstablishmentPhone> spec)
    {
        Result<IEnumerable<EstablishmentPhone>> establishmentPhonesResult = await _establishmentPhoneRepository.ListAsync(spec);
        return establishmentPhonesResult.IsSuccess
            ? Result<IEnumerable<EstablishmentPhoneResponse>>.Success(
                establishmentPhonesResult.Value!.Select(ep => ep.ToEstablishmentPhoneResponse()))
            : Result<IEnumerable<EstablishmentPhoneResponse>>.Failure(
                establishmentPhonesResult.ErrorType, establishmentPhonesResult.Errors);
    }

    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentPhone> spec)
        => await _establishmentPhoneRepository.CountAsync(spec);

    public async Task<Result<bool>> EstablishmentPhoneCombinationExistsAsync(
        PhoneNumber phoneNumber,
        EstablishmentId establishmentId,
        EstablishmentPhoneId? excludedPhoneId = null)
    {
        Result<EstablishmentPhoneId> idResult = excludedPhoneId == null
            ? IdHelper.ValidateAndCreateId<EstablishmentPhoneId>(Guid.NewGuid().ToString()) // Dummy ID for creation check
            : IdHelper.ValidateAndCreateId<EstablishmentPhoneId>(excludedPhoneId.Value.ToString());

        if (!idResult.IsSuccess)
        {
            return Result<bool>.Failure(
                idResult.ErrorType,
                idResult.Errors);
        }

        EstablishmentPhoneId idToCheck = idResult.Value!;

        bool exists = await _establishmentPhoneRepository.PhoneExistsWithSameNumberInEstablishment(
            idToCheck,
            phoneNumber.PhoneNo,
            establishmentId);

        if (exists)
        {
            return Result<bool>.Success(true);
        }

        return Result<bool>.Success(false);
    }
}
