namespace TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

public record EstablishmentMemberResponse(
    string EstablishmentMemberId,
    string EstablishmentId,
    string EmployeeId,
    string MemberTitle,
    string MemberDescription,
    string MemberTag);
