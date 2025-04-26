namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;

public class EstablishmentAddressPatchRequest
{
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Municipality { get; set; }
    public string? Province { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool? IsPrimary { get; set; }
}
