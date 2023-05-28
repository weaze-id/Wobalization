using Shared.Dtos.App;

namespace Shared.Entities.Extensions;

public static class AppExtensions
{
    public static IQueryable<App> HasId(this IQueryable<App> queryable, long id)
    {
        return queryable.Where(e => e.Id == id);
    }

    public static IQueryable<App> ExceptId(this IQueryable<App> queryable, long id)
    {
        return queryable.Where(e => e.Id != id);
    }

    public static IQueryable<App> HasName(this IQueryable<App> queryable, string name)
    {
        return queryable.Where(e => e.Name!.ToLower() == name.ToLower());
    }

    public static IQueryable<App> NotDeleted(this IQueryable<App> queryable)
    {
        return queryable.Where(e => e.DeletedAt == null);
    }

    public static IQueryable<OutAppDto> SelectDto(this IQueryable<App> queryable)
    {
        return queryable.Select(e => new OutAppDto
        {
            Id = e.Id,
            Name = e.Name,
            Key = e.Key,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }
}