using Wobalization.Dtos.Language;

namespace Wobalization.Entities.Extensions;

public static class TranslationLanguageExtensions
{
    public static IQueryable<TranslationLanguage> HasId(this IQueryable<TranslationLanguage> queryable, long id)
    {
        return queryable.Where(e => e.Id == id);
    }

    public static IQueryable<TranslationLanguage> ExceptId(this IQueryable<TranslationLanguage> queryable, long id)
    {
        return queryable.Where(e => e.Id != id);
    }

    public static IQueryable<TranslationLanguage> HasAppId(this IQueryable<TranslationLanguage> queryable, long appId)
    {
        return queryable.Where(e => e.AppId == appId);
    }

    public static IQueryable<TranslationLanguage> HasCulture(this IQueryable<TranslationLanguage> queryable, string culture)
    {
        return queryable.Where(e => e.Culture!.ToLower() == culture.ToLower());
    }

    public static IQueryable<TranslationLanguage> NotDeleted(this IQueryable<TranslationLanguage> queryable)
    {
        return queryable.Where(e => e.DeletedAt == null);
    }

    public static IQueryable<OutLanguageDto> SelectDto(this IQueryable<TranslationLanguage> queryable)
    {
        return queryable.Select(e => new OutLanguageDto
        {
            Id = e.Id,
            Culture = e.Culture,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }
}