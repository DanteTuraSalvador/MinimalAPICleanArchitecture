namespace TestNest.Admin.SharedLibrary.Common.Options;

public abstract class Option<T>
{
    public abstract TResult GetValueOrDefault<TResult>(TResult defaultValue);

    public abstract Option<TResult> Map<TResult>(Func<T, TResult> map);

    public abstract Option<TResult> FlatMap<TResult>(Func<T, Option<TResult>> map);

    public abstract Option<T> IfSome(Action<T> action);

    public abstract TResult Fold<TResult>(TResult ifNone, Func<T, TResult> ifSome);

    public abstract TResult Match<TResult>(Func<TResult> ifNone, Func<T, TResult> ifSome);

    public abstract void Match(Action ifNone, Action<T> ifSome);

    public abstract Option<T> OrElse(Option<T> other);

    public abstract bool IsSome { get; }
    public abstract bool IsNone { get; }

    public abstract Task<Option<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map);

    public abstract Task<Option<TResult>> FlatMapAsync<TResult>(Func<T, Task<Option<TResult>>> map);

    public static Option<T> Some(T value) => new SomeImpl<T>(value);

    public static Option<T> None => NoneImpl<T>.Default;

    public static Option<T> From(T value) => value == null ? None : Some(value);
}

internal sealed class SomeImpl<T> : Option<T>
{
    private readonly T _value;

    public SomeImpl(T value) => _value = value;

    public override TResult GetValueOrDefault<TResult>(TResult defaultValue) => (TResult)(object)_value;

    public override Option<TResult> Map<TResult>(Func<T, TResult> map) => Option<TResult>.Some(map(_value));

    public override Option<TResult> FlatMap<TResult>(Func<T, Option<TResult>> map) => map(_value);

    public override Option<T> IfSome(Action<T> action)
    { action(_value); return this; }

    public override TResult Fold<TResult>(TResult ifNone, Func<T, TResult> ifSome) => ifSome(_value);

    public override TResult Match<TResult>(Func<TResult> ifNone, Func<T, TResult> ifSome) => ifSome(_value);

    public override void Match(Action ifNone, Action<T> ifSome) => ifSome(_value);

    public override Option<T> OrElse(Option<T> other) => this;

    public override bool IsSome => true;
    public override bool IsNone => false;

    public override async Task<Option<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map)
        => Option<TResult>.Some(await map(_value));

    public override async Task<Option<TResult>> FlatMapAsync<TResult>(Func<T, Task<Option<TResult>>> map)
        => await map(_value);
}

internal sealed class NoneImpl<T> : Option<T>
{
    internal static readonly NoneImpl<T> Default = new NoneImpl<T>();

    private NoneImpl()
    { }

    public override TResult GetValueOrDefault<TResult>(TResult defaultValue) => defaultValue;

    public override Option<TResult> Map<TResult>(Func<T, TResult> map) => Option<TResult>.None;

    public override Option<TResult> FlatMap<TResult>(Func<T, Option<TResult>> map) => Option<TResult>.None;

    public override Option<T> IfSome(Action<T> action) => this;

    public override TResult Fold<TResult>(TResult ifNone, Func<T, TResult> ifSome) => ifNone;

    public override TResult Match<TResult>(Func<TResult> ifNone, Func<T, TResult> ifSome) => ifNone();

    public override void Match(Action ifNone, Action<T> ifSome) => ifNone();

    public override Option<T> OrElse(Option<T> other) => other;

    public override bool IsSome => false;
    public override bool IsNone => true;

    public override Task<Option<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map)
        => Task.FromResult(Option<TResult>.None);

    public override Task<Option<TResult>> FlatMapAsync<TResult>(Func<T, Task<Option<TResult>>> map)
        => Task.FromResult(Option<TResult>.None);
}