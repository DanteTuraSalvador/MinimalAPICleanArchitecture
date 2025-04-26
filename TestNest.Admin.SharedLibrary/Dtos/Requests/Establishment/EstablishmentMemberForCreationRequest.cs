namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;

public record EstablishmentMemberForCreationRequest(
    string EstablishmentId,
    string EmployeeId,
    string MemberTitle,
    string MemberDescription,
    string MemberTag);
