namespace TestNest.Admin.SharedLibrary.Common.BaseEntity;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}