using Wobalization.Database.DatabaseContexts;
using Wobalization.Database.Models;
using Wobalization.Dtos.App;

namespace Wobalization.Database.Extensions;

public static class AppExtensions
{
    public static IQueryable<App> SearchAndPaginate(this IQueryable<App> queryable, string? search, int? page)
    {
        if (search != null)
        {
            queryable = queryable.Where(e => e.Name!.ToLower().Contains(search.ToLower()));
        }

        if (page != null)
        {
            queryable = queryable
                .Skip(page < 2 ? 0 : (page.GetValueOrDefault() - 1) * DatabaseContext.PaginationSize);
        }

        return queryable
            .OrderBy(e => e.Name)
            .Take(DatabaseContext.PaginationSize);
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