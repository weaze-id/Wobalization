using FluentValidation.Results;
using Kern.Error;
using Shared.Dtos.App;

namespace Wobalization.Api.Services.Interfaces;

public interface IAppService
{
    public Task<(OutAppDto?, ErrorBase?)> GetAsync(long id);
    public Task<(List<OutAppDto>?, ErrorBase?)> GetListAsync();
    public Task<(OutAppDto?, ValidationResult?, ErrorBase?)> AddAsync(InAppDto dto);
    public Task<(OutAppDto?, ValidationResult?, ErrorBase?)> UpdateAsync(long id, InAppDto dto);
    public Task<ErrorBase?> DeleteAsync(long id);
}