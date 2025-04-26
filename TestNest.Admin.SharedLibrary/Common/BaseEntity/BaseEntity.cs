using System.ComponentModel.DataAnnotations;
using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.Common.BaseEntity;

public abstract class BaseEntity<TId> where TId : StronglyTypedId<TId>
{
    [Required]
    public TId Id { get; protected set; } = default!;

    protected BaseEntity()
    { }

    protected BaseEntity(TId id) => Id = id ?? throw new ArgumentNullException(nameof(id));

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TId> other)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Id.Equals(other.Id) && GetType() == other.GetType();
    }

    public bool Equals(BaseEntity<TId>? other) => Equals((object?)other);

    public override int GetHashCode() => HashCode.Combine(Id, GetType());

    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
        => EqualOperator(left, right);

    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
        => NotEqualOperator(left, right);

    protected static bool EqualOperator(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        if (left is null ^ right is null)
            return false;
        return left is null || left.Equals(right);
    }

    protected static bool NotEqualOperator(BaseEntity<TId>? left, BaseEntity<TId>? right)
        => !EqualOperator(left, right);
}
