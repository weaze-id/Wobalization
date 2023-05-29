using FluentValidation.Results;
using Kern.Error;
using Shared.Dtos.Language;

namespace Wobalization.Api.Services.Interfaces;

public interface ILanguageService
{
    public Task<(OutLanguageDto?, ErrorBase?)> GetAsync(long appId, long id);
    public Task<(List<OutLanguageDto>?, ErrorBase?)> GetListAsync(long appId);
    public Task<(OutLanguageDto?, ValidationResult?, ErrorBase?)> AddAsync(long appId, InLanguageDto dto);
    public Task<(OutLanguageDto?, ValidationResult?, ErrorBase?)> UpdateAsync(long appId, long id, InLanguageDto dto);
    public Task<ErrorBase?> DeleteAsync(long appId, long id);
}