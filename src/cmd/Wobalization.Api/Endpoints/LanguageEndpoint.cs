using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response.Extensions;
using Shared.Dtos.Language;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Endpoints;

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

    private static async Task<IResult> GetListAsync(long appId, ILanguageService service)
    {
        var result = await service.GetListAsync(appId);
        return result.Response();
    }

    private static async Task<IResult> GetAsync(long appId, long id, ILanguageService service)
    {
        var result = await service.GetAsync(appId, id);
        return result.Response();
    }

    private static async Task<IResult> AddAsync(long appId, InLanguageDto dto, ILanguageService service)
    {
        var result = await service.AddAsync(appId, dto);
        return result.Response();
    }

    private static async Task<IResult> UpdateAsync(long appId, long id, InLanguageDto dto, ILanguageService service)
    {
        var result = await service.UpdateAsync(appId, id, dto);
        return result.Response();
    }

    private static async Task<IResult> DeleteAsync(long appId, long id, ILanguageService service)
    {
        var result = await service.DeleteAsync(appId, id);
        return result.Response();
    }
}