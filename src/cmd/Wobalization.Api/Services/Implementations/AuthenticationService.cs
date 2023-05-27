using FluentValidation;
using FluentValidation.Results;
using IdGen;
using Kern.Error;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Authentication;
using Shared.Dtos.User;
using Shared.Entities;
using Shared.Entities.DatabaseContexts;
using Wobalization.Api.Models;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Services.Implementations;

public class AuthenticationService : IAuthenticationService
{
    private readonly DatabaseContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly IdGenerator _idGenerator;
    private readonly IValidator<InLoginDto> _loginValidator;
    private readonly IValidator<InUserAddDto> _registerValidator;


    public AuthenticationService(
        DatabaseContext dbContext,
        IValidator<InLoginDto> loginValidator,
        IIdentityService identityService,
        IValidator<InUserAddDto> registerValidator,
        IdGenerator idGenerator)
    {
        _dbContext = dbContext;
        _loginValidator = loginValidator;
        _identityService = identityService;
        _registerValidator = registerValidator;
        _idGenerator = idGenerator;
    }

    public async Task<ErrorBase?> GetStatusAsync()
    {
        var isAnyUserExist = await _dbContext.User!.AnyAsync();
        if (isAnyUserExist)
        {
            return null;
        }

        return new NotFoundError("No user found, app has not been initialized");
    }

    public async Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignInAsync(InLoginDto dto)
    {
        var validationResult = _loginValidator.Validate(dto);
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

    public async Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignUpAsync(InUserAddDto dto)
    {
        var validationResult = _registerValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        var isAnyUserExist = await _dbContext.User!.AnyAsync();
        if (isAnyUserExist)
        {
            return (null, null, new ConflictError("Can't create an account, app has been initialized"));
        }

        var user = new User
        {
            Id = _idGenerator.CreateId(),
            FullName = dto.FullName,
            Username = dto.Username,
            Password = BC.HashPassword(dto.Password),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        await _dbContext.User!.AddAsync(user);
        await _dbContext.SaveChangesAsync();

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