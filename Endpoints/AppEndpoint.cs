using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response.Extensions;
using Wobalization.Dtos.App;
using Wobalization.Services;

namespace Wobalization.Endpoints;

public class AppEndpoint : IEndpoints
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/app")
            .WithTags("App")
            .RequireAuthorization();

        group
            .MapGet("/", GetListAsync)
            .WithName("Get a list of app")
            .Produces<OutAppDto>();

        group
            .MapGet("/{id}", GetAsync)
            .WithName("Get an app by id")
            .Produces<OutAppDto>();

        group
            .MapPost("/", AddAsync)
            .WithName("Add new app")
            .Produces<OutAppDto>();

        group
            .MapPut("/", UpdateAsync)
            .WithName("Update existing app")
            .Produces<OutAppDto>();

        group
            .MapDelete("/{id}", DeleteAsync)
            .WithName("Delete existing app");

        return group;
    }

    private static async Task<IResult> GetListAsync(string? search, int? page, AppService service)
    {
        var result = await service.GetListAsync(search, page);
        return result.Response();
    }

    private static async Task<IResult> GetAsync(long id, AppService service)
    {
        var result = await service.GetAsync(id);
        return result.Response();
    }

    private static async Task<IResult> AddAsync(InAppDto dto, AppService service)
    {
        var result = await service.AddAsync(dto);
        return result.Response();
    }

    private static async Task<IResult> UpdateAsync(long id, InAppDto dto, AppService service)
    {
        var result = await service.UpdateAsync(id, dto);
        return result.Response();
    }

    private static async Task<IResult> DeleteAsync(long id, AppService service)
    {
        var result = await service.DeleteAsync(id);
        return result.Response();
    }
}