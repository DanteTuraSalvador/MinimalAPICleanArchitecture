namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;

public class EstablishmentPhonePatchRequest
{
    public string? PhoneNumber { get; set; }
    public bool? IsPrimary { get; set; }
}
