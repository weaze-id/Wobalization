using Kern.AspNetCore.Response.Extensions;
using Wobalization.Services;

namespace Wobalization.Middlewares;

public class IdentityMiddleware
{
    private readonly RequestDelegate _next;

    public IdentityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IdentityService identityService)
    {
        if (context.User.Identity!.IsAuthenticated)
        {
            var claims = context.User.Claims;
            var storeClaimsError = identityService.ParseClaims(claims);
            if (storeClaimsError != null)
            {
                await storeClaimsError.Response().ExecuteAsync(context);
                return;
            }
        }

        await _next(context);
    }
}