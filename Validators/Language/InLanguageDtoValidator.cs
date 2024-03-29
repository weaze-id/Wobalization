using FluentValidation;
using Wobalization.Dtos.Language;

namespace Wobalization.Validators.Language;

public class InLanguageDtoValidator : AbstractValidator<InLanguageDto>
{
    public InLanguageDtoValidator()
    {
        RuleFor(e => e.Locale)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(10);
    }
}