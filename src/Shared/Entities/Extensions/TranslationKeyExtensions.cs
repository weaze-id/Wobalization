using Shared.Dtos.Key;

namespace Shared.Entities.Extensions;

public static class TranslationKeyExtensions
{
    public static IQueryable<TranslationKey> HasId(this IQueryable<TranslationKey> queryable, long id)
    {
        return queryable.Where(e => e.Id == id);
    }

    public static IQueryable<TranslationKey> ExceptId(this IQueryable<TranslationKey> queryable, long id)
    {
        return queryable.Where(e => e.Id != id);
    }

    public static IQueryable<TranslationKey> HasAppId(this IQueryable<TranslationKey> queryable, long appId)
    {
        return queryable.Where(e => e.AppId == appId);
    }

    public static IQueryable<TranslationKey> HasKey(this IQueryable<TranslationKey> queryable, string key)
    {
        return queryable.Where(e => e.Key!.ToLower() == key.ToLower());
    }

    public static IQueryable<TranslationKey> NotDeleted(this IQueryable<TranslationKey> queryable)
    {
        return queryable.Where(e => e.DeletedAt == null);
    }

    public static IQueryable<OutKeyDto> SelectDto(this IQueryable<TranslationKey> queryable)
    {
        return queryable.Select(e => new OutKeyDto
        {
            Id = e.Id,
            AppId = e.AppId,
            Key = e.Key,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }
}