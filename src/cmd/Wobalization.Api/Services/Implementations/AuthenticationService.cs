using FluentValidation;
using FluentValidation.Results;
using Kern.Error;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Authentication;
using Shared.Entities;
using Shared.Entities.DatabaseContexts;
using Wobalization.Api.Models;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Services.Implementations;

public class AuthenticationService : IAuthenticationService
{
    private readonly DatabaseContext _dbContext;
    private readonly IdentityService _identityService;
    private readonly IValidator<InLoginDto> _validator;

    public AuthenticationService(
        DatabaseContext dbContext,
        IValidator<InLoginDto> validator,
        IdentityService identityService)
    {
        _dbContext = dbContext;
        _validator = validator;
        _identityService = identityService;
    }

    public async Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignInAsync(InLoginDto dto)
    {
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        var user = await _dbContext.User!
            .Where(e => e.Username!.ToLowerInvariant() == dto.Username!.ToLowerInvariant())
            .Select(e => new User
            {
                Id = e.Id,
                Password = e.Password
            })
            .FirstOrDefaultAsync();

        if (user == null || !BC.Verify(dto.Password, user.Password))
        {
            return (null, null, new BadRequestError("Username or password is wrong"));
        }

        var identity = new IdentityModel
        {
            Id = user.Id
        };

        var token = await _identityService.SignInAsync(identity);
        return (new OutTokenDto { Token = token }, null, null);
    }

    public async Task<ErrorBase?> SignOutAsync()
    {
        return await _identityService.SignOutAsync();
    }
}