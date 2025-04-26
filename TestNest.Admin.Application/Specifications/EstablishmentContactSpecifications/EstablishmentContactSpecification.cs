using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.Extensions;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Specifications.EstablishmentContactSpecifications;

public class EstablishmentContactSpecification : BaseSpecification<EstablishmentContact>
{
    public EstablishmentContactSpecification(EstablishmentContactId establishmentContactId)
        : base(a => a.Id == establishmentContactId) { }

    public EstablishmentContactSpecification(
        string establishmentId = null,
        string establishmentContactId = null,
        string? contactPersonFirstName = null,
        string? contactPersonMiddleName = null,
        string? contactPersonLastName = null,
        string? contactPhoneNumber = null,
        bool? isPrimary = null,
        string? sortBy = null,
        string sortOrder = "asc",
        int? pageNumber = null,
        int? pageSize = null)
    {
        var spec = new BaseSpecification<EstablishmentContact>();

        if (!string.IsNullOrEmpty(establishmentContactId))
        {
            Result<EstablishmentContactId> contactIdResult = IdHelper
                .ValidateAndCreateId<EstablishmentContactId>(establishmentContactId);
            if (contactIdResult.IsSuccess)
            {
                spec = new BaseSpecification<EstablishmentContact>(a => a.Id == contactIdResult.Value!);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(establishmentId))
            {
                Result<EstablishmentId> establishmentIdResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
                if (establishmentIdResult.IsSuccess)
                {
                    var idSpec = new BaseSpecification<EstablishmentContact>(a => a.EstablishmentId == establishmentIdResult.Value!);
                    spec = spec.And(idSpec);
                }
            }

            if (!string.IsNullOrEmpty(contactPersonFirstName))
            {
                var firstNameSpec = new BaseSpecification<EstablishmentContact>(
                    c => EF.Functions.Like(c.ContactPerson.FirstName.ToLowerInvariant(), $"%{contactPersonFirstName.ToLowerInvariant()}%"));
                spec = spec.And(firstNameSpec);
            }

            if (!string.IsNullOrEmpty(contactPersonMiddleName))
            {
                var middleNameSpec = new BaseSpecification<EstablishmentContact>(
                    c => EF.Functions.Like(c.ContactPerson.MiddleName.ToLowerInvariant(), $"%{contactPersonMiddleName.ToLowerInvariant()}%"));
                spec = spec.And(middleNameSpec);
            }

            if (!string.IsNullOrEmpty(contactPersonLastName))
            {
                var lastNameSpec = new BaseSpecification<EstablishmentContact>(
                    c => EF.Functions.Like(c.ContactPerson.LastName.ToLowerInvariant(), $"%{contactPersonLastName.ToLowerInvariant()}%"));
                spec = spec.And(lastNameSpec);
            }

            if (!string.IsNullOrEmpty(contactPhoneNumber))
            {
                var phoneSpec = new BaseSpecification<EstablishmentContact>(
                    c => c.ContactPhone.PhoneNo.Contains(contactPhoneNumber));
                spec = spec.And(phoneSpec);
            }

            if (isPrimary.HasValue)
            {
                var primarySpec = new BaseSpecification<EstablishmentContact>(
                    c => c.IsPrimary == isPrimary.Value);
                spec = spec.And(primarySpec);
            }
        }

        Criteria = spec.Criteria;

        if (!string.IsNullOrEmpty(sortBy))
        {
            SortDirection direction = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;
            switch (sortBy.ToLowerInvariant())
            {
                case "contactpersonfirstname":
                    AddOrderBy(c => c.ContactPerson.FirstName, direction);
                    break;

                case "contactpersonmiddlename":
                    AddOrderBy(c => c.ContactPerson.MiddleName, direction);
                    break;

                case "contactpersonlastname":
                    AddOrderBy(c => c.ContactPerson.LastName, direction);
                    break;

                case "contactphone":
                    AddOrderBy(c => c.ContactPhone.PhoneNo, direction);
                    break;

                case "isprimary":
                    AddOrderBy(c => c.IsPrimary, direction);
                    break;

                case "establishmentid":
                    AddOrderBy(c => c.EstablishmentId, direction);
                    break;

                case "id":
                case "establishmentcontactid":
                    AddOrderBy(c => c.Id, direction);
                    break;

                default:
                    AddOrderBy(c => c.Id, SortDirection.Ascending);
                    break;
            }
        }
        else
        {
            AddOrderBy(c => c.Id, SortDirection.Ascending);
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }
}
