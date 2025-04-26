using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Common;
using TestNest.Admin.Infrastructure.Persistence.Repositories.Common;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Infrastructure.Persistence.Repositories;


public class EstablishmentContactRepository(
    ApplicationDbContext establishmentContactDbContext)
    : GenericRepository<EstablishmentContact, EstablishmentContactId>(establishmentContactDbContext), IEstablishmentContactRepository
{
    private readonly ApplicationDbContext _establishmentContactDbContext = establishmentContactDbContext;

    public override async Task<Result<EstablishmentContact>> GetByIdAsync(EstablishmentContactId id)
    {
        try
        {
            EstablishmentContact? contactResult = await _establishmentContactDbContext.EstablishmentContacts
                .Include(x => x.ContactPerson)
                .Include(x => x.ContactPhone)
                .FirstOrDefaultAsync(ec => ec.Id == id.Value);
            if (contactResult == null)
            {
                return Result<EstablishmentContact>.Failure(
                    ErrorType.NotFound,
                    new Error("NotFound", $"EstablishmentContact with ID '{id}' not found."));
            }
            return Result<EstablishmentContact>.Success(contactResult);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentContact>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<IEnumerable<EstablishmentContact>>> GetAllAsync()
    {
        try
        {
            List<EstablishmentContact> contacts = await _establishmentContactDbContext.EstablishmentContacts
                .Include(x => x.ContactPerson)
                .Include(x => x.ContactPhone)
                .ToListAsync();
            return Result<IEnumerable<EstablishmentContact>>.Success(contacts);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EstablishmentContact>>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<EstablishmentContact>> AddAsync(EstablishmentContact entity)
    {
        try
        {
            _ = await _establishmentContactDbContext.EstablishmentContacts.AddAsync(entity);
            return Result<EstablishmentContact>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentContact>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<EstablishmentContact>> UpdateAsync(EstablishmentContact entity)
    {
        try
        {
            _ = _establishmentContactDbContext.EstablishmentContacts.Update(entity);
            return Result<EstablishmentContact>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentContact>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result> DeleteAsync(EstablishmentContactId id)
    {
        try
        {
            EstablishmentContact? entityToDelete = await _establishmentContactDbContext.EstablishmentContacts
                .FindAsync(id.Value);
            if (entityToDelete != null)
            {
                _ = _establishmentContactDbContext.EstablishmentContacts.Remove(entityToDelete);
                return Result.Success();
            }
            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"EstablishmentContact with ID '{id}' not found."));
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> ExistsAsync(EstablishmentContactId id)
        => await _establishmentContactDbContext.EstablishmentContacts.AnyAsync(e => e.Id == id);

    public Task DetachAsync(EstablishmentContact establishmentContact)
    {
        _establishmentContactDbContext.Entry(establishmentContact).State = EntityState.Detached;
        return Task.CompletedTask;
    }

    public async Task<Result<IEnumerable<EstablishmentContact>>> ListAsync(ISpecification<EstablishmentContact> spec)
    {
        try
        {
            IQueryable<EstablishmentContact> query = _establishmentContactDbContext.EstablishmentContacts
                .Include(x => x.ContactPerson)
                .Include(x => x.ContactPhone)
                .AsQueryable();

            var establishmentContactSpec = (BaseSpecification<EstablishmentContact>)spec;
            query = SpecificationEvaluator<EstablishmentContact>.GetQuery(query, establishmentContactSpec);

            List<EstablishmentContact> establishmentContact = await query.ToListAsync();
            return Result<IEnumerable<EstablishmentContact>>.Success(establishmentContact);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EstablishmentContact>>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentContact> spec)
    {
        try
        {
            IQueryable<EstablishmentContact> query = SpecificationEvaluator<EstablishmentContact>
                .GetQuery(_establishmentContactDbContext.EstablishmentContacts, (BaseSpecification<EstablishmentContact>)spec);
            int count = await query.CountAsync();
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result> SetNonPrimaryForEstablishmentContanctAsync(
        EstablishmentId establishmentId,
        EstablishmentContactId excludeEstablishmentContactId)
    {
        try
        {
            int affectedRows = await _establishmentContactDbContext.EstablishmentContacts
                .Where(ec => ec.EstablishmentId == establishmentId && ec.IsPrimary && ec.Id != excludeEstablishmentContactId)
                .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.IsPrimary, false));
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> ContactExistsWithSameDetailsInEstablishment(
        EstablishmentContactId excludedId,
        PersonName contactPerson,
        PhoneNumber contactPhoneNumber,
        EstablishmentId establishmentId) => await _establishmentContactDbContext.EstablishmentContacts
            .AnyAsync(ec => ec.EstablishmentId == establishmentId &&
                             ec.ContactPerson.FirstName == contactPerson.FirstName &&
                             ec.ContactPerson.MiddleName == contactPerson.MiddleName &&
                             ec.ContactPerson.LastName == contactPerson.LastName &&
                             ec.ContactPhone.PhoneNo == contactPhoneNumber.PhoneNo &&
                             ec.Id != excludedId);
}

//public class EstablishmentContactRepository(
//    ApplicationDbContext establishmentContactDbContext,
//    ILogger<EstablishmentAddressRepository> logger) : IEstablishmentContactRepository
//{
//    private readonly ApplicationDbContext _establishmentAddressDbContext = establishmentContactDbContext;
//    private readonly ILogger<EstablishmentAddressRepository> _logger = logger;

//    public async Task<Result<EstablishmentContact>> GetByIdAsync(
//        EstablishmentContactId establishmentContactId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            EstablishmentContact? contactResult = await _establishmentAddressDbContext.EstablishmentContacts
//                .Include(x => x.ContactPerson)
//                .Include(x => x.ContactPhone)
//                .FirstOrDefaultAsync(ec => ec.Id == establishmentContactId, cancellationToken);

//            if (contactResult == null)
//            {
//                return Result<EstablishmentContact>.Failure(
//                    ErrorType.NotFound,
//                     new Error("NotFound", $"EstablishmentAddress with ID '{establishmentContactId}' not found."));
//            }

//            return Result<EstablishmentContact>.Success(contactResult);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentContact>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<EstablishmentContact>> AddAsync(
//        EstablishmentContact establishmentContact,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _ = await _establishmentAddressDbContext.EstablishmentContacts
//                .AddAsync(establishmentContact, cancellationToken);

//            return Result<EstablishmentContact>.Success(establishmentContact);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentContact>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<EstablishmentContact>> UpdateAsync(
//        EstablishmentContact establishmentContact,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            //_establishmentAddressDbContext.Entry(establishmentContact).State = EntityState.Modified;
//            _ = _establishmentAddressDbContext.EstablishmentContacts.Update(establishmentContact);
//            return Result<EstablishmentContact>.Success(establishmentContact);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentContact>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> DeleteAsync(
//        EstablishmentContactId id,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            EstablishmentContact? contactToDelete = await _establishmentAddressDbContext.EstablishmentContacts
//                .FindAsync([id], cancellationToken);
//            if (contactToDelete != null)
//            {
//                _ = _establishmentAddressDbContext.EstablishmentContacts.Remove(contactToDelete);
//                return Result.Success();
//            }
//            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"EstablishmentContact with ID '{id}' not found."));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> ExistsAsync(EstablishmentContactId id, CancellationToken cancellationToken = default)
//        => await _establishmentAddressDbContext.EstablishmentContacts.AnyAsync(e => e.Id == id, cancellationToken);

//    public Task DetachAsync(EstablishmentContact establishmentContact)
//    {
//        _establishmentAddressDbContext.Entry(establishmentContact).State = EntityState.Detached;
//        return Task.CompletedTask;
//    }

//    public async Task<Result<IEnumerable<EstablishmentContact>>> ListAsync(ISpecification<EstablishmentContact> spec)
//    {
//        try
//        {
//            IQueryable<EstablishmentContact> query = _establishmentAddressDbContext.EstablishmentContacts
//                .Include(x => x.ContactPerson)
//                .Include(x => x.ContactPhone)
//                .AsQueryable();

//            var establishmentContactSpec = (BaseSpecification<EstablishmentContact>)spec;
//            query = SpecificationEvaluator<EstablishmentContact>.GetQuery(query, establishmentContactSpec);

//            List<EstablishmentContact> establishmentContact = await query.ToListAsync();
//            return Result<IEnumerable<EstablishmentContact>>.Success(establishmentContact);
//        }
//        catch (Exception ex)
//        {
//            return Result<IEnumerable<EstablishmentContact>>.Failure(ErrorType.Database, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentContact> spec)
//    {
//        try
//        {
//            IQueryable<EstablishmentContact> query = SpecificationEvaluator<EstablishmentContact>
//                .GetQuery(_establishmentAddressDbContext.EstablishmentContacts, (BaseSpecification<EstablishmentContact>)spec);
//            int count = await query.CountAsync();
//            return Result<int>.Success(count);
//        }
//        catch (Exception ex)
//        {
//            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> SetNonPrimaryForEstablishmentContanctAsync(
//        EstablishmentId establishmentId,
//        EstablishmentContactId excludeEstablishmentContactId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            int affectedRows = await _establishmentAddressDbContext.EstablishmentContacts
//                .Where(ea => ea.EstablishmentId == establishmentId && ea.IsPrimary && ea.Id != excludeEstablishmentContactId)
//                .ExecuteUpdateAsync(setters => setters
//                .SetProperty(b => b.IsPrimary, false),
//                cancellationToken);

//            return Result.Success();
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> ContactExistsWithSameDetailsInEstablishment(
//        EstablishmentContactId excludedId,
//        PersonName contactPerson,
//        PhoneNumber contactPhoneNumber,
//        EstablishmentId establishmentId) => await _establishmentAddressDbContext.EstablishmentContacts
//            .AnyAsync(ec => ec.EstablishmentId == establishmentId &&
//                           ec.ContactPerson.FirstName == contactPerson.FirstName &&
//                           ec.ContactPerson.MiddleName == contactPerson.MiddleName &&
//                           ec.ContactPerson.LastName == contactPerson.LastName &&
//                           ec.ContactPhone.PhoneNo == contactPhoneNumber.PhoneNo &&
//                           ec.Id != excludedId);
//}
