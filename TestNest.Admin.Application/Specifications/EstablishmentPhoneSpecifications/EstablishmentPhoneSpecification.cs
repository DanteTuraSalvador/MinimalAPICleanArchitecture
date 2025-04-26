using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.Extensions;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Specifications.EstablishmentPhoneSpecifications;

public class EstablishmentPhoneSpecification : BaseSpecification<EstablishmentPhone>
{
    public EstablishmentPhoneSpecification(EstablishmentPhoneId establishmentPhoneId)
        : base(p => p.Id == establishmentPhoneId) { }

    public EstablishmentPhoneSpecification(
        string establishmentId = null,
        string establishmentPhoneId = null,
        string? phoneNumber = null,
        bool? isPrimary = null,
        string? sortBy = null,
        string sortOrder = "asc",
        int? pageNumber = null,
        int? pageSize = null)
    {
        var spec = new BaseSpecification<EstablishmentPhone>();

        if (!string.IsNullOrEmpty(establishmentPhoneId))
        {
            Result<EstablishmentPhoneId> phoneIdResult = IdHelper
                .ValidateAndCreateId<EstablishmentPhoneId>(establishmentPhoneId);
            if (phoneIdResult.IsSuccess)
            {
                spec = new BaseSpecification<EstablishmentPhone>(p => p.Id == phoneIdResult.Value!);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(establishmentId))
            {
                Result<EstablishmentId> establishmentIdResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
                if (establishmentIdResult.IsSuccess)
                {
                    var idSpec = new BaseSpecification<EstablishmentPhone>(p => p.EstablishmentId == establishmentIdResult.Value!);
                    spec = spec.And(idSpec);
                }
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var phoneSpec = new BaseSpecification<EstablishmentPhone>(
                    p => p.EstablishmentPhoneNumber.PhoneNo.Contains(phoneNumber));
                spec = spec.And(phoneSpec);
            }

            if (isPrimary.HasValue)
            {
                var primarySpec = new BaseSpecification<EstablishmentPhone>(
                    p => p.IsPrimary == isPrimary.Value);
                spec = spec.And(primarySpec);
            }
        }

        Criteria = spec.Criteria;

        if (!string.IsNullOrEmpty(sortBy))
        {
            SortDirection direction = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;
            switch (sortBy.ToLowerInvariant())
            {
                case "phonenumber":
                    AddOrderBy(p => p.EstablishmentPhoneNumber.PhoneNo, direction);
                    break;

                case "isprimary":
                    AddOrderBy(p => p.IsPrimary, direction);
                    break;

                case "establishmentid":
                    AddOrderBy(p => p.EstablishmentId, direction);
                    break;

                case "id":
                case "establishmentphoneid":
                    AddOrderBy(p => p.Id, direction);
                    break;

                default:
                    AddOrderBy(p => p.Id, SortDirection.Ascending);
                    break;
            }
        }
        else
        {
            AddOrderBy(p => p.Id, SortDirection.Ascending);
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }
}
