using Kern.AspNetCore.Endpoints;
using Kern.AspNetCore.Response.Extensions;
using Wobalization.Dtos.Translation;
using Wobalization.Services;

namespace Wobalization.Endpoints;

public class TranslationEndpoint : IEndpoints
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/translation/{appKey}/{locale}")
            .WithTags("Translation");

        group
            .MapGet("/", GetListAsync)
            .WithName("Get a list of translation")
            .Produces<OutTranslationDto>();

        return group;
    }

    private static async Task<IResult> GetListAsync(Guid appKey, string locale, TranslationService service)
    {
        var result = await service.GetListAsync(appKey, locale);
        return result.Response();
    }
}