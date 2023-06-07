namespace Shared.Entities;

public class TranslationValue
{
    public long? Id { get; set; }
    public long? TranslationKeyId { get; set; }
    public long? LanguageId { get; set; }
    public string? Value { get; set; }
    public long? CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }
    public long? DeletedAt { get; set; }

    public TranslationKey? TranslationKey { get; set; }
    public Language? Language { get; set; }
}