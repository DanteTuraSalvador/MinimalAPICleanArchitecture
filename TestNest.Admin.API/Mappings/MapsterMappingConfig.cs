using Mapster;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.API.Mappings;

public class MapsterMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        _ = config.NewConfig<SocialMediaPlatform, SocialMediaPlatformResponse>()
            .Map(dest => dest.Id, src => src.Id.Value.ToString())
            .Map(dest => dest.Name, src => src.SocialMediaName.Name)
            .Map(dest => dest.PlatformURL, src => src.SocialMediaName.PlatformURL);

        _ = config.NewConfig<EmployeeRole, EmployeeRoleResponse>()
            .Map(dest => dest.Id, src => src.Id.Value.ToString())
            .Map(dest => dest.RoleName, src => src.RoleName.Name);

        _ = config.NewConfig<Establishment, EstablishmentResponse>()
            .Map(dest => dest.EstablishmentId, src => src.Id.Value.ToString())
            .Map(dest => dest.EstablishmentName, src => src.EstablishmentName.Name)
            .Map(dest => dest.EstablishmentEmail, src => src.EstablishmentEmail.Email)
            .Map(dest => dest.EstablishmentStatusId, src => src.EstablishmentStatus.Id)
            .Map(dest => dest.EstablishmentStatusName, src => src.EstablishmentStatus.Name);

        _ = config.NewConfig<Employee, EmployeeResponse>()
            .Map(dest => dest.EmployeeId, src => src.EmployeeId.ToString())
            .Map(dest => dest.EmployeeNumber, src => src.EmployeeNumber.EmployeeNo)
            .Map(dest => dest.FirstName, src => src.EmployeeName.FirstName)
            .Map(dest => dest.MiddleName, src => src.EmployeeName.MiddleName)
            .Map(dest => dest.LastName, src => src.EmployeeName.LastName)
            .Map(dest => dest.Email, src => src.EmployeeEmail.Email)
            .Map(dest => dest.StatusId, src => src.EmployeeStatus.Id.ToString())
            .Map(dest => dest.RoleId, src => src.EmployeeRoleId.ToString())
            .Map(dest => dest.EstablishmentId, src => src.EstablishmentId.ToString());

        _ = config.NewConfig<EstablishmentAddress, EstablishmentAddressResponse>()
            .Map(dest => dest.EstablishmentAddressId, src => src.Id.Value.ToString())
            .Map(dest => dest.EstablishmentId, src => src.EstablishmentId.Value.ToString())
            .Map(dest => dest.AddressLine, src => src.Address.AddressLine)
            .Map(dest => dest.City, src => src.Address.City)
            .Map(dest => dest.Municipality, src => src.Address.Municipality)
            .Map(dest => dest.Province, src => src.Address.Province)
            .Map(dest => dest.Region, src => src.Address.Region)
            .Map(dest => dest.Country, src => src.Address.Country)
            .Map(dest => dest.Latitude, src => src.Address.Latitude)
            .Map(dest => dest.Longitude, src => src.Address.Longitude)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary);

        _ = config.NewConfig<EstablishmentContact, EstablishmentContactResponse>()
            .Map(dest => dest.EstablishmentContactId, src => src.Id.Value.ToString())
            .Map(dest => dest.EstablishmentId, src => src.EstablishmentId.Value.ToString())
            .Map(dest => dest.ContactFirstName, src => src.ContactPerson.FirstName)
            .Map(dest => dest.ContactMiddleName, src => src.ContactPerson.MiddleName)
            .Map(dest => dest.ContactLastName, src => src.ContactPerson.LastName)
            .Map(dest => dest.ContactPhoneNumber, src => src.ContactPhone.PhoneNo)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary);

        _ = config.NewConfig<EstablishmentPhone, EstablishmentPhoneResponse>()
            .Map(dest => dest.EstablishmentPhoneId, src => src.Id.Value.ToString())
            .Map(dest => dest.EstablishmentId, src => src.EstablishmentId.Value.ToString())
            .Map(dest => dest.PhoneNumber, src => src.EstablishmentPhoneNumber.PhoneNo)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary);
    }
}
