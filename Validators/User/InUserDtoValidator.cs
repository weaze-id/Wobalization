using FluentValidation;
using Wobalization.Dtos.User;

namespace Wobalization.Validators.User;

public class InUserDtoValidator : AbstractValidator<InUserDto>
{
    public InUserDtoValidator()
    {
        RuleFor(e => e.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(e => e.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);
    }
}