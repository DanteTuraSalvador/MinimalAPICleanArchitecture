namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;

public class EmployeeForUpdateRequest
{
    public string EmployeeNumber { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public Guid EmployeeRoleId { get; set; } // Ensure this is a GUID
    public Guid EstablishmentId { get; set; } // Ensure this is a GUID
    public int EmployeeStatusId { get; set; }
}