using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.Infrastructure.Persistence.Common;
using TestNest.Admin.Infrastructure.Persistence.Repositories.Common;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Infrastructure.Persistence.Repositories;

public class SocialMediaPlatformRepository(
    ApplicationDbContext socialMediaPlatformDbContext)
    : GenericRepository<SocialMediaPlatform, SocialMediaId>(socialMediaPlatformDbContext), ISocialMediaPlatformRepository
{
    private readonly ApplicationDbContext _SocialMediaPlatformDbContext = socialMediaPlatformDbContext;

    public override async Task<Result<IEnumerable<SocialMediaPlatform>>> GetAllAsync()
    {
        try
        {
            List<SocialMediaPlatform> socialMediaPlatforms = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
                .Include(x => x.SocialMediaName)
                .ToListAsync();

            return Result<IEnumerable<SocialMediaPlatform>>.Success(socialMediaPlatforms);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<SocialMediaPlatform>>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<SocialMediaPlatform>> AddAsync(SocialMediaPlatform socialMediaPlatform)
    {
        try
        {
            _ = await _SocialMediaPlatformDbContext.SocialMediaPlatforms.AddAsync(socialMediaPlatform);
            return Result<SocialMediaPlatform>.Success(socialMediaPlatform);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            return Result<SocialMediaPlatform>.Failure(ErrorType.Conflict,
                new Error("DuplicateName", "Platform name already exists"));
        }
        catch (Exception ex)
        {
            return Result<SocialMediaPlatform>.Failure(ErrorType.Internal,
                new Error("AddFailed", ex.Message));
        }
    }

    public override async Task<Result<SocialMediaPlatform>> UpdateAsync(SocialMediaPlatform socialMediaPlatform)
    {
        try
        {
            bool exists = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
                .AsNoTracking()
                .AnyAsync(x => x.Id == socialMediaPlatform.Id);

            if (!exists)
            {
                var exception = SocialMediaPlatformException.NotFound();
                return Result<SocialMediaPlatform>.Failure(ErrorType.NotFound,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }

            _ = _SocialMediaPlatformDbContext.Update(socialMediaPlatform);

            return Result<SocialMediaPlatform>.Success(socialMediaPlatform);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            return Result<SocialMediaPlatform>.Failure(ErrorType.Conflict,
                new Error("DuplicateName", "A platform with that name already exists."));
        }
        catch (Exception ex)
        {
            return Result<SocialMediaPlatform>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result> DeleteAsync(SocialMediaId socialMediaId)
    {
        try
        {
            SocialMediaPlatform? socialMediaPlatform = await _SocialMediaPlatformDbContext.SocialMediaPlatforms.FindAsync(socialMediaId);
            if (socialMediaPlatform == null)
            {
                return Result.Failure(ErrorType.NotFound,
                    new Error(SocialMediaPlatformException.NotFound().Code.ToString(),
                        SocialMediaPlatformException.NotFound().Message.ToString()));
            }
            _ = _SocialMediaPlatformDbContext.SocialMediaPlatforms.Remove(socialMediaPlatform);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<SocialMediaPlatform>> GetByIdAsync(SocialMediaId socialMediaId)
    {
        try
        {
            SocialMediaPlatform? socialMediaPlatform = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
                .FirstOrDefaultAsync(x => x.Id == socialMediaId);

            if (socialMediaPlatform == null)
            {
                var exception = SocialMediaPlatformException.NotFound();
                return Result<SocialMediaPlatform>.Failure(
                    ErrorType.NotFound,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }

            return Result<SocialMediaPlatform>.Success(socialMediaPlatform);
        }
        catch (Exception ex)
        {
            return Result<SocialMediaPlatform>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<SocialMediaPlatform>> GetSocialMediaPlatformByNameAsync(string socialMediaAccountName)
    {
        try
        {
            SocialMediaPlatform? socialMediaPlatform = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
                .Include(x => x.SocialMediaName)
                .FirstOrDefaultAsync(x => x.SocialMediaName.Name == socialMediaAccountName);

            if (socialMediaPlatform == null)
            {
                var exception = SocialMediaPlatformException.NotFound();
                return Result<SocialMediaPlatform>.Failure(
                    ErrorType.NotFound,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }

            return Result<SocialMediaPlatform>.Success(socialMediaPlatform);
        }
        catch (Exception ex)
        {
            return Result<SocialMediaPlatform>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public async Task DetachAsync(SocialMediaPlatform platform)
    {
        _SocialMediaPlatformDbContext.Entry(platform).State = EntityState.Detached;
        await Task.CompletedTask;
    }

    public async Task<Result<IEnumerable<SocialMediaPlatform>>> ListAsync(ISpecification<SocialMediaPlatform> spec)
    {
        try
        {
            IQueryable<SocialMediaPlatform> query = _SocialMediaPlatformDbContext.SocialMediaPlatforms
                .Include(x => x.SocialMediaName)
                .AsQueryable();

            var platformSpec = (BaseSpecification<SocialMediaPlatform>)spec;
            query = SpecificationEvaluator<SocialMediaPlatform>.GetQuery(query, platformSpec);

            List<SocialMediaPlatform> platforms = await query.ToListAsync();
            return Result<IEnumerable<SocialMediaPlatform>>.Success(platforms);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<SocialMediaPlatform>>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<int>> CountAsync(ISpecification<SocialMediaPlatform> spec)
    {
        try
        {
            IQueryable<SocialMediaPlatform> query = SpecificationEvaluator<SocialMediaPlatform>.GetQuery(
                _SocialMediaPlatformDbContext.SocialMediaPlatforms,
                (BaseSpecification<SocialMediaPlatform>)spec
            );
            int count = await query.CountAsync();
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }
}

//public class SocialMediaPlatformRepository(
//    ApplicationDbContext socialMediaPlatformDbContext)
//    : ISocialMediaPlatformRepository
//{
//    private readonly ApplicationDbContext _SocialMediaPlatformDbContext = socialMediaPlatformDbContext;

//    public async Task<Result<IEnumerable<SocialMediaPlatform>>> GetAllAsync()
//    {
//        try
//        {
//            var socialMediaPlatforms = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
//                .Include(x => x.SocialMediaName)
//                .ToListAsync();

//            return Result<IEnumerable<SocialMediaPlatform>>.Success(socialMediaPlatforms);
//        }
//        catch (Exception ex)
//        {
//            return Result<IEnumerable<SocialMediaPlatform>>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> AddAsync(SocialMediaPlatform socialMediaPlatform)
//    {
//        try
//        {
//            await _SocialMediaPlatformDbContext.SocialMediaPlatforms.AddAsync(socialMediaPlatform);
//            return Result.Success();
//        }
//        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
//        {
//            return Result.Failure(ErrorType.Conflict,
//                new Error("DuplicateName", "Platform name already exists"));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(ErrorType.Internal,
//                new Error("AddFailed", ex.Message));
//        }
//    }

//    public async Task<Result> UpdateAsync(SocialMediaPlatform socialMediaPlatform)
//    {
//        try
//        {
//            var exists = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
//                .AsNoTracking()
//                .AnyAsync(x => x.Id == socialMediaPlatform.Id);

//            if (!exists)
//            {
//                var exception = SocialMediaPlatformException.NotFound();
//                return Result.Failure(ErrorType.NotFound,
//                    new Error(exception.Code.ToString(), exception.Message.ToString()));
//            }

//            _SocialMediaPlatformDbContext.Update(socialMediaPlatform);

//            return Result.Success();
//        }
//        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
//        {
//            return Result.Failure(ErrorType.Conflict,
//                new Error("DuplicateName", "A platform with that name already exists."));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> DeleteAsync(SocialMediaId socialMediaId)
//    {
//        try
//        {
//            var rowsDeleted = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
//                .Where(p => p.Id == socialMediaId)
//                .ExecuteDeleteAsync();

//            return rowsDeleted > 0
//                ? Result.Success()
//                : Result.Failure(ErrorType.NotFound,
//                    new Error(SocialMediaPlatformException.NotFound().Code.ToString(),
//                        SocialMediaPlatformException.NotFound().Message.ToString()));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<SocialMediaPlatform>> GetByIdAsync(SocialMediaId socialMediaId)
//    {
//        try
//        {
//            var socialMediaPlatform = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
//                .FirstOrDefaultAsync(x => x.Id == socialMediaId);

//            if (socialMediaPlatform == null)
//            {
//                var exception = SocialMediaPlatformException.NotFound();
//                return Result<SocialMediaPlatform>.Failure(
//                    ErrorType.NotFound,
//                    new Error(exception.Code.ToString(), exception.Message.ToString()));
//            }

//            return Result<SocialMediaPlatform>.Success(socialMediaPlatform);
//        }
//        catch (Exception ex)
//        {
//            return Result<SocialMediaPlatform>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<SocialMediaPlatform>> GetSocialMediaPlatformByNameAsync(string socialMediaAccountName)
//    {
//        try
//        {
//            var socialMediaPlatform = await _SocialMediaPlatformDbContext.SocialMediaPlatforms
//                .FirstOrDefaultAsync(x => x.SocialMediaName.Name == socialMediaAccountName);

//            if (socialMediaPlatform == null)
//            {
//                var exception = SocialMediaPlatformException.NotFound();
//                return Result<SocialMediaPlatform>.Failure(
//                    ErrorType.NotFound,
//                    new Error(exception.Code.ToString(), exception.Message.ToString()));
//            }

//            return Result<SocialMediaPlatform>.Success(socialMediaPlatform);
//        }
//        catch (Exception ex)
//        {
//            return Result<SocialMediaPlatform>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task DetachAsync(SocialMediaPlatform platform)
//    {
//        _SocialMediaPlatformDbContext.Entry(platform).State = EntityState.Detached;
//        await Task.CompletedTask;
//    }

//    public async Task<Result<IEnumerable<SocialMediaPlatform>>> ListAsync(ISpecification<SocialMediaPlatform> spec)
//    {
//        try
//        {
//            var query = _SocialMediaPlatformDbContext.SocialMediaPlatforms
//                .Include(x => x.SocialMediaName)
//                .AsQueryable();

//            var platformSpec = (BaseSpecification<SocialMediaPlatform>)spec;
//            query = SpecificationEvaluator<SocialMediaPlatform>.GetQuery(query, platformSpec);

//            var platforms = await query.ToListAsync();
//            return Result<IEnumerable<SocialMediaPlatform>>.Success(platforms);
//        }
//        catch (Exception ex)
//        {
//            return Result<IEnumerable<SocialMediaPlatform>>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<int>> CountAsync(ISpecification<SocialMediaPlatform> spec)
//    {
//        try
//        {
//            var query = SpecificationEvaluator<SocialMediaPlatform>.GetQuery(
//                _SocialMediaPlatformDbContext.SocialMediaPlatforms,
//                (BaseSpecification<SocialMediaPlatform>)spec
//            );
//            var count = await query.CountAsync();
//            return Result<int>.Success(count);
//        }
//        catch (Exception ex)
//        {
//            return Result<int>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }
//}
