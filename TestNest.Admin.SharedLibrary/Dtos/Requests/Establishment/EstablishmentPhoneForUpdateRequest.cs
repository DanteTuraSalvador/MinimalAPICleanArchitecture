namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;

public class EstablishmentPhoneForUpdateRequest
{
    public string EstablishmentId { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsPrimary { get; set; }
}
