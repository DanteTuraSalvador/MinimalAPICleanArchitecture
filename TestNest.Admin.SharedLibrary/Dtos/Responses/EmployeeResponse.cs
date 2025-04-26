namespace TestNest.Admin.SharedLibrary.Dtos.Requests;

public class EmployeeResponse
{
    public string EmployeeId { get; set; }
    public string EmployeeNumber { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string StatusId { get; set; }
    public string RoleId { get; set; }
    public string EstablishmentId { get; set; }
}