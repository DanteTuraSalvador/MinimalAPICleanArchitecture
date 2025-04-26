namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class AddressException : Exception
{
    public enum ErrorCode
    {
        EmptyAddressLine,
        EmptyCity,
        EmptyMunicipality,
        EmptyProvince,
        EmptyRegion,
        EmptyCountry,
        InvalidLatitude,
        InvalidLongitude,
        NullAddress,
        InvalidAddress
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.EmptyAddressLine, "Address line cannot be empty." },
        { ErrorCode.EmptyCity, "City cannot be empty." },
        { ErrorCode.EmptyMunicipality, "Municipality cannot be empty." },
        { ErrorCode.EmptyProvince, "Province cannot be empty." },
        { ErrorCode.EmptyRegion, "Region cannot be empty." },
        { ErrorCode.EmptyCountry, "Country cannot be empty." },
        { ErrorCode.InvalidLatitude, "Latitude must be between -90 and 90." },
        { ErrorCode.InvalidLongitude, "Longitude must be between -180 and 180." },
        { ErrorCode.NullAddress, "Address cannot be empty or null." },
        { ErrorCode.InvalidAddress, "Invalid address." }
    };

    public ErrorCode Code { get; }

    public AddressException(ErrorCode code) : base(ErrorMessages[code])
    {
        Code = code;
    }

    public static AddressException EmptyAddressLine() => new AddressException(ErrorCode.EmptyAddressLine);

    public static AddressException EmptyCity() => new AddressException(ErrorCode.EmptyCity);

    public static AddressException EmptyMunicipality() => new AddressException(ErrorCode.EmptyMunicipality);

    public static AddressException EmptyProvince() => new AddressException(ErrorCode.EmptyProvince);

    public static AddressException EmptyRegion() => new AddressException(ErrorCode.EmptyRegion);

    public static AddressException EmptyCountry() => new AddressException(ErrorCode.EmptyCountry);

    public static AddressException InvalidLatitude() => new AddressException(ErrorCode.InvalidLatitude);

    public static AddressException InvalidLongitude() => new AddressException(ErrorCode.InvalidLongitude);

    public static AddressException NullAddress() => new AddressException(ErrorCode.NullAddress);

    public static AddressException InvalidAddress() => new AddressException(ErrorCode.InvalidAddress);
}