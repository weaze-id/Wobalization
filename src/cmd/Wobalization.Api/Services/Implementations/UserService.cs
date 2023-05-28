using FluentValidation;
using FluentValidation.Results;
using IdGen;
using Kern.AspNetCore.Extensions;
using Kern.Error;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.User;
using Shared.Entities;
using Shared.Entities.DatabaseContexts;
using Shared.Entities.Extensions;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly DatabaseContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly IdGenerator _idGenerator;
    private readonly IValidator<InUserDto> _validator;

    public UserService(
        DatabaseContext dbContext,
        IdGenerator idGenerator,
        IIdentityService identityService,
        IValidator<InUserDto> validator)
    {
        _dbContext = dbContext;
        _idGenerator = idGenerator;
        _identityService = identityService;
        _validator = validator;
    }

    public async Task<(OutUserDto?, ValidationResult?, ErrorBase?)> AddAsync(InUserDto dto)
    {
        // Validate the DTO
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        // Check if the username is already used
        var isUsernameUsed = await _dbContext.User!
            .HasUsername(dto.Username!)
            .NotDeleted()
            .AnyAsync();

        if (isUsernameUsed)
        {
            return (null, null, new ConflictError("Username already exist"));
        }

        // Create a new user with the provided information
        var user = new User
        {
            Id = _idGenerator.CreateId(),
            FullName = dto.FullName,
            Username = dto.Username,
            Password = BC.HashPassword("secret123"),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Add the user to the database and save changes
        await _dbContext.User!.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(user.Id.GetValueOrDefault());
        return (outDto, null, outDtoError);
    }

    public async Task<ErrorBase?> DeleteAsync(long id)
    {
        // Get user identity
        var (identity, identityError) = _identityService.Get();
        if (identityError != null)
        {
            return identityError;
        }

        // Delete the user
        var user = await _dbContext.User!
            .AsTracking()
            .HasId(id)
            .ExceptId(identity!.Id.GetValueOrDefault())
            .NotDeleted()
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return new NotFoundError("User not found");
        }

        user.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Save changes
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<(OutUserDto?, ErrorBase?)> GetAsync(long id)
    {
        var dto = await _dbContext.User!
            .HasId(id)
            .NotDeleted()
            .SelectDto()
            .FirstOrDefaultAsync();

        if (dto == null)
        {
            return (null, new NotFoundError("User not found"));
        }

        return (dto, null);
    }

    public async Task<(List<OutUserDto>?, ErrorBase?)> GetListAsync()
    {
        var dtos = await _dbContext.User!
            .NotDeleted()
            .SelectDto()
            .ToListAsync();

        return (dtos, null);
    }

    public async Task<(OutUserDto?, ValidationResult?, ErrorBase?)> UpdateAsync(long id, InUserDto dto)
    {
        // Get user identity
        var (identity, identityError) = _identityService.Get();
        if (identityError != null)
        {
            return (null, null, identityError);
        }

        // Validate the DTO
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        // Update the user with the provided information
        var user = await _dbContext.User!
            .AsTracking()
            .HasId(id)
            .NotDeleted()
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return (null, null, new NotFoundError("User not found"));
        }

        user.FullName = dto.FullName.EmptyToNull();
        user.Username = dto.Username.EmptyToNull();
        user.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Check if the username is already used
        var isUsernameUsed = await _dbContext.User!
            .HasUsername(dto.Username!)
            .ExceptId(identity!.Id.GetValueOrDefault())
            .NotDeleted()
            .AnyAsync();

        if (isUsernameUsed)
        {
            return (null, null, new ConflictError("Username already exist"));
        }

        // Save changes
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(id);
        return (outDto, null, outDtoError);
    }
}