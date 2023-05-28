using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response.Extensions;
using Shared.Dtos.User;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Endpoints;

public class UserEndpoint : IEndpoints
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/user")
            .WithTags("User")
            .RequireAuthorization();

        group
            .MapGet("/", GetListAsync)
            .WithName("Get a list of user")
            .Produces<OutUserDto>();

        group
            .MapGet("/{id}", GetAsync)
            .WithName("Get an user by id")
            .Produces<OutUserDto>();

        group
            .MapPost("/", AddAsync)
            .WithName("Add new user")
            .Produces<OutUserDto>();

        group
            .MapPut("/", UpdateAsync)
            .WithName("Update existing user")
            .Produces<OutUserDto>();

        group
            .MapDelete("/{id}", DeleteAsync)
            .WithName("Delete existing user");

        return group;
    }

    private static async Task<IResult> GetListAsync(IUserService service)
    {
        var result = await service.GetListAsync();
        return result.Response();
    }

    private static async Task<IResult> GetAsync(long id, IUserService service)
    {
        var result = await service.GetAsync(id);
        return result.Response();
    }

    private static async Task<IResult> AddAsync(InUserDto dto, IUserService service)
    {
        var result = await service.AddAsync(dto);
        return result.Response();
    }

    private static async Task<IResult> UpdateAsync(long id, InUserDto dto, IUserService service)
    {
        var result = await service.UpdateAsync(id, dto);
        return result.Response();
    }

    private static async Task<IResult> DeleteAsync(long id, IUserService service)
    {
        var result = await service.DeleteAsync(id);
        return result.Response();
    }
}