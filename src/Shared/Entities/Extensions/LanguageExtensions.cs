using Shared.Dtos.Language;

namespace Shared.Entities.Extensions;

public static class LanguageExtensions
{
    public static IQueryable<Language> HasId(this IQueryable<Language> queryable, long id)
    {
        return queryable.Where(e => e.Id == id);
    }

    public static IQueryable<Language> ExceptId(this IQueryable<Language> queryable, long id)
    {
        return queryable.Where(e => e.Id != id);
    }

    public static IQueryable<Language> HasAppId(this IQueryable<Language> queryable, long appId)
    {
        return queryable.Where(e => e.AppId == appId);
    }

    public static IQueryable<Language> HasCulture(this IQueryable<Language> queryable, string culture)
    {
        return queryable.Where(e => e.Culture!.ToLower() == culture.ToLower());
    }

    public static IQueryable<Language> NotDeleted(this IQueryable<Language> queryable)
    {
        return queryable.Where(e => e.DeletedAt == null);
    }

    public static IQueryable<OutLanguageDto> SelectDto(this IQueryable<Language> queryable)
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