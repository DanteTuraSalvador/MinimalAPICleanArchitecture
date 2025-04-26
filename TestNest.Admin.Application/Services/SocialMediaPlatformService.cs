using System.Transactions;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.Application.Services.Base;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.SharedLibrary.Common.Results;
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

    public async Task<Result<SocialMediaPlatform>> CreateSocialMediaPlatformAsync(
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
            return Result<SocialMediaPlatform>.Failure(
                ErrorType.Validation,
                [.. socialMediaNameResult.Errors]);
        }

        Result<SocialMediaPlatform> existingPlatformResult = await _socialMediaRepository
            .GetSocialMediaPlatformByNameAsync(socialMediaNameResult.Value!.Name);

        if (existingPlatformResult.IsSuccess)
        {
            var exception = SocialMediaPlatformException.DuplicateResource();
            return Result<SocialMediaPlatform>.Failure(
                ErrorType.Conflict,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        Result<SocialMediaPlatform> socialMediaPlatformResult = SocialMediaPlatform
            .Create(socialMediaNameResult.Value!);

        if (!socialMediaPlatformResult.IsSuccess)
        {
            return Result<SocialMediaPlatform>.Failure(
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
            return commitResult;
        }
        return commitResult;
    }

    public async Task<Result<SocialMediaPlatform>> UpdateSocialMediaPlatformAsync(
        SocialMediaId socialMediaId,
        SocialMediaPlatformForUpdateRequest socialMediaPlatformUpdateDto)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<SocialMediaPlatform> validatedSocialMediaPlatform = await _socialMediaRepository.GetByIdAsync(socialMediaId);
        if (!validatedSocialMediaPlatform.IsSuccess)
        {
            return validatedSocialMediaPlatform;
        }

        SocialMediaPlatform socialMediaPlatform = validatedSocialMediaPlatform.Value!;
        await _socialMediaRepository.DetachAsync(socialMediaPlatform);

        Result<SocialMediaName> socialMediaName = SocialMediaName.Create(
            socialMediaPlatformUpdateDto.Name,
            socialMediaPlatformUpdateDto.PlatformURL);

        if (!socialMediaName.IsSuccess)
        {
            return Result<SocialMediaPlatform>.Failure(
                ErrorType.Validation,
                socialMediaName.Errors.ToArray());
        }

        Result<SocialMediaPlatform> updatedSocialMediaPlatformResult = socialMediaPlatform
            .WithSocialMediaName(socialMediaName.Value!);

        if (!updatedSocialMediaPlatformResult.IsSuccess)
        {
            return updatedSocialMediaPlatformResult;
        }

        Result<SocialMediaPlatform> updateResult = await _socialMediaRepository
            .UpdateAsync(updatedSocialMediaPlatformResult.Value!);
        if (!updateResult.IsSuccess)
        {
            return Result<SocialMediaPlatform>.Failure(
                updateResult.ErrorType,
                updateResult.Errors);
        }

        Result commitResult = await SafeCommitAsync();
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<SocialMediaPlatform>.Success(updatedSocialMediaPlatformResult.Value!);
        }
        return Result<SocialMediaPlatform>.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result> DeleteSocialMediaPlatformAsync(SocialMediaId socialMediaId)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        Result result = await _socialMediaRepository.DeleteAsync(socialMediaId);
        if (!result.IsSuccess)
        {
            return result;
        }

        Result commitResult = await SafeCommitAsync();
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result.Success();
        }
        return Result.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result<SocialMediaPlatform>> GetSocialMediaPlatformByIdAsync(SocialMediaId socialMediaId)
        => await _socialMediaRepository.GetByIdAsync(socialMediaId);

    public async Task<Result<IEnumerable<SocialMediaPlatform>>> GetAllSocialMediaPlatformsAsync()
        => await _socialMediaRepository.GetAllAsync();

    public async Task<Result<IEnumerable<SocialMediaPlatform>>> GetAllSocialMediaPlatformsAsync(ISpecification<SocialMediaPlatform> spec)
        => await _socialMediaRepository.ListAsync(spec);

    public async Task<Result<int>> CountAsync(ISpecification<SocialMediaPlatform> spec)
        => await _socialMediaRepository.CountAsync(spec);
}
