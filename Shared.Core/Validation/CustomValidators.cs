using CSharpFunctionalExtensions;
using Shared.Kernel.ErrorsShared;
using FluentValidation;
using FluentValidation.Results;

namespace Shared.Core.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement?> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement?> ruleBuilder
        , Func<TElement?, Result<TValueObject, Error>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            if (value == null)
                return;
            Result<TValueObject, Error> result = factoryMethod.Invoke(value);
            if (result.IsSuccess)
                return;

            // Используем context.PropertyPath вместо PropertyName
            var failure = new ValidationFailure(context.PropertyPath, result.Error.Message)
            {
                CustomState = result.Error // Сохраняем объект напрямую без JSON
            };

            context.AddFailure(failure);
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Error error)
    {
        return rule
            .WithMessage(error.Message)
            .WithState(_ => error);
    }
}
