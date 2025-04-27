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

public class EstablishmentContactService(
    IEstablishmentContactRepository establishmentContactRepository,
    IEstablishmentRepository establishmentRepository,
    IUnitOfWork unitOfWork,
    IDatabaseExceptionHandlerFactory exceptionHandlerFactory,
    ILogger<EstablishmentContactService> logger) : BaseService(unitOfWork, logger, exceptionHandlerFactory), IEstablishmentContactService
{
    private readonly IEstablishmentContactRepository _establishmentContactRepository = establishmentContactRepository;
    private readonly IEstablishmentRepository _establishmentRepository = establishmentRepository;
    private readonly ILogger<EstablishmentContactService> _logger = logger;

    public async Task<Result<EstablishmentContactResponse>> CreateEstablishmentContactAsync(
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
            return Result<EstablishmentContactResponse>.Failure(
                ErrorType.Validation,
                [.. combinedValidationResult.Errors]);
        }

        Result<Establishment> establishmentResult = await _establishmentRepository
            .GetByIdAsync(establishmentIdResult.Value!);

        if (!establishmentResult.IsSuccess)
        {
            return Result<EstablishmentContactResponse>.Failure(
                establishmentResult.ErrorType,
                [.. establishmentResult.Errors]);
        }

        Result<bool> uniquenessCheckResult = await EstablishmentContactCombinationExistsAsync(
            personNameResult.Value!,
            phoneNumberResult.Value!,
            establishmentIdResult.Value!);

        if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
        {
            return Result<EstablishmentContactResponse>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"A contact with the same name and phone number already exists for this establishment."));
        }

        if (creationRequest.IsPrimary)
        {
            Result setNonPrimaryResult = await _establishmentContactRepository
                .SetNonPrimaryForEstablishmentContanctAsync(establishmentIdResult.Value!, EstablishmentContactId.Empty());

            if (!setNonPrimaryResult.IsSuccess)
            {
                return Result<EstablishmentContactResponse>.Failure(setNonPrimaryResult.ErrorType, [.. setNonPrimaryResult.Errors]);
            }
        }

        Result<EstablishmentContact> establishmentContactResult = EstablishmentContact.Create(
            establishmentIdResult.Value!,
            personNameResult.Value!,
            phoneNumberResult.Value!,
            creationRequest.IsPrimary);

        if (!establishmentContactResult.IsSuccess)
        {
            return Result<EstablishmentContactResponse>.Failure(
                establishmentContactResult.ErrorType,
                [.. establishmentContactResult.Errors]);
        }

        EstablishmentContact establishmentContact = establishmentContactResult.Value!;
        _ = await _establishmentContactRepository.AddAsync(establishmentContact);
        Result<EstablishmentContact> commitResult = await SafeCommitAsync(() => Result<EstablishmentContact>.Success(establishmentContact));

        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EstablishmentContactResponse>.Success(commitResult.Value!.ToEstablishmentContactResponse());
        }

        return Result<EstablishmentContactResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result<EstablishmentContactResponse>> GetEstablishmentContactByIdAsync(EstablishmentContactId establishmentContactId)
    {
        Result<EstablishmentContact> contactResult = await _establishmentContactRepository.GetByIdAsync(establishmentContactId);
        return contactResult.IsSuccess
            ? Result<EstablishmentContactResponse>.Success(contactResult.Value!.ToEstablishmentContactResponse())
            : Result<EstablishmentContactResponse>.Failure(contactResult.ErrorType, contactResult.Errors);
    }

    public async Task<Result<IEnumerable<EstablishmentContactResponse>>> GetEstablishmentContactsAsync(ISpecification<EstablishmentContact> spec)
    {
        Result<IEnumerable<EstablishmentContact>> contactsResult = await _establishmentContactRepository.ListAsync(spec);
        return contactsResult.IsSuccess
            ? Result<IEnumerable<EstablishmentContactResponse>>.Success(
                contactsResult.Value!.Select(c => c.ToEstablishmentContactResponse()))
            : Result<IEnumerable<EstablishmentContactResponse>>.Failure(contactsResult.ErrorType, contactsResult.Errors);
    }

    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentContact> spec)
        => await _establishmentContactRepository.CountAsync(spec);

    public async Task<Result<EstablishmentContactResponse>> UpdateEstablishmentContactAsync(
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
            return Result<EstablishmentContactResponse>.Failure(
                ErrorType.Validation, establishmentIdResult.Errors);
        }

        EstablishmentId updateEstablishmentId = establishmentIdResult.Value!;
        bool establishmentExists = await _establishmentRepository.ExistsAsync(updateEstablishmentId);
        if (!establishmentExists)
        {
            return Result<EstablishmentContactResponse>.Failure(
                ErrorType.NotFound, new Error("NotFound", $"Establishment with ID '{updateEstablishmentId}' not found."));
        }

        Result<EstablishmentContact> existingContactResult = await _establishmentContactRepository
            .GetByIdAsync(establishmentContactId);
        if (!existingContactResult.IsSuccess)
        {
            return Result<EstablishmentContactResponse>.Failure(ErrorType.NotFound, existingContactResult.Errors);
        }

        EstablishmentContact existingContact = existingContactResult.Value!;
        await _establishmentContactRepository.DetachAsync(existingContact);

        if (existingContact.EstablishmentId != updateEstablishmentId)
        {
            return Result<EstablishmentContactResponse>.Failure(
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
                return Result<EstablishmentContactResponse>.Failure(personNameResult.ErrorType, personNameResult.Errors);
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
                return Result<EstablishmentContactResponse>.Failure(phoneNumberResult.ErrorType, phoneNumberResult.Errors);
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
            return Result<EstablishmentContactResponse>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"A contact with the same name and phone number already exists for this establishment."));
        }

        if (updateRequest.IsPrimary != existingContact.IsPrimary)
        {
            if (updateRequest.IsPrimary)
            {
                Result setNonePrimaryResult = await _establishmentContactRepository
                    .SetNonPrimaryForEstablishmentContanctAsync(updateEstablishmentId, establishmentContactId);
                if (!setNonePrimaryResult.IsSuccess)
                {
                    return Result<EstablishmentContactResponse>.Failure(setNonePrimaryResult.ErrorType, setNonePrimaryResult.Errors);
                }
            }
            updatedContact = updatedContact.WithPrimaryFlag(updateRequest.IsPrimary).Value!;
            hasChanges = true;
        }

        if (!hasChanges)
        {
            return Result<EstablishmentContactResponse>.Success(updatedContact.ToEstablishmentContactResponse());
        }

        Result<EstablishmentContact> updateResult = await _establishmentContactRepository.UpdateAsync(updatedContact);
        if (!updateResult.IsSuccess)
        {
            return Result<EstablishmentContactResponse>.Failure(updateResult.ErrorType, updateResult.Errors);
        }

        Result<EstablishmentContact> commitResult = await SafeCommitAsync(() => updateResult);
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EstablishmentContactResponse>.Success(commitResult.Value!.ToEstablishmentContactResponse());
        }
        return Result<EstablishmentContactResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    private static bool HasContactUpdate(EstablishmentContactForUpdateRequest request) =>
        request.ContactPersonFirstName != null ||
        request.ContactPersonMiddleName != null ||
        request.ContactPersonLastName != null;

    public async Task<Result<EstablishmentContactResponse>> PatchEstablishmentContactAsync(
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
            return Result<EstablishmentContactResponse>.Failure(existingContactResult.ErrorType, existingContactResult.Errors);
        }

        EstablishmentContact existingContact = existingContactResult.Value!;
        await _establishmentContactRepository.DetachAsync(existingContact);

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(existingContact.EstablishmentId.ToString());
        if (!establishmentIdResult.IsSuccess)
        {
            return Result<EstablishmentContactResponse>.Failure(ErrorType.Validation, establishmentIdResult.Errors);
        }

        EstablishmentId requestEstablishmentId = establishmentIdResult.Value!;
        bool establishmentExists = await _establishmentRepository.ExistsAsync(requestEstablishmentId);
        if (!establishmentExists)
        {
            return Result<EstablishmentContactResponse>.Failure(
                ErrorType.NotFound,
                new Error("NotFound", $"Establishment with ID '{requestEstablishmentId}' not found."));
        }

        if (requestEstablishmentId != existingContact.EstablishmentId)
        {
            return Result<EstablishmentContactResponse>.Failure(
                ErrorType.Unauthorized,
                new Error("Unauthorized", $"Cannot patch contact. The provided EstablishmentId '{requestEstablishmentId}' does not match the existing contact's EstablishmentId '{existingContact.EstablishmentId}'."));
        }

        EstablishmentContact updatedContact = existingContact;
        PersonName? updatedPersonName = null;
        PhoneNumber? updatedPhoneNumber = null;
        bool hasChanges = false;

        if (patchRequest.ContactPersonFirstName != existingContact.ContactPerson.FirstName ||
            patchRequest.ContactPersonMiddleName != existingContact.ContactPerson.MiddleName ||
            patchRequest.ContactPersonLastName != existingContact.ContactPerson.LastName)
        {
            Result<PersonName> personNameResult = PersonName.Create(
                patchRequest.ContactPersonFirstName ?? existingContact.ContactPerson.FirstName,
                patchRequest.ContactPersonMiddleName ?? existingContact.ContactPerson.MiddleName,
                patchRequest.ContactPersonLastName ?? existingContact.ContactPerson.LastName);

            if (!personNameResult.IsSuccess)
            {
                return Result<EstablishmentContactResponse>.Failure(personNameResult.ErrorType, personNameResult.Errors);
            }
            updatedPersonName = personNameResult.Value!;
            updatedContact = updatedContact.WithContactPerson(updatedPersonName).Value!;
            hasChanges = true;
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
                return Result<EstablishmentContactResponse>.Failure(phoneNumberResult.ErrorType, phoneNumberResult.Errors);
            }
            updatedPhoneNumber = phoneNumberResult.Value!;
            updatedContact = updatedContact.WithContactPhone(phoneNumberResult.Value!).Value!;
            hasChanges = true;
        }
        else
        {
            updatedPhoneNumber = existingContact.ContactPhone;
        }

        Result<bool> uniquenessCheckResult = await EstablishmentContactCombinationExistsAsync(
            updatedPersonName,
            updatedPhoneNumber,
            requestEstablishmentId,
            establishmentContactId);

        if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
        {
            return Result<EstablishmentContactResponse>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"A contact with the same name and phone number already exists for this establishment."));
        }

        if (patchRequest.IsPrimary.HasValue && patchRequest.IsPrimary != existingContact.IsPrimary)
        {
            if (patchRequest.IsPrimary.Value)
            {
                Result setNonPrimaryResult = await _establishmentContactRepository
                    .SetNonPrimaryForEstablishmentContanctAsync(requestEstablishmentId, establishmentContactId);

                if (!setNonPrimaryResult.IsSuccess)
                {
                    return Result<EstablishmentContactResponse>.Failure(setNonPrimaryResult.ErrorType, setNonPrimaryResult.Errors);
                }
            }
            updatedContact = updatedContact.WithPrimaryFlag(patchRequest.IsPrimary.Value).Value!;
            hasChanges = true;
        }

        if (hasChanges)
        {
            Result<EstablishmentContact> updateResult = await _establishmentContactRepository.UpdateAsync(updatedContact);
            Result<EstablishmentContact> commitResult = await SafeCommitAsync(() => updateResult);
            if (commitResult.IsSuccess)
            {
                scope.Complete();
                return Result<EstablishmentContactResponse>.Success(commitResult.Value!.ToEstablishmentContactResponse());
            }
            return Result<EstablishmentContactResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
        }

        return Result<EstablishmentContactResponse>.Success(existingContact.ToEstablishmentContactResponse());
    }

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
                new Error("DeletionNotAllowed", $"Cannot delete the primary contact for Establishment ID '{existingContact.EstablishmentId}'. Please set another contact as primary first."));
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
