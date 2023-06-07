using Shared.Dtos.Value;

namespace Shared.Entities.Extensions;

public static class TranslationValueExtensions
{
    public static IQueryable<TranslationValue> HasId(this IQueryable<TranslationValue> queryable, long id)
    {
        return queryable.Where(e => e.Id == id);
    }

    public static IQueryable<TranslationValue> ExceptId(this IQueryable<TranslationValue> queryable, long id)
    {
        return queryable.Where(e => e.Id != id);
    }

    public static IQueryable<TranslationValue> HasAppId(this IQueryable<TranslationValue> queryable, long appId)
    {
        return queryable.Where(e => e.TranslationKey!.AppId == appId);
    }

    public static IQueryable<TranslationValue> HasLanguageId(
        this IQueryable<TranslationValue> queryable,
        long languageId)
    {
        return queryable.Where(e => e.LanguageId == languageId);
    }

    public static IQueryable<TranslationValue> HasKeyId(this IQueryable<TranslationValue> queryable, long keyId)
    {
        return queryable.Where(e => e.TranslationKeyId == keyId);
    }

    public static IQueryable<TranslationValue> NotDeleted(this IQueryable<TranslationValue> queryable)
    {
        return queryable.Where(e => e.DeletedAt == null);
    }

    public static IQueryable<OutValueDto> SelectDto(this IQueryable<TranslationValue> queryable)
    {
        return queryable.Select(e => new OutValueDto
        {
            Id = e.Id,
            KeyId = e.TranslationKeyId,
            LanguageId = e.LanguageId,
            Value = e.Value,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }
}