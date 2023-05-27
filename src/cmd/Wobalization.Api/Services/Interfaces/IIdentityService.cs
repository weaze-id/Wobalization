using Kern.Error;
using Wobalization.Api.Models;

namespace Wobalization.Api.Services.Interfaces;

public interface IIdentityService
{
    public (IdentityModel?, ErrorBase?) Get();
    public Task<string> SignInAsync(IdentityModel identity);
    public Task SignOutAsync();
}