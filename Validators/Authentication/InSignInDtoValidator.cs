using FluentValidation;
using Wobalization.Dtos.Authentication;

namespace Wobalization.Validators.Authentication;

public class InSignInDtoValidator : AbstractValidator<InSignInDto>
{
    public InSignInDtoValidator()
    {
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