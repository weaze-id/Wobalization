using FluentValidation.Results;
using Kern.Error;
using Shared.Dtos.Authentication;

namespace Wobalization.Api.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignInAsync(InLoginDto dto);
    public Task<ErrorBase?> SignOutAsync();
}