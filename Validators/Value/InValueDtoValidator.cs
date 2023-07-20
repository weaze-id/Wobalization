using FluentValidation;
using Wobalization.Dtos.Value;

namespace Wobalization.Validators.Value;

public class InValueDtoValidator : AbstractValidator<InValueDto>
{
    public InValueDtoValidator()
    {
        RuleFor(e => e.KeyId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(e => e.Value)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}