using System.Transactions;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.Application.Mappings;
using TestNest.Admin.Application.Services.Base;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Requests.SocialMediaPlatform;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Services;

public class SocialMediaPlatformService(
    ISocialMediaPlatformRepository socialMediaRepository,
    IUnitOfWork unitOfWork,
    IDatabaseExceptionHandlerFactory exceptionHandlerFactory,
    ILogger<SocialMediaPlatformService> logger) : BaseService(unitOfWork, logger, exceptionHandlerFactory), ISocialMediaPlatformService
{
    private readonly ISocialMediaPlatformRepository _socialMediaRepository = socialMediaRepository;

    public async Task<Result<SocialMediaPlatformResponse>> CreateSocialMediaPlatformAsync(
        SocialMediaPlatformForCreationRequest socialMediaPlatformForCreationRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<SocialMediaName> socialMediaNameResult = SocialMediaName
            .Create(socialMediaPlatformForCreationRequest.Name,
                socialMediaPlatformForCreationRequest.PlatformURL);

        if (!socialMediaNameResult.IsSuccess)
        {
            return Result<SocialMediaPlatformResponse>.Failure(
                ErrorType.Validation,
                [.. socialMediaNameResult.Errors]);
        }

        Result<SocialMediaPlatform> existingPlatformResult = await _socialMediaRepository
            .GetSocialMediaPlatformByNameAsync(socialMediaNameResult.Value!.Name);

        if (existingPlatformResult.IsSuccess)
        {
            var exception = SocialMediaPlatformException.DuplicateResource();
            return Result<SocialMediaPlatformResponse>.Failure(
                ErrorType.Conflict,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        Result<SocialMediaPlatform> socialMediaPlatformResult = SocialMediaPlatform
            .Create(socialMediaNameResult.Value!);

        if (!socialMediaPlatformResult.IsSuccess)
        {
            return Result<SocialMediaPlatformResponse>.Failure(
                ErrorType.Validation,
                [.. socialMediaPlatformResult.Errors]);
        }

        SocialMediaPlatform socialMediaPlatform = socialMediaPlatformResult.Value!;
        _ = await _socialMediaRepository.AddAsync(socialMediaPlatform);

        Result<SocialMediaPlatform> commitResult = await SafeCommitAsync(
            () => Result<SocialMediaPlatform>.Success(socialMediaPlatform));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<SocialMediaPlatformResponse>.Success(
                commitResult.Value!.ToSocialMediaPlatformResponse());
        }
        return Result<SocialMediaPlatformResponse>.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    public async Task<Result<SocialMediaPlatformResponse>> UpdateSocialMediaPlatformAsync(
     SocialMediaId socialMediaId,
     SocialMediaPlatformForUpdateRequest socialMediaPlatformUpdateDto)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<SocialMediaPlatform> validatedSocialMediaPlatform = await _socialMediaRepository.GetByIdAsync(socialMediaId);
        if (!validatedSocialMediaPlatform.IsSuccess)
        {
            return Result<SocialMediaPlatformResponse>.Failure(
                validatedSocialMediaPlatform.ErrorType,
                validatedSocialMediaPlatform.Errors);
        }

        SocialMediaPlatform socialMediaPlatform = validatedSocialMediaPlatform.Value!;
        await _socialMediaRepository.DetachAsync(socialMediaPlatform);

        Result<SocialMediaName> socialMediaName = SocialMediaName.Create(
            socialMediaPlatformUpdateDto.Name,
            socialMediaPlatformUpdateDto.PlatformURL);

        if (!socialMediaName.IsSuccess)
        {
            return Result<SocialMediaPlatformResponse>.Failure(
                ErrorType.Validation,
                [.. socialMediaName.Errors]);
        }

        Result<SocialMediaPlatform> updatedSocialMediaPlatformResult = socialMediaPlatform
            .WithSocialMediaName(socialMediaName.Value!);

        if (!updatedSocialMediaPlatformResult.IsSuccess)
        {
            return Result<SocialMediaPlatformResponse>.Failure(updatedSocialMediaPlatformResult.ErrorType, updatedSocialMediaPlatformResult.Errors);
        }

        Result<SocialMediaPlatform> updateResult = await _socialMediaRepository
            .UpdateAsync(updatedSocialMediaPlatformResult.Value!);
        if (!updateResult.IsSuccess)
        {
            return Result<SocialMediaPlatformResponse>.Failure(
                updateResult.ErrorType,
                updateResult.Errors);
        }

        Result<SocialMediaPlatform> commitResult = await SafeCommitAsync(
            () => updateResult);

        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<SocialMediaPlatformResponse>.Success(
                commitResult.Value!.ToSocialMediaPlatformResponse());
        }
        return Result<SocialMediaPlatformResponse>.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    public async Task<Result> DeleteSocialMediaPlatformAsync(SocialMediaId socialMediaId)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        Result result = await _socialMediaRepository.DeleteAsync(socialMediaId);
        if (!result.IsSuccess)
        {
            return result;
        }

        Result<bool> commitResult = await SafeCommitAsync(
            () => result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorType, result.Errors));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result.Success();
        }
        return Result.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result<SocialMediaPlatformResponse>> GetSocialMediaPlatformByIdAsync(SocialMediaId socialMediaId)
    {
        Result<SocialMediaPlatform> platformResult = await _socialMediaRepository.GetByIdAsync(socialMediaId);
        if (!platformResult.IsSuccess)
        {
            return Result<SocialMediaPlatformResponse>.Failure(platformResult.ErrorType, platformResult.Errors);
        }
        return Result<SocialMediaPlatformResponse>.Success(platformResult.Value!.ToSocialMediaPlatformResponse());
    }

    public async Task<Result<IEnumerable<SocialMediaPlatformResponse>>> GetAllSocialMediaPlatformsAsync(ISpecification<SocialMediaPlatform> spec)
    {
        Result<IEnumerable<SocialMediaPlatform>> platformsResult = await _socialMediaRepository.ListAsync(spec);
        if (!platformsResult.IsSuccess)
        {
            return Result<IEnumerable<SocialMediaPlatformResponse>>.Failure(platformsResult.ErrorType, platformsResult.Errors);
        }
        IEnumerable<SocialMediaPlatformResponse> platformResponses = platformsResult.Value!.Select(p => p.ToSocialMediaPlatformResponse());
        return Result<IEnumerable<SocialMediaPlatformResponse>>.Success(platformResponses);
    }

    public async Task<Result<int>> CountAsync(ISpecification<SocialMediaPlatform> spec)
        => await _socialMediaRepository.CountAsync(spec);
}
