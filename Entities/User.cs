namespace Wobalization.Entities;

public class User
{
    public long? Id { get; set; }
    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public long? CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }
    public long? DeletedAt { get; set; }
}