using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response;
using Kern.AspNetCore.Response.Extensions;
using Wobalization.Dtos.Authentication;
using Wobalization.Services;

namespace Wobalization.Endpoints;

public class AuthenticationEndpoint : IEndpoints
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/authentication")
            .WithTags("Authentication");

        group
            .MapGet("/status", GetStatusAsync)
            .WithName("Get app initialized status");

        group
            .MapPost("/sign-in", SignInAsync)
            .WithName("Sign in")
            .Produces<OutTokenDto>();

        group
            .MapPost("/sign-up", SignUpAsync)
            .WithName("Sign up")
            .Produces<OutTokenDto>();

        group
            .MapPost("/sign-out", SignOutAsync)
            .WithName("Sign out");

        group
            .MapGet("/check", () => JsonResponse.Success())
            .WithName("Check authentication")
            .RequireAuthorization();

        return group;
    }

    private static async Task<IResult> GetStatusAsync(AuthenticationService service)
    {
        var result = await service.GetStatusAsync();
        return result.Response();
    }

    private static async Task<IResult> SignInAsync(InSignInDto dto, AuthenticationService service)
    {
        var result = await service.SignInAsync(dto);
        return result.Response();
    }

    private static async Task<IResult> SignUpAsync(InSignUpDto dto, AuthenticationService service)
    {
        var result = await service.SignUpAsync(dto);
        return result.Response();
    }

    private static async Task<IResult> SignOutAsync(AuthenticationService service)
    {
        var result = await service.SignOutAsync();
        return result.Response();
    }
}