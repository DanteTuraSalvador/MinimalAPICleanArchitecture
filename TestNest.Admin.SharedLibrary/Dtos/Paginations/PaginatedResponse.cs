namespace TestNest.Admin.SharedLibrary.Dtos.Paginations;

public class PaginatedResponse<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T> Data { get; set; }
    public PaginatedLinks Links { get; set; } // Changed the type to PaginatedLinks
}
