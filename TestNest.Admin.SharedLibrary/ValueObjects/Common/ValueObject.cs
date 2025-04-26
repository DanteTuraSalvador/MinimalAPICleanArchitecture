namespace TestNest.Admin.SharedLibrary.ValueObjects.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public static bool operator ==(ValueObject? a, ValueObject? b)
        => a is null ? b is null
            : a.Equals(b);

    public static bool operator !=(ValueObject? a, ValueObject? b)
        => !(a == b);

    public virtual bool Equals(ValueObject? other)
        => ReferenceEquals(this, other) || (other is not null && GetType() == other.GetType() && ValuesAreEqual(other));

    public override bool Equals(object? obj)
        => obj is ValueObject valueObject && Equals(valueObject);

    public override int GetHashCode()
        => GetAtomicValues().Aggregate(17, (hash, value) => HashCode.Combine(hash, value?.GetHashCode() ?? 0));

    protected abstract IEnumerable<object?> GetAtomicValues();

    private bool ValuesAreEqual(ValueObject other)
        => GetAtomicValues().SequenceEqual(other.GetAtomicValues());
}