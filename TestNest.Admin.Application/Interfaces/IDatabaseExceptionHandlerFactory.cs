using Microsoft.EntityFrameworkCore;
using TestNest.Admin.SharedLibrary.Common.Results;

namespace TestNest.Admin.Application.Interfaces;

public interface IDatabaseExceptionHandlerFactory
{
    Result HandleDbUpdateException(DbUpdateException ex);
}