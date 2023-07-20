using Wobalization.Database.DatabaseContexts;
using Wobalization.Database.Models;
using Wobalization.Dtos.Key;

namespace Wobalization.Database.Extensions;

public static class TranslationKeyExtensions
{
    public static IQueryable<TranslationKey> SearchAndPaginate(this IQueryable<TranslationKey> queryable, string? search, int? page)
    {
        if (search != null)
        {
            queryable = queryable.Where(e => e.Key!.ToLower().Contains(search.ToLower()));
        }

        if (page != null)
        {
            queryable = queryable
                .Skip(page < 2 ? 0 : (page.GetValueOrDefault() - 1) * DatabaseContext.PaginationSize);
        }

        return queryable
            .OrderBy(e => e.Key)
            .Take(DatabaseContext.PaginationSize);
    }

    public static IQueryable<OutKeyDto> SelectDto(this IQueryable<TranslationKey> queryable)
    {
        return queryable.Select(e => new OutKeyDto
        {
            Id = e.Id,
            AppId = e.AppId,
            Key = e.Key,
            Values = e.TranslationValues!
                .Where(e => e.DeletedAt == null && e.TranslationLanguage!.DeletedAt == null)
                .Select(e => new OutKeyValueDto
                {
                    Id = e.Id,
                    LanguageId = e.TranslationLanguageId,
                    Value = e.Value,
                    CreatedAt = e.CreatedAt,
                })
                .ToList(),
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }
}