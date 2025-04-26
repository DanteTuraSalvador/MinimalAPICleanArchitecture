namespace TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

public class EstablishmentContactResponse
{
    public string EstablishmentContactId { get; set; }
    public string EstablishmentId { get; set; }
    public string ContactFirstName { get; set; }
    public string ContactMiddleName { get; set; }
    public string ContactLastName { get; set; }
    public string ContactPhoneNumber { get; set; }
    public bool IsPrimary { get; set; }
}
