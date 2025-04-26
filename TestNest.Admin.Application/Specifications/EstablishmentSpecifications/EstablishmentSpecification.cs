using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.Extensions;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.Application.Specifications.EstablishmentSpecifications;

public class EstablishmentSpecification : BaseSpecification<Establishment>
{
    public EstablishmentSpecification(
        string establishmentName,
        string establishmentEmail,
        int? establishmentStatusId,
        string establishmentId = null,
        string sortBy = null,
        string sortDirection = "asc",
        int? pageNumber = null,
        int? pageSize = null)
    {
        var spec = new BaseSpecification<Establishment>();

        if (!string.IsNullOrEmpty(establishmentId))
        {
            Result<EstablishmentId> establishmentIdValidatedResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
            if (establishmentIdValidatedResult.IsSuccess)
            {
                var idSpec = new BaseSpecification<Establishment>(e => e.Id == establishmentIdValidatedResult.Value!);
                spec = spec.And(idSpec);
            }
        }
        else
        {
            if (establishmentStatusId.HasValue)
            {
                Result<EstablishmentStatus> statusResult = EstablishmentStatus.FromId(establishmentStatusId.Value);
                if (statusResult.IsSuccess)
                {
                    EstablishmentStatus? statusToFilter = statusResult.Value;
                    var statusIdSpec = new BaseSpecification<Establishment>(e => e.EstablishmentStatus == statusToFilter!);
                    spec = spec.And(statusIdSpec);
                }
            }

            if (!string.IsNullOrEmpty(establishmentName))
            {
                var establishmentNameSpec = new BaseSpecification<Establishment>(
                    e => EF.Functions.Like(e.EstablishmentName.Name.ToLowerInvariant(), $"%{establishmentName.ToLowerInvariant()}%"));
                spec = spec.And(establishmentNameSpec);
            }

            if (!string.IsNullOrEmpty(establishmentEmail))
            {
                var establishmentEmailSpec = new BaseSpecification<Establishment>(
                    e => EF.Functions.Like(e.EstablishmentEmail.Email.ToLowerInvariant(), $"%{establishmentEmail.ToLowerInvariant()}%"));
                spec = spec.And(establishmentEmailSpec);
            }
        }

        Criteria = spec.Criteria;

        if (!string.IsNullOrEmpty(sortBy))
        {
            var direction = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;
            switch (sortBy.ToLowerInvariant())
            {
                case "establishmentname":
                    AddOrderBy(e => e.EstablishmentName.Name, direction);
                    break;

                case "establishmentemail":
                    AddOrderBy(e => e.EstablishmentEmail.Email, direction);
                    break;

                case "establishmentstatusid":
                    AddOrderBy(e => EF.Property<int>(e, "EstablishmentStatusId"), direction);
                    break;

                case "establishmentid":
                default:
                    AddOrderBy(e => e.Id, direction);
                    break;
            }
        }
        else
        {
            AddOrderBy(e => e.Id, SortDirection.Ascending);
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }

    public EstablishmentSpecification(EstablishmentId establishmentId)
       : base(e => e.Id == establishmentId) { }
}
