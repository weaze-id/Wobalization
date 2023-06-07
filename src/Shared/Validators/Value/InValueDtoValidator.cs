using FluentValidation;
using Shared.Dtos.Value;

namespace Shared.Validators.Value;

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