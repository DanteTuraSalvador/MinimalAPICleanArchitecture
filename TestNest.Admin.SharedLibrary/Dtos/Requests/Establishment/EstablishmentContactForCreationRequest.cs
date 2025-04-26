namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;

public class EstablishmentContactForCreationRequest
{
    public string EstablishmentId { get; set; }
    public string ContactPersonFirstName { get; set; }
    public string? ContactPersonMiddleName { get; set; }
    public string ContactPersonLastName { get; set; }
    public string ContactPhoneNumber { get; set; }
    public bool IsPrimary { get; set; } = true;
}
