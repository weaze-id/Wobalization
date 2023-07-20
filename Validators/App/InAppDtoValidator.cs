using FluentValidation;
using Wobalization.Dtos.App;

namespace Wobalization.Validators.App;

public class InAppDtoValidator : AbstractValidator<InAppDto>
{
    public InAppDtoValidator()
    {
        RuleFor(e => e.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);
    }
}