using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.Extensions;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Specifications.EstablishmentAddressesSpecifications;

public class EstablishmentAddressSpecification : BaseSpecification<EstablishmentAddress>
{
    public EstablishmentAddressSpecification(
        int? pageNumber = null,
        int? pageSize = null,
        string sortBy = null,
        string sortOrder = "asc",
        string establishmentId = null,
        string city = null,
        string municipality = null,
        string province = null,
        string region = null,
        string establishmentAddressId = null,
        bool? isPrimary = null)
    {
        var spec = new BaseSpecification<EstablishmentAddress>();

        if (!string.IsNullOrEmpty(establishmentAddressId))
        {
            Result<EstablishmentAddressId> addressIdResult = IdHelper.ValidateAndCreateId<EstablishmentAddressId>(establishmentAddressId);
            if (addressIdResult.IsSuccess)
            {
                spec.Criteria = a => a.Id == addressIdResult.Value!;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(establishmentId))
            {
                Result<EstablishmentId> establishmentIdResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
                if (establishmentIdResult.IsSuccess)
                {
                    var idSpec = new BaseSpecification<EstablishmentAddress>(a => a.EstablishmentId == establishmentIdResult.Value!);
                    spec = spec.And(idSpec);
                }
            }
            if (!string.IsNullOrEmpty(city))
            {
                var citySpec = new BaseSpecification<EstablishmentAddress>(
                    a => EF.Functions.Like(a.Address.City.ToLowerInvariant(), $"%{city.ToLowerInvariant()}%"));
                spec = spec.And(citySpec);
            }
            if (!string.IsNullOrEmpty(municipality))
            {
                var municipalitySpec = new BaseSpecification<EstablishmentAddress>(
                    a => EF.Functions.Like(a.Address.Municipality.ToLowerInvariant(), $"%{municipality.ToLowerInvariant()}%"));
                spec = spec.And(municipalitySpec);
            }
            if (!string.IsNullOrEmpty(province))
            {
                var provinceSpec = new BaseSpecification<EstablishmentAddress>(
                    a => EF.Functions.Like(a.Address.Province.ToLowerInvariant(), $"%{province.ToLowerInvariant()}%"));
                spec = spec.And(provinceSpec);
            }
            if (!string.IsNullOrEmpty(region))
            {
                var regionSpec = new BaseSpecification<EstablishmentAddress>(
                    a => EF.Functions.Like(a.Address.Region.ToLowerInvariant(), $"%{region.ToLowerInvariant()}%"));
                spec = spec.And(regionSpec);
            }
            if (isPrimary.HasValue)
            {
                var isPrimarySpec = new BaseSpecification<EstablishmentAddress>(a => a.IsPrimary == isPrimary.Value);
                spec = spec.And(isPrimarySpec);
            }
        }

        Criteria = spec.Criteria;

        if (!string.IsNullOrEmpty(sortBy))
        {
            SortDirection direction = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;
            switch (sortBy.ToLowerInvariant())
            {
                case "addressline":
                    AddOrderBy(a => a.Address.AddressLine, direction);
                    break;

                case "municipality":
                    AddOrderBy(a => a.Address.Municipality, direction);
                    break;

                case "city":
                    AddOrderBy(a => a.Address.City, direction);
                    break;

                case "province":
                    AddOrderBy(a => a.Address.Province, direction);
                    break;

                case "region":
                    AddOrderBy(a => a.Address.Region, direction);
                    break;

                case "isprimary":
                    AddOrderBy(a => a.IsPrimary, direction);
                    break;

                case "establishmentid":
                    AddOrderBy(a => a.EstablishmentId, direction);
                    break;

                case "id":
                case "establishmentaddressid":
                    AddOrderBy(a => a.Id, direction);
                    break;

                default:
                    AddOrderBy(a => a.Id, SortDirection.Ascending);
                    break;
            }
        }
        else
        {
            AddOrderBy(a => a.Id, SortDirection.Ascending);
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }

    public EstablishmentAddressSpecification(EstablishmentAddressId addressId)
        : base(a => a.Id == addressId) { }
}
