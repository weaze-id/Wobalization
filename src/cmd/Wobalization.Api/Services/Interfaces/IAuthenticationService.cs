using FluentValidation.Results;
using Kern.Error;
using Shared.Dtos.Authentication;
using Shared.Dtos.User;

namespace Wobalization.Api.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<ErrorBase?> GetStatusAsync();
    public Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignInAsync(InLoginDto dto);
    public Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignUpAsync(InUserAddDto dto);
    public Task<ErrorBase?> SignOutAsync();
}