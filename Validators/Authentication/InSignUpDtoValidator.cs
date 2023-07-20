using FluentValidation;
using Wobalization.Dtos.Authentication;

namespace Wobalization.Validators.Authentication;

public class InSignUpDtoValidator : AbstractValidator<InSignUpDto>
{
    public InSignUpDtoValidator()
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
            .NotEmpty()
            .MinimumLength(8);
    }
}