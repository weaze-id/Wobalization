using FluentValidation;
using Shared.Dtos.App;

namespace Shared.Validators.App;

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