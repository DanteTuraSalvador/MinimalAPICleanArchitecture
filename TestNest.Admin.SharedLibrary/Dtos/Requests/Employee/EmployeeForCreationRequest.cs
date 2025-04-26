namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;

public class EmployeeForCreationRequest
{
    public string EmployeeNumber { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public Guid EmployeeRoleId { get; set; }
    public Guid EstablishmentId { get; set; }
}