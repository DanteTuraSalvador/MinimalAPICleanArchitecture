namespace TestNest.Admin.SharedLibrary.Dtos.Paginations;

public class PaginationRequest
{
    public int? PageNumber { get; set; } = 1; // Make nullable
    public int? PageSize { get; set; } = 10;   // Make nullable
}
