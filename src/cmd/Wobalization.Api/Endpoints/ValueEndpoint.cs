using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response.Extensions;
using Shared.Dtos.Value;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Endpoints;

public class ValueEndpoint : IEndpoints
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("app/{appId}/language/{languageId}/value")
            .WithTags("Value")
            .RequireAuthorization();

        group
            .MapGet("/", GetListAsync)
            .WithName("Get a list of value")
            .Produces<OutValueDto>();

        group
            .MapGet("/{id}", GetAsync)
            .WithName("Get a value by id")
            .Produces<OutValueDto>();

        group
            .MapPost("/", AddAsync)
            .WithName("Add new value")
            .Produces<OutValueDto>();

        group
            .MapPut("/", UpdateAsync)
            .WithName("Update existing value")
            .Produces<OutValueDto>();

        group
            .MapDelete("/{id}", DeleteAsync)
            .WithName("Delete existing value");

        return group;
    }

    private static async Task<IResult> GetListAsync(
        long appId,
        long languageId,
        string? search,
        long? lastId,
        IValueService service)
    {
        var result = await service.GetListAsync(appId, languageId, search, lastId);
        return result.Response();
    }

    private static async Task<IResult> GetAsync(long appId, long languageId, long id, IValueService service)
    {
        var result = await service.GetAsync(appId, languageId, id);
        return result.Response();
    }

    private static async Task<IResult> AddAsync(long appId, long languageId, InValueDto dto, IValueService service)
    {
        var result = await service.AddAsync(appId, languageId, dto);
        return result.Response();
    }

    private static async Task<IResult> UpdateAsync(
        long appId,
        long languageId,
        long id,
        InValueDto dto,
        IValueService service)
    {
        var result = await service.UpdateAsync(appId, languageId, id, dto);
        return result.Response();
    }

    private static async Task<IResult> DeleteAsync(long appId, long languageId, long id, IValueService service)
    {
        var result = await service.DeleteAsync(appId, languageId, id);
        return result.Response();
    }
}