using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.Extensions;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Specifications.EstablishmentMemberSpecifications;

public class EstablishmentMemberSpecification : BaseSpecification<EstablishmentMember>
{
    public EstablishmentMemberSpecification(EstablishmentMemberId establishmentMemberId)
        : base(m => m.Id == establishmentMemberId) { }

    public EstablishmentMemberSpecification(
        string establishmentId = null,
        string establishmentMemberId = null,
        string? employeeId = null,
        string? memberTitle = null,
        string? memberDescription = null,
        string? memberTag = null,
        string? sortBy = null,
        string sortOrder = "asc",
        int? pageNumber = null,
        int? pageSize = null)
    {
        var spec = new BaseSpecification<EstablishmentMember>();

        if (!string.IsNullOrEmpty(establishmentMemberId))
        {
            Result<EstablishmentMemberId> memberIdResult = IdHelper
                .ValidateAndCreateId<EstablishmentMemberId>(establishmentMemberId);
            if (memberIdResult.IsSuccess)
            {
                spec = new BaseSpecification<EstablishmentMember>(m => m.Id == memberIdResult.Value!);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(establishmentId))
            {
                Result<EstablishmentId> establishmentIdResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
                if (establishmentIdResult.IsSuccess)
                {
                    var idSpec = new BaseSpecification<EstablishmentMember>(m => m.EstablishmentId == establishmentIdResult.Value!);
                    spec = spec.And(idSpec);
                }
            }

            if (!string.IsNullOrEmpty(employeeId))
            {
                Result<EmployeeId> employeeIdResult = IdHelper.ValidateAndCreateId<EmployeeId>(employeeId);
                if (employeeIdResult.IsSuccess)
                {
                    var employeeSpec = new BaseSpecification<EstablishmentMember>(m => m.EmployeeId == employeeIdResult.Value!);
                    spec = spec.And(employeeSpec);
                }
            }

            if (!string.IsNullOrEmpty(memberTitle))
            {
                var titleSpec = new BaseSpecification<EstablishmentMember>(m => m.MemberTitle.Title.Contains(memberTitle));
                spec = spec.And(titleSpec);
            }

            if (!string.IsNullOrEmpty(memberDescription))
            {
                var descriptionSpec = new BaseSpecification<EstablishmentMember>(m => m.MemberDescription.Description.Contains(memberDescription));
                spec = spec.And(descriptionSpec);
            }

            if (!string.IsNullOrEmpty(memberTag))
            {
                var tagSpec = new BaseSpecification<EstablishmentMember>(m => m.MemberTag.Tag.Contains(memberTag));
                spec = spec.And(tagSpec);
            }
        }

        Criteria = spec.Criteria;

        if (!string.IsNullOrEmpty(sortBy))
        {
            SortDirection direction = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;
            switch (sortBy.ToLowerInvariant())
            {
                case "membertitle":
                    AddOrderBy(m => m.MemberTitle.Title, direction);
                    break;

                case "memberdescription":
                    AddOrderBy(m => m.MemberDescription.Description, direction);
                    break;

                case "membertag":
                    AddOrderBy(m => m.MemberTag.Tag, direction);
                    break;

                case "employeeid":
                    AddOrderBy(m => m.EmployeeId.Value, direction);
                    break;

                case "establishmentid":
                    AddOrderBy(m => m.EstablishmentId.Value, direction);
                    break;

                case "id":
                case "establishmentmemberid":
                    AddOrderBy(m => m.Id, direction);
                    break;

                default:
                    AddOrderBy(m => m.Id, SortDirection.Ascending);
                    break;
            }
        }
        else
        {
            AddOrderBy(m => m.Id, SortDirection.Ascending);
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }
}
