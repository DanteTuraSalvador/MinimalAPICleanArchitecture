namespace TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

public class EstablishmentAddressResponse
{
    public string EstablishmentAddressId { get; set; }
    public string EstablishmentId { get; set; }
    public string AddressLine { get; set; }
    public string City { get; set; }
    public string Municipality { get; set; }
    public string Province { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsPrimary { get; set; }
}
