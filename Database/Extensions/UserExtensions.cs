using Wobalization.Database.DatabaseContexts;
using Wobalization.Database.Models;
using Wobalization.Dtos.User;

namespace Wobalization.Database.Extensions;

public static class UserExtensions
{
    public static IQueryable<User> SearchAndPaginate(this IQueryable<User> queryable, string? search, int? page)
    {
        if (search != null)
        {
            queryable = queryable
                .Where(e => e.FullName!.ToLower().Contains(search.ToLower()) ||
                            e.Username!.ToLower().Contains(search.ToLower()));
        }

        if (page != null)
        {
            queryable = queryable
                .Skip(page < 2 ? 0 : (page.GetValueOrDefault() - 1) * DatabaseContext.PaginationSize);
        }

        return queryable
            .OrderBy(e => e.FullName)
            .Take(DatabaseContext.PaginationSize);
    }

    public static IQueryable<OutUserDto> SelectDto(this IQueryable<User> queryable)
    {
        return queryable.Select(e => new OutUserDto
        {
            Id = e.Id,
            FullName = e.FullName,
            Username = e.Username,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }
}