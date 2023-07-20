namespace Wobalization.Database.Models;

public class TranslationKey
{
    public long? Id { get; set; }
    public long? AppId { get; set; }
    public string? Key { get; set; }
    public long? CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }
    public long? DeletedAt { get; set; }

    public App? App { get; set; }
    public List<TranslationValue>? TranslationValues { get; set; }
}