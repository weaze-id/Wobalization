namespace Wobalization.Dtos.Key;

public class OutKeyDto
{
    public long? Id { get; set; }
    public long? AppId { get; set; }
    public string? Key { get; set; }
    public List<OutKeyValueDto>? Values { get; set; }
    public long? CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }
}