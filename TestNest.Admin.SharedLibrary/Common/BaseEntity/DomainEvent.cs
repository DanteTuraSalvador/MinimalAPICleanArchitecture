using TestNest.Admin.SharedLibrary.Common.BaseEntity;

namespace TestNest.Domain.Common.BaseEntity;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
    }

    protected DomainEvent(Guid id, DateTime occurredOnUtc)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid Id { get; }

    public DateTime OccurredOnUtc { get; }

    DateTime IDomainEvent.OccurredOn => OccurredOnUtc;
}