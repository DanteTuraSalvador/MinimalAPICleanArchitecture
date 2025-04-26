using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.Extensions;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Specifications.SoicalMediaPlatfomrSpecifications;

public class SocialMediaPlatformSpecification : BaseSpecification<SocialMediaPlatform>
{
    public SocialMediaPlatformSpecification(
        string name = null,
        string platformURL = null,
        string sortBy = "Id",
        string sortDirection = "asc",
        int? pageNumber = 1,
        int? pageSize = 10,
        string socialMediaId = null) // Included socialMediaId for filtering
    {
        BaseSpecification<SocialMediaPlatform> spec = new BaseSpecification<SocialMediaPlatform>();

        if (!string.IsNullOrEmpty(socialMediaId))
        {
            var socialMediaIdValidatedResult = IdHelper.ValidateAndCreateId<SocialMediaId>(socialMediaId);
            if (socialMediaIdValidatedResult.IsSuccess)
            {
                var idSpec = new BaseSpecification<SocialMediaPlatform>(e => e.SocialMediaId == socialMediaId);
                spec = spec.And(idSpec);
            }
        }

        //if (!string.IsNullOrEmpty(socialMediaId))
        //{
        //    var estIdSpec = new BaseSpecification<SocialMediaPlatform>(e => e.SocialMediaId == socialMediaId);
        //    spec = spec.And(estIdSpec);
        //}

        if (!string.IsNullOrEmpty(name))
        {
            var nameSpec = new BaseSpecification<SocialMediaPlatform>(
                s => EF.Functions.Like(s.SocialMediaName.Name.ToLower(), $"%{name.ToLower()}%"));
            spec = spec.And(nameSpec);
        }

        if (!string.IsNullOrEmpty(platformURL))
        {
            var urlSpec = new BaseSpecification<SocialMediaPlatform>(
                s => EF.Functions.Like(s.SocialMediaName.PlatformURL.ToLower(), $"%{platformURL.ToLower()}%"));
            spec = spec.And(urlSpec);
        }

        Criteria = spec.Criteria;

        if (!string.IsNullOrEmpty(sortBy))
        {
            var direction = sortDirection.ToLower() == "desc" ? SortDirection.Descending : SortDirection.Ascending;
            switch (sortBy.ToLower())
            {
                case "name":
                    AddOrderBy(s => s.SocialMediaName.Name, direction);
                    break;

                case "platformurl":
                    AddOrderBy(s => s.SocialMediaName.PlatformURL, direction);
                    break;

                case "id":
                default:
                    AddOrderBy(s => s.Id, direction);
                    break;
            }
        }
        else
        {
            AddOrderBy(s => s.Id, SortDirection.Ascending);
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }

    public SocialMediaPlatformSpecification(SocialMediaId socialMediaId) : base(s => s.Id == socialMediaId)
    {
    }
}
