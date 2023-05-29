using FluentValidation;
using FluentValidation.Results;
using IdGen;
using Kern.AspNetCore.Extensions;
using Kern.Error;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.App;
using Shared.Entities;
using Shared.Entities.DatabaseContexts;
using Shared.Entities.Extensions;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Services.Implementations;

/// <summary>
/// Service implementation for managing apps.
/// </summary>
public class AppService : IAppService
{
    private readonly DatabaseContext _dbContext;
    private readonly IdGenerator _idGenerator;
    private readonly IValidator<InAppDto> _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="idGenerator">The ID generator.</param>
    /// <param name="validator">The validator for app DTO.</param>
    public AppService(
        DatabaseContext dbContext,
        IdGenerator idGenerator,
        IValidator<InAppDto> validator)
    {
        _dbContext = dbContext;
        _idGenerator = idGenerator;
        _validator = validator;
    }

    /// <summary>
    /// Add a new app.
    /// </summary>
    /// <param name="dto">The app data to add.</param>
    /// <returns>A tuple containing the added app DTO, validation results, and an error if any.</returns>
    public async Task<(OutAppDto?, ValidationResult?, ErrorBase?)> AddAsync(InAppDto dto)
    {
        // Validate the DTO
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        // Check if the appname is already used
        var isAppNameUsed = await _dbContext.App!
            .HasName(dto.Name!)
            .NotDeleted()
            .AnyAsync();

        if (isAppNameUsed)
        {
            return (null, null, new ConflictError("App name already used"));
        }

        // Create a new app with the provided information
        var app = new App
        {
            Id = _idGenerator.CreateId(),
            Name = dto.Name.EmptyToNull(),
            Key = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Add the app to the database and save changes
        await _dbContext.App!.AddAsync(app);
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(app.Id.GetValueOrDefault());
        return (outDto, null, outDtoError);
    }

    /// <summary>
    /// Delete a app with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the app to delete.</param>
    /// <returns>An error if any.</returns>
    public async Task<ErrorBase?> DeleteAsync(long id)
    {
        // Delete the app
        var app = await _dbContext.App!
            .AsTracking()
            .HasId(id)
            .NotDeleted()
            .FirstOrDefaultAsync();

        if (app == null)
        {
            return new NotFoundError("App not found");
        }

        app.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Save changes
        await _dbContext.SaveChangesAsync();
        return null;
    }

    /// <summary>
    /// Get a app with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the app to retrieve.</param>
    /// <returns>A tuple containing the app DTO and an error if any.</returns>
    public async Task<(OutAppDto?, ErrorBase?)> GetAsync(long id)
    {
        var dto = await _dbContext.App!
            .HasId(id)
            .NotDeleted()
            .SelectDto()
            .FirstOrDefaultAsync();

        if (dto == null)
        {
            return (null, new NotFoundError("App not found"));
        }

        return (dto, null);
    }

    /// <summary>
    /// Get a list of all apps.
    /// </summary>
    /// <returns>A tuple containing the list of app DTOs and an error if any.</returns>
    public async Task<(List<OutAppDto>?, ErrorBase?)> GetListAsync()
    {
        var dtos = await _dbContext.App!
            .NotDeleted()
            .SelectDto()
            .ToListAsync();

        return (dtos, null);
    }

    /// <summary>
    /// Update a app with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the app to update.</param>
    /// <param name="dto">The updated app data.</param>
    /// <returns>A tuple containing the updated app DTO, validation results, and an error if any.</returns>
    public async Task<(OutAppDto?, ValidationResult?, ErrorBase?)> UpdateAsync(long id, InAppDto dto)
    {
        // Validate the DTO
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        // Update the app with the provided information
        var app = await _dbContext.App!
            .AsTracking()
            .HasId(id)
            .NotDeleted()
            .FirstOrDefaultAsync();

        if (app == null)
        {
            return (null, null, new NotFoundError("App not found"));
        }

        app.Name = dto.Name.EmptyToNull();
        app.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Check if the appname is already used
        var isAppNameUsed = await _dbContext.App!
            .HasName(dto.Name!)
            .ExceptId(id)
            .NotDeleted()
            .AnyAsync();

        if (isAppNameUsed)
        {
            return (null, null, new ConflictError("App name already used"));
        }

        // Save changes
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(id);
        return (outDto, null, outDtoError);
    }
}