using FluentValidation.Results;
using Kern.Error;
using Shared.Dtos.User;

namespace Wobalization.Api.Services.Interfaces;

public interface IUserService
{
    public Task<(OutUserDto?, ErrorBase?)> GetAsync(long id);
    public Task<(List<OutUserDto>?, ErrorBase?)> GetListAsync();
    public Task<(OutUserDto?, ValidationResult?, ErrorBase?)> AddAsync(InUserDto dto);
    public Task<(OutUserDto?, ValidationResult?, ErrorBase?)> UpdateAsync(long id, InUserDto dto);
    public Task<ErrorBase?> DeleteAsync(long id);
}