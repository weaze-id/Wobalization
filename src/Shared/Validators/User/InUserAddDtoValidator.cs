using FluentValidation;
using Shared.Dtos.User;

namespace Shared.Validators.User;

public class InUserAddDtoValidator : AbstractValidator<InUserAddDto>
{
    public InUserAddDtoValidator()
    {
        RuleFor(e => e.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(e => e.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(e => e.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}