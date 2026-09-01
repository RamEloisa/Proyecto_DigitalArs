using DigitalArs.Application.DTOs;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DigitalArs.API.Filters;

/// Valida DTOs de request con FluentValidation y responde 400 con lista de errores.
public sealed class FluentValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null || argument is CancellationToken)
                continue;

            var argumentType = argument.GetType();
            if (argumentType.IsPrimitive || argumentType.IsEnum || argumentType == typeof(string))
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                continue;

            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                context.Result = ValidationErrorResponseFactory.FromFluent(result);
                return;
            }
        }

        await next();
    }
}

public static class ValidationErrorResponseFactory
{
    public static BadRequestObjectResult From(IReadOnlyList<ValidationErrorDto> errors) =>
        new(new ValidationProblemDto(StatusCodes.Status400BadRequest, errors));

    public static BadRequestObjectResult FromModelState(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(kvp => kvp.Value is { Errors.Count: > 0 })
            .SelectMany(kvp => kvp.Value!.Errors.Select(error =>
                new ValidationErrorDto(
                    string.IsNullOrWhiteSpace(kvp.Key) ? "payload" : kvp.Key,
                    string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Valor inválido."
                        : error.ErrorMessage)))
            .ToList();

        return From(errors);
    }

    public static BadRequestObjectResult FromFluent(ValidationResult result) =>
        From(result.Errors.Select(e => new ValidationErrorDto(e.PropertyName, e.ErrorMessage)).ToList());
}
