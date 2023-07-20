using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response.Extensions;
using Wobalization.Dtos.Language;
using Wobalization.Services;

namespace Wobalization.Endpoints;

public class LanguageEndpoint : IEndpoints
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/app/{appId}/language")
            .WithTags("Language")
            .RequireAuthorization();

        group
            .MapGet("/", GetListAsync)
            .WithName("Get a list of language")
            .Produces<OutLanguageDto>();

        group
            .MapGet("/{id}", GetAsync)
            .WithName("Get an language by id")
            .Produces<OutLanguageDto>();

        group
            .MapPost("/", AddAsync)
            .WithName("Add new language")
            .Produces<OutLanguageDto>();

        group
            .MapPut("/", UpdateAsync)
            .WithName("Update existing language")
            .Produces<OutLanguageDto>();

        group
            .MapDelete("/{id}", DeleteAsync)
            .WithName("Delete existing language");

        return group;
    }

    private static async Task<IResult> GetListAsync(long appId, string? search, int? page, LanguageService service)
    {
        var result = await service.GetListAsync(appId, search, page);
        return result.Response();
    }

    private static async Task<IResult> GetAsync(long appId, long id, LanguageService service)
    {
        var result = await service.GetAsync(appId, id);
        return result.Response();
    }

    private static async Task<IResult> AddAsync(long appId, InLanguageDto dto, LanguageService service)
    {
        var result = await service.AddAsync(appId, dto);
        return result.Response();
    }

    private static async Task<IResult> UpdateAsync(long appId, long id, InLanguageDto dto, LanguageService service)
    {
        var result = await service.UpdateAsync(appId, id, dto);
        return result.Response();
    }

    private static async Task<IResult> DeleteAsync(long appId, long id, LanguageService service)
    {
        var result = await service.DeleteAsync(appId, id);
        return result.Response();
    }
}