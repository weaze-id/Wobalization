using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response.Extensions;
using Wobalization.Dtos.Key;
using Wobalization.Services;

namespace Wobalization.Endpoints;

public class KeyEndpoint : IEndpoints
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/app/{appId}/key")
            .WithTags("Key")
            .RequireAuthorization();

        group
            .MapGet("/", GetListAsync)
            .WithName("Get a list of key")
            .Produces<OutKeyDto>();

        group
            .MapGet("/{id}", GetAsync)
            .WithName("Get a key by id")
            .Produces<OutKeyDto>();

        group
            .MapPost("/", AddAsync)
            .WithName("Add new key")
            .Produces<OutKeyDto>();

        group
            .MapPut("/", UpdateAsync)
            .WithName("Update existing key")
            .Produces<OutKeyDto>();

        group
            .MapDelete("/{id}", DeleteAsync)
            .WithName("Delete existing key");

        group
            .MapPost("/{id}/value", AddValueAsync)
            .WithName("Add new value to existing key");

        return group;
    }

    private static async Task<IResult> GetListAsync(long appId, string? search, long? lastId, KeyService service)
    {
        var result = await service.GetListAsync(appId, search, lastId);
        return result.Response();
    }

    private static async Task<IResult> GetAsync(long appId, long id, KeyService service)
    {
        var result = await service.GetAsync(appId, id);
        return result.Response();
    }

    private static async Task<IResult> AddAsync(long appId, InKeyDto dto, KeyService service)
    {
        var result = await service.AddAsync(appId, dto);
        return result.Response();
    }

    private static async Task<IResult> UpdateAsync(long appId, long id, InKeyDto dto, KeyService service)
    {
        var result = await service.UpdateAsync(appId, id, dto);
        return result.Response();
    }

    private static async Task<IResult> DeleteAsync(long appId, long id, KeyService service)
    {
        var result = await service.DeleteAsync(appId, id);
        return result.Response();
    }

    private static async Task<IResult> AddValueAsync(long appId, long id, InKeyValueDto dto, KeyService service)
    {
        var result = await service.AddValueAsync(appId, id, dto);
        return result.Response();
    }
}