using FluentValidation;
using Shared.Dtos.Key;

namespace Shared.Validators.Key;

public class InKeyDtoValidator : AbstractValidator<InKeyDto>
{
    public InKeyDtoValidator()
    {
        RuleFor(e => e.Key)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}