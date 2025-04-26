namespace TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;

public class EmployeePatchRequest
{
    public string? EmployeeNumber { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? EmployeeRoleId { get; set; }
    public string? EstablishmentId { get; set; }
    public int? EmployeeStatusId { get; set; }
}