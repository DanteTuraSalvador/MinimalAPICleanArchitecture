using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.Application.Exceptions;

public class SqlServerExceptionHandlerFactory() : IDatabaseExceptionHandlerFactory
{
    public Result HandleDbUpdateException(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sqlException)
        {
            if (sqlException.Number is 2601 or 2627) // Unique constraint
            {
                return Result.Failure(ErrorType.Conflict, new Error("DuplicateError", "A duplicate value exists."));
            }
            if (sqlException.Number == 547) // Foreign key violation
            {
                return Result.Failure(ErrorType.Conflict, new Error("ForeignKeyError", "Foreign key constraint violation."));
            }
        }
        return Result.Failure(ErrorType.Internal, new Error("DatabaseError", "A database error occurred."));
    }
}
