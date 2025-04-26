using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class Address : ValueObject
{
    public string AddressLine { get; }
    public string City { get; }
    public string Municipality { get; }
    public string Province { get; }
    public string Region { get; }
    public string Country { get; }
    public decimal Latitude { get; }
    public decimal Longitude { get; }

    public bool IsEmpty() => this == Empty();

    private static readonly Lazy<Address> _empty = new(() => new Address());

    public static Address Empty() => _empty.Value;

    public Address() => (AddressLine, City, Municipality, Province, Region, Country, Latitude, Longitude)
        = (string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0);

    private Address(string addressLine, string city, string municipality, string province,
        string region, string country, decimal latitude, decimal longitude)
            => (AddressLine, City, Municipality, Province, Region, Country, Latitude, Longitude)
                = (addressLine, city, municipality, province, region, country, latitude, longitude);

    public static Result<Address> Create(string addressLine, string city, string municipality,
        string province, string region, string country, decimal latitude, decimal longitude)
    {
        Result validationResult = ValidateAddress(addressLine, city, municipality, province, region, country, latitude, longitude);

        return validationResult.IsSuccess
            ? Result<Address>.Success(new Address(addressLine, city, municipality, province, region, country, latitude, longitude))
            : Result<Address>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<Address> Update(string addressLine, string city, string municipality,
        string province, string region, string country, decimal latitude, decimal longitude)
        => Create(addressLine, city, municipality, province, region, country, latitude, longitude);

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return AddressLine;
        yield return City;
        yield return Municipality;
        yield return Province;
        yield return Region;
        yield return Country;
        yield return Latitude;
        yield return Longitude;
    }

    private static Result ValidateAddress(string addressLine, string city, string municipality,
      string province, string region, string country, decimal latitude, decimal longitude)
        => Result.Combine(
            Guard.AgainstNullOrWhiteSpace(addressLine, AddressException.EmptyAddressLine),
            Guard.AgainstNullOrWhiteSpace(city, AddressException.EmptyCity),
            Guard.AgainstNullOrWhiteSpace(municipality, AddressException.EmptyMunicipality),
            Guard.AgainstNullOrWhiteSpace(province, AddressException.EmptyProvince),
            Guard.AgainstNullOrWhiteSpace(region, AddressException.EmptyRegion),
            Guard.AgainstNullOrWhiteSpace(country, AddressException.EmptyCountry),
            Guard.AgainstCondition(latitude is < (-90) or > 90, AddressException.InvalidLatitude),
            Guard.AgainstCondition(longitude is < (-180) or > 180, AddressException.InvalidLongitude)
        );

    public override string ToString()
        => $"{AddressLine}, {City}, {Municipality}, {Province}, {Region}, {Country} (Lat: {Latitude}, Long: {Longitude})";
}
