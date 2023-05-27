using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response.Extensions;
using Shared.Dtos.Authentication;
using Shared.Dtos.User;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Endpoints;

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

        return group;
    }

    private static async Task<IResult> GetStatusAsync(IAuthenticationService service)
    {
        var result = await service.GetStatusAsync();
        return result.Response();
    }

    private static async Task<IResult> SignInAsync(IAuthenticationService service, InLoginDto dto)
    {
        var result = await service.SignInAsync(dto);
        return result.Response();
    }

    private static async Task<IResult> SignUpAsync(IAuthenticationService service, InUserAddDto dto)
    {
        var result = await service.SignUpAsync(dto);
        return result.Response();
    }

    private static async Task<IResult> SignOutAsync(IAuthenticationService service)
    {
        var result = await service.SignOutAsync();
        return result.Response();
    }
}