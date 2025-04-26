using System.Transactions;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.Application.Services.Base;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Services;

public class EstablishmentContactService(
    IEstablishmentContactRepository establishmentContactRepository,
    IEstablishmentRepository establishmentRepository,
    IUnitOfWork unitOfWork,
    IDatabaseExceptionHandlerFactory exceptionHandlerFactory,
    ILogger<EstablishmentContactService> logger) : BaseService(unitOfWork, logger, exceptionHandlerFactory), IEstablishmentContactService
{
    private readonly IEstablishmentContactRepository _establishmentContactRepository = establishmentContactRepository;
    private readonly IEstablishmentRepository _establishmentRepository = establishmentRepository;

    public async Task<Result<EstablishmentContact>> CreateEstablishmentContactAsync(
        EstablishmentContactForCreationRequest creationRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
                                             new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                             TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(creationRequest.EstablishmentId);

        Result<PersonName> personNameResult = PersonName.Create(
            creationRequest.ContactPersonFirstName,
            creationRequest.ContactPersonMiddleName,
            creationRequest.ContactPersonLastName);

        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(
            creationRequest.ContactPhoneNumber);

        var combinedValidationResult = Result.Combine(
            establishmentIdResult.ToResult(),
            personNameResult.ToResult(),
            phoneNumberResult.ToResult());

        if (!combinedValidationResult.IsSuccess)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.Validation,
                [.. combinedValidationResult.Errors]);
        }

        Result<Establishment> establishmentResult = await _establishmentRepository
            .GetByIdAsync(establishmentIdResult.Value!);

        if (!establishmentResult.IsSuccess)
        {
            return Result<EstablishmentContact>.Failure(
                establishmentResult.ErrorType,
                [.. establishmentResult.Errors]);
        }

        Result<bool> uniquenessCheckResult = await EstablishmentContactCombinationExistsAsync(
                personNameResult.Value!,
                phoneNumberResult.Value!,
                establishmentIdResult.Value!);

        if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"A contact with the same name and phone number already exists for this establishment."));
        }

        if (creationRequest.IsPrimary)
        {
            Result setNonPrimaryResult = await _establishmentContactRepository
                .SetNonPrimaryForEstablishmentContanctAsync(establishmentIdResult.Value!, EstablishmentContactId.Empty());

            if (!setNonPrimaryResult.IsSuccess)
            {
                return Result<EstablishmentContact>.Failure(setNonPrimaryResult.ErrorType, [.. setNonPrimaryResult.Errors]);
            }
        }

        Result<EstablishmentContact> establishmentContactResult = EstablishmentContact.Create(
            establishmentIdResult.Value!,
            personNameResult.Value!,
            phoneNumberResult.Value!,
            creationRequest.IsPrimary);

        if (!establishmentContactResult.IsSuccess)
        {
            return Result<EstablishmentContact>.Failure(
                establishmentContactResult.ErrorType,
                [.. establishmentContactResult.Errors]);
        }

        EstablishmentContact establishmentContact = establishmentContactResult.Value!;
        _ = await _establishmentContactRepository.AddAsync(establishmentContact);
        Result<EstablishmentContact> commitResult = await SafeCommitAsync(() => Result<EstablishmentContact>.Success(establishmentContact));

        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return commitResult;
        }

        return commitResult;
    }

    public async Task<Result<EstablishmentContact>> GetEstablishmentContactByIdAsync(EstablishmentContactId establishmentContactId)
        => await _establishmentContactRepository.GetByIdAsync(establishmentContactId);

    public async Task<Result<IEnumerable<EstablishmentContact>>> GetEstablishmentContactsAsync(ISpecification<EstablishmentContact> spec)
        => await _establishmentContactRepository.ListAsync(spec);

    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentContact> spec)
        => await _establishmentContactRepository.CountAsync(spec);

    public async Task<Result<EstablishmentContact>> UpdateEstablishmentContactAsync(
       EstablishmentContactId establishmentContactId,
       EstablishmentContactForUpdateRequest updateRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
                                             new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                             TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(updateRequest.EstablishmentId.ToString());
        if (!establishmentIdResult.IsSuccess)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.Validation, establishmentIdResult.Errors);
        }

        EstablishmentId updateEstablishmentId = establishmentIdResult.Value!;
        bool establishmentExists = await _establishmentRepository.ExistsAsync(updateEstablishmentId);
        if (!establishmentExists)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.NotFound, new Error("NotFound", $"Establishment with ID '{updateEstablishmentId}' not found."));
        }

        Result<EstablishmentContact> existingContactResult = await _establishmentContactRepository
            .GetByIdAsync(establishmentContactId);
        if (!existingContactResult.IsSuccess)
        {
            return Result<EstablishmentContact>.Failure(ErrorType.NotFound, existingContactResult.Errors);
        }

        EstablishmentContact existingContact = existingContactResult.Value!;
        await _establishmentContactRepository.DetachAsync(existingContact);

        if (existingContact.EstablishmentId != updateEstablishmentId)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.Unauthorized,
                new Error("Unauthorized", $"Cannot update contact. The provided EstablishmentId '{updateEstablishmentId}' does not match the existing contact's EstablishmentId '{existingContact.EstablishmentId}'."));
        }

        EstablishmentContact updatedContact = existingContact;
        bool hasChanges = false;
        PersonName? updatedPersonName = null;
        PhoneNumber? updatedPhoneNumber = null;

        if (HasContactUpdate(updateRequest))
        {
            Result<PersonName> personNameResult = PersonName.Create(
                updateRequest.ContactPersonFirstName ?? existingContact.ContactPerson.FirstName,
                updateRequest.ContactPersonMiddleName ?? existingContact.ContactPerson.MiddleName,
                updateRequest.ContactPersonLastName ?? existingContact.ContactPerson.LastName);
            if (!personNameResult.IsSuccess)
            {
                return Result<EstablishmentContact>.Failure(personNameResult.ErrorType, personNameResult.Errors);
            }
            updatedPersonName = personNameResult.Value!;
            updatedContact = updatedContact.WithContactPerson(updatedPersonName).Value!;
            hasChanges = true;
        }
        else
        {
            updatedPersonName = existingContact.ContactPerson;
        }

        if (updateRequest.ContactPhoneNumber != existingContact.ContactPhone.PhoneNo)
        {
            Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(updateRequest.ContactPhoneNumber ?? existingContact.ContactPhone.PhoneNo);
            if (!phoneNumberResult.IsSuccess)
            {
                return Result<EstablishmentContact>.Failure(phoneNumberResult.ErrorType, phoneNumberResult.Errors);
            }
            updatedPhoneNumber = phoneNumberResult.Value!;
            updatedContact = updatedContact.WithContactPhone(updatedPhoneNumber).Value!;
            hasChanges = true;
        }
        else
        {
            updatedPhoneNumber = existingContact.ContactPhone;
        }

        Result<bool> uniquenessCheckResult = await EstablishmentContactCombinationExistsAsync(
                updatedPersonName,
                updatedPhoneNumber,
                updateEstablishmentId,
                establishmentContactId);

        if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"A contact with the same name and phone number already exists for this establishment."));
        }

        if (updateRequest.IsPrimary && updateRequest.IsPrimary != existingContact.IsPrimary)
        {
            if (updateRequest.IsPrimary)
            {
                Result setNonePrimaryResult = await _establishmentContactRepository
                    .SetNonPrimaryForEstablishmentContanctAsync(updateEstablishmentId, establishmentContactId);
                if (!setNonePrimaryResult.IsSuccess)
                {
                    return Result<EstablishmentContact>.Failure(setNonePrimaryResult.ErrorType, setNonePrimaryResult.Errors);
                }
            }
            updatedContact = updatedContact.WithPrimaryFlag(updateRequest.IsPrimary).Value!;
            hasChanges = true;
        }

        if (!hasChanges)
        {
            return Result<EstablishmentContact>.Success(updatedContact);
        }

        Result<EstablishmentContact> updateResult = await _establishmentContactRepository.UpdateAsync(updatedContact);
        if (!updateResult.IsSuccess)
        {
            return Result<EstablishmentContact>.Failure(updateResult.ErrorType, updateResult.Errors);
        }

        Result<EstablishmentContact> commitResult = await SafeCommitAsync(() => Result<EstablishmentContact>.Success(updatedContact));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return commitResult;
        }
        return commitResult;
    }

    private static bool HasContactUpdate(EstablishmentContactForUpdateRequest request) =>
        request.ContactPersonFirstName != null ||
        request.ContactPersonMiddleName != null ||
        request.ContactPersonLastName != null;

    public async Task<Result<EstablishmentContact>> PatchEstablishmentContactAsync(
        EstablishmentContactId establishmentContactId,
        EstablishmentContactPatchRequest patchRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentContact> existingContactResult = await _establishmentContactRepository
            .GetByIdAsync(establishmentContactId);
        if (!existingContactResult.IsSuccess)
        {
            return existingContactResult;
        }

        EstablishmentContact existingContact = existingContactResult.Value!;
        await _establishmentContactRepository.DetachAsync(existingContact);

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(existingContact.EstablishmentId.ToString());
        if (!establishmentIdResult.IsSuccess)
        {
            return Result<EstablishmentContact>.Failure(ErrorType.Validation, establishmentIdResult.Errors);
        }

        EstablishmentId requestEstablishmentId = establishmentIdResult.Value!;
        bool establishmentExists = await _establishmentRepository.ExistsAsync(requestEstablishmentId);
        if (!establishmentExists)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.NotFound,
                new Error("NotFound", $"Establishment with ID '{requestEstablishmentId}' not found."));
        }

        if (requestEstablishmentId != existingContact.EstablishmentId)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.Unauthorized,
                new Error("Unauthorized", $"Cannot patch contact. The provided EstablishmentId '{requestEstablishmentId}' does not match the existing contact's EstablishmentId '{existingContact.EstablishmentId}'."));
        }

        EstablishmentContact updatedContact = existingContact;
        PersonName? updatedPersonName = null;
        PhoneNumber? updatedPhoneNumber = null;

        if (HasContactPatchUpdate(patchRequest, existingContact.ContactPerson))
        {
            Result<PersonName> personNameResult = PersonName.Create(
                patchRequest.ContactPersonFirstName ?? existingContact.ContactPerson.FirstName,
                patchRequest.ContactPersonMiddleName ?? existingContact.ContactPerson.MiddleName,
                patchRequest.ContactPersonLastName ?? existingContact.ContactPerson.LastName);

            if (!personNameResult.IsSuccess)
            {
                return Result<EstablishmentContact>.Failure(personNameResult.ErrorType, personNameResult.Errors);
            }
            updatedPersonName = personNameResult.Value!;
            updatedContact = updatedContact.WithContactPerson(updatedPersonName).Value!;
        }
        else
        {
            updatedPersonName = existingContact.ContactPerson;
        }

        if (patchRequest.ContactPhoneNumber != existingContact.ContactPhone.PhoneNo)
        {
            Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(patchRequest.ContactPhoneNumber ?? existingContact.ContactPhone.PhoneNo);
            if (!phoneNumberResult.IsSuccess)
            {
                return Result<EstablishmentContact>.Failure(phoneNumberResult.ErrorType, phoneNumberResult.Errors);
            }
            updatedPhoneNumber = phoneNumberResult.Value!;
            updatedContact = updatedContact.WithContactPhone(phoneNumberResult.Value!).Value!;
        }
        else
        {
            updatedPhoneNumber = existingContact.ContactPhone;
        }

        // Check for uniqueness (excluding the current contact)
        Result<bool> uniquenessCheckResult = await EstablishmentContactCombinationExistsAsync(
                updatedPersonName,
                updatedPhoneNumber,
                requestEstablishmentId,
                establishmentContactId);

        if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
        {
            return Result<EstablishmentContact>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"A contact with the same name and phone number already exists for this establishment."));
        }

        if (patchRequest.IsPrimary != existingContact.IsPrimary)
        {
            if (patchRequest.IsPrimary!.Value)
            {
                Result setNonPrimaryResult = await _establishmentContactRepository
                    .SetNonPrimaryForEstablishmentContanctAsync(requestEstablishmentId, establishmentContactId);

                if (!setNonPrimaryResult.IsSuccess)
                {
                    return Result<EstablishmentContact>.Failure(setNonPrimaryResult.ErrorType, setNonPrimaryResult.Errors);
                }
            }
            updatedContact = updatedContact.WithPrimaryFlag(patchRequest.IsPrimary.Value).Value!;
        }

        if (updatedContact != existingContact)
        {
            Result<EstablishmentContact> updateResult = await _establishmentContactRepository.UpdateAsync(updatedContact);
            Result<EstablishmentContact> commitResult = await SafeCommitAsync(() => updateResult);
            if (commitResult.IsSuccess)
            {
                scope.Complete();
                return commitResult;
            }
            return commitResult;
        }

        return Result<EstablishmentContact>.Success(updatedContact);
    }

    private static bool HasContactPatchUpdate(EstablishmentContactPatchRequest request, PersonName existingContactPerson) =>
        request.ContactPersonFirstName != existingContactPerson.FirstName ||
        request.ContactPersonMiddleName != existingContactPerson.MiddleName ||
        request.ContactPersonLastName != existingContactPerson.LastName;

    public async Task<Result> DeleteEstablishmentContactAsync(EstablishmentContactId establishmentContactId)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentContact> existingContactResult = await _establishmentContactRepository
            .GetByIdAsync(establishmentContactId);
        if (!existingContactResult.IsSuccess)
        {
            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"EstablishmentContact with ID '{establishmentContactId}' not found."));
        }

        EstablishmentContact existingContact = existingContactResult.Value!;

        if (existingContact.IsPrimary)
        {
            return Result.Failure(ErrorType.Validation,
                 new Error("DeletionNotAllowed", $"Cannot delete the primary contact for Establishment ID '{existingContact.EstablishmentId}'. Please set another address as primary first."));
        }

        Result deleteResult = await _establishmentContactRepository.DeleteAsync(establishmentContactId);
        Result<bool> commitResult = await SafeCommitAsync(() => Result<bool>.Success(true));

        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result.Success();
        }
        return Result.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result<bool>> EstablishmentContactCombinationExistsAsync(
        PersonName contactPerson,
        PhoneNumber contactPhoneNumber,
        EstablishmentId establishmentId,
        EstablishmentContactId? excludedContactId = null)
    {
        Result<EstablishmentContactId> idResult = excludedContactId == null
            ? IdHelper.ValidateAndCreateId<EstablishmentContactId>(Guid.NewGuid().ToString())
            : IdHelper.ValidateAndCreateId<EstablishmentContactId>(excludedContactId.Value.ToString());

        if (!idResult.IsSuccess)
        {
            return Result<bool>.Failure(
                idResult.ErrorType,
                idResult.Errors);
        }

        EstablishmentContactId idToCheck = idResult.Value!;

        bool exists = await _establishmentContactRepository.ContactExistsWithSameDetailsInEstablishment(
            idToCheck,
            contactPerson,
            contactPhoneNumber,
            establishmentId);

        return Result<bool>.Success(exists);
    }
}
