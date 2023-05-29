using FluentValidation;
using Shared.Dtos.Language;

namespace Shared.Validators.Language;

public class InLanguageDtoValidator : AbstractValidator<InLanguageDto>
{
    public InLanguageDtoValidator()
    {
        RuleFor(e => e.Culture)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(10);
    }
}