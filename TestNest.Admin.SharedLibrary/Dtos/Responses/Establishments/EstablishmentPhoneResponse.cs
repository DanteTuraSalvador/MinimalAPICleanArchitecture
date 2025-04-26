namespace TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

public class EstablishmentPhoneResponse
{
    public string EstablishmentPhoneId { get; set; }
    public string EstablishmentId { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsPrimary { get; set; }
}
