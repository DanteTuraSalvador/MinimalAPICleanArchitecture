namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;

public class EstablishmentPatchRequest
{
    public string? EstablishmentName { get; set; }
    public string? EmailAddress { get; set; }
    public int? EstablishmentStatus { get; set; }
}