using FluentValidation;
using Wobalization.Dtos.Key;

namespace Wobalization.Validators.Key;

public class InValueDtoValidator : AbstractValidator<InKeyValueDto>
{
    public InValueDtoValidator()
    {
        RuleFor(e => e.LanguageId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(e => e.Value)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}