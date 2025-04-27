using TestNest.Admin.Domain.Employees;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Responses;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.Application.Mappings;

public static class EntityToDtoMapper
{
    public static EmployeeRoleResponse ToEmployeeRoleResponse(this EmployeeRole role) => new()
    {
        Id = role.Id.Value.ToString(),
        RoleName = role.RoleName.Name
    };

    public static SocialMediaPlatformResponse ToSocialMediaPlatformResponse(this SocialMediaPlatform platform) => new()
    {
        Id = platform.SocialMediaId.Value.ToString(),
        Name = platform.SocialMediaName.Name,
        PlatformURL = platform.SocialMediaName.PlatformURL
    };

    public static EmployeeResponse ToEmployeeResponse(this Employee employee) => new()
    {
        EmployeeId = employee.EmployeeId.Value.ToString(),
        EmployeeNumber = employee.EmployeeNumber.EmployeeNo,
        FirstName = employee.EmployeeName.FirstName,
        MiddleName = employee.EmployeeName.MiddleName,
        LastName = employee.EmployeeName.LastName,
        Email = employee.EmployeeEmail.Email,
        StatusId = employee.EmployeeStatus.Id.ToString(),
        RoleId = employee.EmployeeRoleId.Value.ToString(),
        EstablishmentId = employee.EstablishmentId.Value.ToString()
    };

    public static EstablishmentResponse ToEstablishmentResponse(this Establishment establishment) => new()
    {
        EstablishmentId = establishment.Id.Value.ToString(),
        EstablishmentName = establishment.EstablishmentName.Name,
        EstablishmentEmail = establishment.EstablishmentEmail.Email,
        EstablishmentStatusId = establishment.EstablishmentStatus.Id,
        EstablishmentStatusName = establishment.EstablishmentStatus.Name
    };

    public static EstablishmentAddressResponse ToEstablishmentAddressResponse(this EstablishmentAddress establishmentAddress) => new()
    {
        EstablishmentAddressId = establishmentAddress.Id.Value.ToString(),
        EstablishmentId = establishmentAddress.EstablishmentId.Value.ToString(),
        AddressLine = establishmentAddress.Address.AddressLine,
        City = establishmentAddress.Address.City,
        Municipality = establishmentAddress.Address.Municipality,
        Province = establishmentAddress.Address.Province,
        Region = establishmentAddress.Address.Region,
        Country = establishmentAddress.Address.Country,
        Latitude = (double)establishmentAddress.Address.Latitude,
        Longitude = (double)establishmentAddress.Address.Longitude,
        IsPrimary = establishmentAddress.IsPrimary
    };

    public static EstablishmentPhoneResponse ToEstablishmentPhoneResponse(this EstablishmentPhone establishmentPhone) => new()
    {
        EstablishmentPhoneId = establishmentPhone.Id.Value.ToString(),
        EstablishmentId = establishmentPhone.EstablishmentId.Value.ToString(),
        PhoneNumber = establishmentPhone.EstablishmentPhoneNumber.PhoneNo,
        IsPrimary = establishmentPhone.IsPrimary
    };

    public static EstablishmentContactResponse ToEstablishmentContactResponse(this EstablishmentContact contact) => new()
    {
        EstablishmentContactId = contact.Id.Value.ToString(),
        EstablishmentId = contact.EstablishmentId.Value.ToString(),
        ContactFirstName = contact.ContactPerson.FirstName,
        ContactMiddleName = contact.ContactPerson.MiddleName,
        ContactLastName = contact.ContactPerson.LastName,
        ContactPhoneNumber = contact.ContactPhone.PhoneNo,
        IsPrimary = contact.IsPrimary
    };

    public static EstablishmentMemberResponse ToEstablishmentMemberResponse(this EstablishmentMember member) => new()
    {
        EstablishmentMemberId = member.Id.Value.ToString(),
        EstablishmentId = member.EstablishmentId.Value.ToString(),
        EmployeeId = member.EmployeeId.Value.ToString(),
        MemberTitle = member.MemberTitle.Title,
        MemberDescription = member.MemberDescription.Description,
        MemberTag = member.MemberTag.Tag
    };
}
