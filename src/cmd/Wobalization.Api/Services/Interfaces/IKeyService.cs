using FluentValidation.Results;
using Kern.Error;
using Shared.Dtos.Key;

namespace Wobalization.Api.Services.Interfaces;

public interface IKeyService
{
    public Task<(OutKeyDto?, ErrorBase?)> GetAsync(long appId, long id);
    public Task<(List<OutKeyDto>?, ErrorBase?)> GetListAsync(long appId, string? search, long? lastId);
    public Task<(OutKeyDto?, ValidationResult?, ErrorBase?)> AddAsync(long appId, InKeyDto dto);
    public Task<(OutKeyDto?, ValidationResult?, ErrorBase?)> UpdateAsync(long appId, long id, InKeyDto dto);
    public Task<ErrorBase?> DeleteAsync(long appId, long id);
}