using Wobalization.Dtos.User;

namespace Wobalization.Entities.Extensions;

public static class UserExtensions
{
    public static IQueryable<User> HasId(this IQueryable<User> queryable, long id)
    {
        return queryable.Where(e => e.Id == id);
    }

    public static IQueryable<User> ExceptId(this IQueryable<User> queryable, long id)
    {
        return queryable.Where(e => e.Id != id);
    }

    public static IQueryable<User> HasUsername(this IQueryable<User> queryable, string username)
    {
        return queryable.Where(e => e.Username!.ToLower() == username.ToLower());
    }

    public static IQueryable<User> NotDeleted(this IQueryable<User> queryable)
    {
        return queryable.Where(e => e.DeletedAt == null);
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