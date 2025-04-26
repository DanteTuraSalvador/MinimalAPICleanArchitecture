using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.Application.Mappings;
public static class EntityToDtoMapper
{
    //public static EmployeeRoleResponse ToEmployeeRoleResponse(this EmployeeRole role) => new()
    //{
    //    Id = role.Id.Value.ToString(),
    //    RoleName = role.RoleName.Name
    //};

    public static EstablishmentResponse ToEstablishmentResponse(this Establishment establishment) => new()
    {
        EstablishmentId = establishment.Id.Value.ToString(),
        EstablishmentName = establishment.EstablishmentName.Name,
        EstablishmentEmail = establishment.EstablishmentEmail.Email,
        EstablishmentStatusId = establishment.EstablishmentStatus.Id,
        EstablishmentStatusName = establishment.EstablishmentStatus.Name
    };


}
