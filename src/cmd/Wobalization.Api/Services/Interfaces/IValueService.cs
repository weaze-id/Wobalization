using FluentValidation.Results;
using Kern.Error;
using Shared.Dtos.Value;

namespace Wobalization.Api.Services.Interfaces;

public interface IValueService
{
    public Task<(OutValueDto?, ErrorBase?)> GetAsync(long appId, long languageId, long id);

    public Task<(List<OutValueDto>?, ErrorBase?)> GetListAsync(
        long appId,
        long languageId,
        string? search,
        long? lastId);

    public Task<(OutValueDto?, ValidationResult?, ErrorBase?)> AddAsync(long appId, long languageId, InValueDto dto);

    public Task<(OutValueDto?, ValidationResult?, ErrorBase?)> UpdateAsync(
        long appId,
        long languageId,
        long id,
        InValueDto dto);

    public Task<ErrorBase?> DeleteAsync(long appId, long languageId, long id);
}