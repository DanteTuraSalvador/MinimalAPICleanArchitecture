namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;

public record EstablishmentMemberForUpdateRequest(
    string EstablishmentId,
    string? MemberTitle,
    string? MemberDescription,
    string? MemberTag);
