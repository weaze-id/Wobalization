using FluentValidation;
using Shared.Dtos.Authentication;

namespace Shared.Validators.Authentication;

public class InLoginDtoValidator : AbstractValidator<InLoginDto>
{
    public InLoginDtoValidator()
    {
        RuleFor(e => e.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(e => e.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}