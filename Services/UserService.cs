using FluentValidation;
using FluentValidation.Results;
using IdGen;
using Kern.Error;
using Kern.Extensions;
using Microsoft.EntityFrameworkCore;
using Wobalization.Dtos.User;
using Wobalization.Entities;
using Wobalization.Entities.DatabaseContexts;
using Wobalization.Entities.Extensions;
using Wobalization.Services;

namespace Wobalization.Services;

/// <summary>
/// Service implementation for managing users.
/// </summary>
public class UserService
{
    private readonly DatabaseContext _dbContext;
    private readonly IdentityService _identityService;
    private readonly IdGenerator _idGenerator;
    private readonly IValidator<InUserDto> _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="idGenerator">The ID generator.</param>
    /// <param name="identityService">The identity service.</param>
    /// <param name="validator">The validator for user DTO.</param>
    public UserService(
        DatabaseContext dbContext,
        IdGenerator idGenerator,
        IdentityService identityService,
        IValidator<InUserDto> validator)
    {
        _dbContext = dbContext;
        _idGenerator = idGenerator;
        _identityService = identityService;
        _validator = validator;
    }

    /// <summary>
    /// Add a new user.
    /// </summary>
    /// <param name="dto">The user data to add.</param>
    /// <returns>A tuple containing the added user DTO, validation results, and an error if any.</returns>
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
            return (null, null, new ConflictError("Username already used"));
        }

        // Create a new user with the provided information
        var user = new User
        {
            Id = _idGenerator.CreateId(),
            FullName = dto.FullName.EmptyToNull(),
            Username = dto.Username.EmptyToNull(),
            Password = BC.HashPassword("secret123"),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Add the user to the database and save changes
        await _dbContext.User!.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(user.Id.GetValueOrDefault());
        return (outDto, null, outDtoError);
    }

    /// <summary>
    /// Delete a user with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>An error if any.</returns>
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

    /// <summary>
    /// Get a user with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>A tuple containing the user DTO and an error if any.</returns>
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

    /// <summary>
    /// Get a list of all users.
    /// </summary>
    /// <returns>A tuple containing the list of user DTOs and an error if any.</returns>
    public async Task<(List<OutUserDto>?, ErrorBase?)> GetListAsync()
    {
        var dtos = await _dbContext.User!
            .NotDeleted()
            .SelectDto()
            .ToListAsync();

        return (dtos, null);
    }

    /// <summary>
    /// Update a user with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="dto">The updated user data.</param>
    /// <returns>A tuple containing the updated user DTO, validation results, and an error if any.</returns>
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
            .ExceptId(id)
            .NotDeleted()
            .AnyAsync();

        if (isUsernameUsed)
        {
            return (null, null, new ConflictError("Username already used"));
        }

        // Save changes
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(id);
        return (outDto, null, outDtoError);
    }
}