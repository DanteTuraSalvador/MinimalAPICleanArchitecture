namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;

public record EstablishmentMemberPatchRequest
{
    public string? EstablishmentId { get; set; }
    public string? EmployeeId { get; set; }
    public string? MemberTitle { get; set; }
    public string? MemberDescription { get; set; }
    public string? MemberTag { get; set; }
}
