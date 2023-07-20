using Wobalization.Database.DatabaseContexts;
using Wobalization.Database.Models;
using Wobalization.Dtos.Language;

namespace Wobalization.Database.Extensions;

public static class TranslationLanguageExtensions
{
    public static IQueryable<TranslationLanguage> SearchAndPaginate(
        this IQueryable<TranslationLanguage> queryable,
        string? search,
        int? page)
    {
        if (search != null)
        {
            queryable = queryable.Where(e => e.Locale!.ToLower().Contains(search.ToLower()));
        }

        if (page != null)
        {
            queryable = queryable
                .Skip(page < 2 ? 0 : (page.GetValueOrDefault() - 1) * DatabaseContext.PaginationSize);
        }

        return queryable
            .OrderBy(e => e.Locale)
            .Take(DatabaseContext.PaginationSize);
    }

    public static IQueryable<OutLanguageDto> SelectDto(this IQueryable<TranslationLanguage> queryable)
    {
        return queryable.Select(e => new OutLanguageDto
        {
            Id = e.Id,
            Locale = e.Locale,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }
}