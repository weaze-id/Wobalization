using FluentValidation.Results;
using Kern.Error;
using Shared.Dtos.Authentication;

namespace Wobalization.Api.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<ErrorBase?> GetStatusAsync();
    public Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignInAsync(InSignInDto dto);
    public Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignUpAsync(InSignUpDto dto);
    public Task<ErrorBase?> SignOutAsync();
}