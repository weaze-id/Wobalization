using FluentValidation;
using Wobalization.Dtos.Key;

namespace Wobalization.Validators.Key;

public class InKeyDtoValidator : AbstractValidator<InKeyDto>
{
    public InKeyDtoValidator()
    {
        RuleFor(e => e.Key)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}