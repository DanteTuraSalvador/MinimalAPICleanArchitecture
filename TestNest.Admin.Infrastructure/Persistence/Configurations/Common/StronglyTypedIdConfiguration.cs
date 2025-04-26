using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations.Common;

public static class StronglyTypedIdConfiguration
{
    public static PropertyBuilder<TId> ConfigureStronglyTypedId<TId>(this PropertyBuilder<TId> builder)
        where TId : StronglyTypedId<TId>
    {
        builder.HasConversion(
            id => id.Value,
            value => CreateStronglyTypedId<TId>(value)
        )
        .ValueGeneratedNever();

        return builder;
    }

    private static TId CreateStronglyTypedId<TId>(object value) where TId : StronglyTypedId<TId>
    {
        var constructor = typeof(TId)
            .GetConstructor(new[] { value.GetType() });

        if (constructor == null)
        {
            throw new InvalidOperationException($"The type {typeof(TId).Name} does not have a constructor that accepts a parameter of type {value.GetType().Name}.");
        }

        return (TId)constructor.Invoke(new[] { value });
    }
}