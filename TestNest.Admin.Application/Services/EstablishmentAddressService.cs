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

public class EstablishmentAddressService(
    IEstablishmentAddressRepository establishmentAddressRepository,
    IEstablishmentRepository establishmentRepository,
    IUnitOfWork unitOfWork,
    IDatabaseExceptionHandlerFactory exceptionHandlerFactory,
    ILogger<EstablishmentAddressService> logger) : BaseService(unitOfWork, logger, exceptionHandlerFactory), IEstablishmentAddressService
{
    private readonly IEstablishmentRepository _establishmentRepository = establishmentRepository;
    private readonly IEstablishmentAddressRepository _establishmentAddressRepository = establishmentAddressRepository;
    private readonly ILogger<EstablishmentAddressService> _logger = logger;

    public async Task<Result<EstablishmentAddressResponse>> CreateEstablishmentAddressAsync(
        EstablishmentAddressForCreationRequest request)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
                                               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                               TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(request.EstablishmentId);

        Result<Address> addressResult = Address.Create(
            request.AddressLine,
            request.Municipality,
            request.City,
            request.Province,
            request.Region,
            request.Country,
            (decimal)request.Latitude,
            (decimal)request.Longitude);

        var combinedValidationResult = Result.Combine(
            establishmentIdResult.ToResult(),
            addressResult.ToResult());

        if (!combinedValidationResult.IsSuccess)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                ErrorType.Validation,
                [.. combinedValidationResult.Errors]);
        }

        EstablishmentId establishmentIdToCheck = establishmentIdResult.Value!;
        decimal latitudeToCheck = addressResult.Value!.Latitude;
        decimal longitudeToCheck = addressResult.Value!.Longitude;

        Result<bool> uniquenessCheckResult = await EstablishmentAddressCombinationExistsAsync(
            latitudeToCheck,
            longitudeToCheck,
            establishmentIdToCheck);

        if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"An address with the same latitude ({latitudeToCheck}) and longitude ({longitudeToCheck}) already exists for this establishment."));
        }

        Result<Establishment> establishmentResult = await _establishmentRepository
            .GetByIdAsync(establishmentIdToCheck);

        if (!establishmentResult.IsSuccess)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                establishmentResult.ErrorType,
                [.. establishmentResult.Errors]);
        }

        if (request.IsPrimary)
        {
            Result setNonPrimaryResult = await _establishmentAddressRepository
                .SetNonPrimaryForEstablishmentAsync(establishmentIdToCheck, EstablishmentAddressId.Empty());

            if (!setNonPrimaryResult.IsSuccess)
            {
                return Result<EstablishmentAddressResponse>.Failure(setNonPrimaryResult.ErrorType, [.. setNonPrimaryResult.Errors]);
            }
        }

        Result<EstablishmentAddress> establishmentAddressResult = EstablishmentAddress.Create(
            establishmentIdToCheck,
            addressResult.Value!,
            request.IsPrimary);

        if (!establishmentAddressResult.IsSuccess)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                establishmentAddressResult.ErrorType,
                [.. establishmentAddressResult.Errors]);
        }

        EstablishmentAddress establishmentAddress = establishmentAddressResult.Value!;
        _ = await _establishmentAddressRepository.AddAsync(establishmentAddress);
        Result<EstablishmentAddress> commitResult = await SafeCommitAsync(() => Result<EstablishmentAddress>.Success(establishmentAddress));

        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EstablishmentAddressResponse>.Success(commitResult.Value!.ToEstablishmentAddressResponse());
        }

        return Result<EstablishmentAddressResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result<EstablishmentAddressResponse>> UpdateEstablishmentAddressAsync(
        EstablishmentAddressId establishmentAddressId,
        EstablishmentAddressForUpdateRequest establishmentAddressForUpdateRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(establishmentAddressForUpdateRequest.EstablishmentId.ToString());
        if (!establishmentIdResult.IsSuccess)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                ErrorType.Validation, establishmentIdResult.Errors);
        }

        EstablishmentId updateEstablishmentId = establishmentIdResult.Value!;
        bool establishmentExists = await _establishmentRepository.ExistsAsync(updateEstablishmentId);
        if (!establishmentExists)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                ErrorType.NotFound, new Error("NotFound", $"Establishment with ID '{updateEstablishmentId}' not found."));
        }

        Result<EstablishmentAddress> existingAddressResult = await _establishmentAddressRepository
            .GetByIdAsync(establishmentAddressId);
        if (!existingAddressResult.IsSuccess)
        {
            return Result<EstablishmentAddressResponse>.Failure(ErrorType.Validation, existingAddressResult.Errors);
        }

        EstablishmentAddress existingAddress = existingAddressResult.Value!;
        await _establishmentAddressRepository.DetachAsync(existingAddress);

        if (existingAddress.EstablishmentId != updateEstablishmentId)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                ErrorType.Unauthorized,
                new Error("Unauthorized", $"Cannot update address. The provided EstablishmentId '{updateEstablishmentId}' does not match the existing address's EstablishmentId '{existingAddress.EstablishmentId}'."));
        }

        EstablishmentAddress? updatedAddress = existingAddress;
        bool hasChanges = false;

        decimal updatedLatitude = existingAddress.Address.Latitude;
        decimal updatedLongitude = existingAddress.Address.Longitude;

        if (HasAddressUpdate(establishmentAddressForUpdateRequest))
        {
            Result<Address> addressResult = Address.Create(
                establishmentAddressForUpdateRequest.AddressLine ?? existingAddress.Address.AddressLine,
                establishmentAddressForUpdateRequest.City ?? existingAddress.Address.City,
                establishmentAddressForUpdateRequest.Municipality ?? existingAddress.Address.Municipality,
                establishmentAddressForUpdateRequest.Province ?? existingAddress.Address.Province,
                establishmentAddressForUpdateRequest.Region ?? existingAddress.Address.Region,
                establishmentAddressForUpdateRequest.Country ?? existingAddress.Address.Country,
                (decimal)establishmentAddressForUpdateRequest.Latitude,
                (decimal)establishmentAddressForUpdateRequest.Longitude
            );
            if (!addressResult.IsSuccess)
            {
                return Result<EstablishmentAddressResponse>.Failure(addressResult.ErrorType, addressResult.Errors);
            }

            updatedAddress = updatedAddress.WithAddress(addressResult.Value!).Value!;
            updatedLatitude = addressResult.Value!.Latitude;
            updatedLongitude = addressResult.Value!.Longitude;
            hasChanges = true;
        }

        // Check for uniqueness based on Latitude and Longitude (excluding the current address being updated)
        Result<bool> uniquenessCheckResult = await EstablishmentAddressCombinationExistsAsync(
            updatedLatitude,
            updatedLongitude,
            updateEstablishmentId,
            establishmentAddressId);

        if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"An address with the same latitude ({updatedLatitude}) and longitude ({updatedLongitude}) already exists for this establishment."));
        }

        if (establishmentAddressForUpdateRequest.IsPrimary && establishmentAddressForUpdateRequest.IsPrimary != existingAddress.IsPrimary)
        {
            if (establishmentAddressForUpdateRequest.IsPrimary)
            {
                Result setNonPrimaryResult = await _establishmentAddressRepository
                    .SetNonPrimaryForEstablishmentAsync(updateEstablishmentId, establishmentAddressId);

                if (!setNonPrimaryResult.IsSuccess)
                {
                    return Result<EstablishmentAddressResponse>.Failure(
                        setNonPrimaryResult.ErrorType, setNonPrimaryResult.Errors);
                }

                updatedAddress = updatedAddress.WithIsPrimary(true).Value!;
                hasChanges = true;
            }
            else
            {
                updatedAddress = updatedAddress.WithIsPrimary(false).Value!;
                hasChanges = true;
            }
        }

        if (!hasChanges)
        {
            return Result<EstablishmentAddressResponse>.Success(existingAddress.ToEstablishmentAddressResponse());
        }

        Result<EstablishmentAddress> updateResult = await _establishmentAddressRepository.UpdateAsync(updatedAddress);
        Result<EstablishmentAddress> commitResult = await SafeCommitAsync(() => updateResult);
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EstablishmentAddressResponse>.Success(commitResult.Value.ToEstablishmentAddressResponse());
        }
        return Result<EstablishmentAddressResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result> DeleteEstablishmentAddressAsync(
        EstablishmentAddressId establishmentAddressId)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
                                               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                               TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentAddress> existingAddressResult = await _establishmentAddressRepository
            .GetByIdAsync(establishmentAddressId);
        if (!existingAddressResult.IsSuccess)
        {
            return Result.Failure(ErrorType.NotFound,
                                  new Error("NotFound", $"EstablishmentAddress with ID '{establishmentAddressId}' not found."));
        }

        EstablishmentAddress existingAddress = existingAddressResult.Value!;

        if (existingAddress.IsPrimary)
        {
            return Result.Failure(ErrorType.Validation,
                                  new Error("DeletionNotAllowed", $"Cannot delete the primary address for Establishment ID '{existingAddress.EstablishmentId}'. Please set another address as primary first."));
        }

        Result deleteResult = await _establishmentAddressRepository.DeleteAsync(establishmentAddressId);
        Result<bool> commitResult = await SafeCommitAsync(() => Result<bool>.Success(true));

        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result.Success();
        }
        return Result.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result<EstablishmentAddressResponse>> GetEstablishmentAddressByIdAsync(EstablishmentAddressId establishmentAddressId)
    {
        Result<EstablishmentAddress> establishmentAddressResult = await _establishmentAddressRepository.GetByIdAsync(establishmentAddressId);
        return establishmentAddressResult.IsSuccess
            ? Result<EstablishmentAddressResponse>.Success(establishmentAddressResult.Value!.ToEstablishmentAddressResponse())
            : Result<EstablishmentAddressResponse>.Failure(establishmentAddressResult.ErrorType, establishmentAddressResult.Errors);
    }

    private async Task<Result<bool>> EstablishmentAddressCombinationExistsAsync(
        decimal latitude,
        decimal longitude,
        EstablishmentId establishmentId,
        EstablishmentAddressId? excludedAddressId = null)
    {
        Result<EstablishmentAddressId> idResult = excludedAddressId == null
            ? IdHelper.ValidateAndCreateId<EstablishmentAddressId>(Guid.NewGuid().ToString())
            : IdHelper.ValidateAndCreateId<EstablishmentAddressId>(excludedAddressId.Value.ToString());

        if (!idResult.IsSuccess)
        {
            return Result<bool>.Failure(
                idResult.ErrorType,
                idResult.Errors);
        }

        EstablishmentAddressId idToCheck = idResult.Value!;

        bool exists = await _establishmentAddressRepository.AddressExistsWithSameCoordinatesInEstablishment(
            idToCheck,
            latitude,
            longitude,
            establishmentId);

        if (exists)
        {
            return Result<bool>.Success(true);
        }

        return Result<bool>.Success(false);
    }

    public async Task<Result<EstablishmentAddressResponse>> PatchEstablishmentAddressAsync(
        EstablishmentAddressId establishmentAddressId,
        EstablishmentAddressPatchRequest establishmentAddressPatchRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<EstablishmentAddress> existingAddressResult = await _establishmentAddressRepository
            .GetByIdAsync(establishmentAddressId);
        if (!existingAddressResult.IsSuccess)
        {
            return Result<EstablishmentAddressResponse>.Failure(existingAddressResult.ErrorType, existingAddressResult.Errors);
        }

        EstablishmentAddress existingAddress = existingAddressResult.Value!;
        await _establishmentAddressRepository.DetachAsync(existingAddress);

        EstablishmentAddress updatedAddress = existingAddress;
        bool hasChanges = false;

        decimal updatedLatitude = existingAddress.Address.Latitude;
        decimal updatedLongitude = existingAddress.Address.Longitude;

        if (HasAddressPatchUpdate(establishmentAddressPatchRequest, existingAddress.Address))
        {
            Result<Address> addressResult = Address.Create(
                establishmentAddressPatchRequest.AddressLine ?? existingAddress.Address.AddressLine,
                establishmentAddressPatchRequest.City ?? existingAddress.Address.City,
                establishmentAddressPatchRequest.Municipality ?? existingAddress.Address.Municipality,
                establishmentAddressPatchRequest.Province ?? existingAddress.Address.Province,
                establishmentAddressPatchRequest.Region ?? existingAddress.Address.Region,
                establishmentAddressPatchRequest.Country ?? existingAddress.Address.Country,
                establishmentAddressPatchRequest.Latitude.HasValue ? (decimal)establishmentAddressPatchRequest.Latitude.Value : existingAddress.Address.Latitude,
                establishmentAddressPatchRequest.Longitude.HasValue ? (decimal)establishmentAddressPatchRequest.Longitude.Value : existingAddress.Address.Longitude
            );

            if (!addressResult.IsSuccess)
            {
                return Result<EstablishmentAddressResponse>.Failure(
                    addressResult.ErrorType, [.. addressResult.Errors]);
            }

            updatedAddress = updatedAddress.WithAddress(addressResult.Value!).Value!;
            updatedLatitude = addressResult.Value!.Latitude;
            updatedLongitude = addressResult.Value!.Longitude;
            hasChanges = true;
        }

        Result<bool> uniquenessCheckResult = await EstablishmentAddressCombinationExistsAsync(
            updatedLatitude,
            updatedLongitude,
            existingAddress.EstablishmentId,
            establishmentAddressId);

        if (!uniquenessCheckResult.IsSuccess || uniquenessCheckResult.Value)
        {
            return Result<EstablishmentAddressResponse>.Failure(
                ErrorType.Conflict,
                new Error("Validation", $"An address with the same latitude ({updatedLatitude}) and longitude ({updatedLongitude}) already exists for this establishment."));
        }

        if (establishmentAddressPatchRequest.IsPrimary.HasValue && establishmentAddressPatchRequest.IsPrimary != existingAddress.IsPrimary)
        {
            if (establishmentAddressPatchRequest.IsPrimary.Value)
            {
                Result setNonPrimaryResult = await _establishmentAddressRepository
                    .SetNonPrimaryForEstablishmentAsync(existingAddress.EstablishmentId, establishmentAddressId);

                if (!setNonPrimaryResult.IsSuccess)
                {
                    return Result<EstablishmentAddressResponse>.Failure(
                        setNonPrimaryResult.ErrorType, [.. setNonPrimaryResult.Errors]);
                }
            }
            updatedAddress = updatedAddress.WithIsPrimary(establishmentAddressPatchRequest.IsPrimary.Value).Value!;
            hasChanges = true;
        }

        if (hasChanges)
        {
            Result<EstablishmentAddress> updateResult = await _establishmentAddressRepository.UpdateAsync(updatedAddress);
            Result<EstablishmentAddress> commitResult = await SafeCommitAsync(() => updateResult);
            if (commitResult.IsSuccess)
            {
                scope.Complete();
                return Result<EstablishmentAddressResponse>.Success(commitResult.Value.ToEstablishmentAddressResponse());
            }
            return Result<EstablishmentAddressResponse>.Failure(commitResult.ErrorType, commitResult.Errors);
        }

        return Result<EstablishmentAddressResponse>.Success(existingAddress.ToEstablishmentAddressResponse());
    }

    private static bool HasAddressPatchUpdate(EstablishmentAddressPatchRequest
        request, Address existingAddress) =>
        request.AddressLine != existingAddress.AddressLine ||
        request.City != existingAddress.City ||
        request.Municipality != existingAddress.Municipality ||
        request.Province != existingAddress.Province ||
        request.Region != existingAddress.Region ||
        request.Country != existingAddress.Country ||
        request.Latitude.HasValue && (decimal)request.Latitude.Value != existingAddress.Latitude ||
        request.Longitude.HasValue && (decimal)request.Longitude.Value != existingAddress.Longitude;

    private static bool HasAddressUpdate(EstablishmentAddressForUpdateRequest request) =>
        request.AddressLine != null ||
        request.City != null ||
        request.Municipality != null ||
        request.Province != null ||
        request.Region != null ||
        request.Country != null ||
        request.Latitude != default ||
        request.Longitude != default;

    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentAddress> spec)
        => await _establishmentAddressRepository.CountAsync(spec);

    public async Task<Result<IEnumerable<EstablishmentAddressResponse>>> ListAsync(ISpecification<EstablishmentAddress> spec)
    {
        Result<IEnumerable<EstablishmentAddress>> establishmentAddressesResult = await _establishmentAddressRepository.ListAsync(spec);
        return establishmentAddressesResult.IsSuccess
            ? Result<IEnumerable<EstablishmentAddressResponse>>.Success(
                establishmentAddressesResult.Value!.Select(ea => ea.ToEstablishmentAddressResponse()))
            : Result<IEnumerable<EstablishmentAddressResponse>>.Failure(
                establishmentAddressesResult.ErrorType, establishmentAddressesResult.Errors);
    }
}
