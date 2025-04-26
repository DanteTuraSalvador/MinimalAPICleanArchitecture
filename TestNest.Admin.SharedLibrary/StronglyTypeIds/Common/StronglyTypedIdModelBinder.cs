using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

public class StronglyTypedIdModelBinder<T> : IModelBinder where T : StronglyTypedId<T>
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw StronglyTypedIdException.NullBindingContext();

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
            throw StronglyTypedIdException.MissingValue();

        var value = valueProviderResult.FirstValue;

        if (Guid.TryParse(value, out var guid))
        {
            var idInstance = Activator.CreateInstance(typeof(T), guid) as T;
            if (idInstance is null)
                throw StronglyTypedIdException.NullInstanceCreation();

            bindingContext.Result = ModelBindingResult.Success(idInstance);
            return Task.CompletedTask;
        }

        throw StronglyTypedIdException.InvalidFormat();
    }
}