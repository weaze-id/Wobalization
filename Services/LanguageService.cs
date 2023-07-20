using FluentValidation;
using FluentValidation.Results;
using IdGen;
using Kern.Error;
using Kern.Extensions;
using Microsoft.EntityFrameworkCore;
using Wobalization.Database.DatabaseContexts;
using Wobalization.Database.Extensions;
using Wobalization.Database.Models;
using Wobalization.Dtos.Language;

namespace Wobalization.Services;

/// <summary>
/// Service implementation for managing languages.
/// </summary>
public class LanguageService
{
    private readonly DatabaseContext _dbContext;
    private readonly IdGenerator _idGenerator;
    private readonly IValidator<InLanguageDto> _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="idGenerator">The ID generator.</param>
    /// <param name="validator">The validator for language DTO.</param>
    public LanguageService(
        DatabaseContext dbContext,
        IdGenerator idGenerator,
        IValidator<InLanguageDto> validator)
    {
        _dbContext = dbContext;
        _idGenerator = idGenerator;
        _validator = validator;
    }

    /// <summary>
    /// Get a language with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the language to retrieve.</param>
    /// <returns>A tuple containing the language DTO and an error if any.</returns>
    public async Task<(OutLanguageDto?, ErrorBase?)> GetAsync(long appId, long id)
    {
        // Check if the app exist
        var isAppExist = await _dbContext.App!
            .Where(e => e.Id == appId &&
                        e.DeletedAt == null)
            .AnyAsync();

        if (!isAppExist)
        {
            return (null, new NotFoundError("App not found"));
        }

        var dto = await _dbContext.TranslationLanguage!
            .Where(e => e.Id == id &&
                        e.AppId == appId &&
                        e.DeletedAt == null)
            .SelectDto()
            .FirstOrDefaultAsync();

        if (dto == null)
        {
            return (null, new NotFoundError("Language not found"));
        }

        return (dto, null);
    }

    /// <summary>
    /// Get a list of all languages.
    /// </summary>
    /// <returns>A tuple containing the list of language DTOs and an error if any.</returns>
    public async Task<(List<OutLanguageDto>?, ErrorBase?)> GetListAsync(long appId, string? search, int? page)
    {
        // Check if the app exist
        var isAppExist = await _dbContext.App!
            .Where(e => e.Id == appId &&
                        e.DeletedAt == null)
            .AnyAsync();

        if (!isAppExist)
        {
            return (null, new NotFoundError("App not found"));
        }

        var dtos = await _dbContext.TranslationLanguage!
            .Where(e => e.AppId == appId &&
                        e.DeletedAt == null)
            .SearchAndPaginate(search, page)
            .SelectDto()
            .ToListAsync();

        return (dtos, null);
    }

    /// <summary>
    /// Add a new language.
    /// </summary>
    /// <param name="dto">The language data to add.</param>
    /// <returns>A tuple containing the added language DTO, validation results, and an error if any.</returns>
    public async Task<(OutLanguageDto?, ValidationResult?, ErrorBase?)> AddAsync(long appId, InLanguageDto dto)
    {
        // Validate the DTO
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        // Check if the app exist
        var isAppExist = await _dbContext.App!
            .Where(e => e.Id == appId &&
                        e.DeletedAt == null)
            .AnyAsync();

        if (!isAppExist)
        {
            return (null, null, new NotFoundError("App not found"));
        }

        // Check if the culture is already exist
        var isCultureExist = await _dbContext.TranslationLanguage!
            .Where(e => e.AppId == appId &&
                        e.Culture!.ToLower() == dto.Culture!.ToLower() &&
                        e.DeletedAt == null)
            .AnyAsync();

        if (isCultureExist)
        {
            return (null, null, new NotFoundError("Language culture already exist"));
        }

        // Create a new language with the provided information
        var language = new TranslationLanguage
        {
            Id = _idGenerator.CreateId(),
            AppId = appId,
            Culture = dto.Culture.EmptyToNull(),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Add the language to the database and save changes
        await _dbContext.TranslationLanguage!.AddAsync(language);
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(appId, language.Id.GetValueOrDefault());
        return (outDto, null, outDtoError);
    }

    /// <summary>
    /// Update a language with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the language to update.</param>
    /// <param name="dto">The updated language data.</param>
    /// <returns>A tuple containing the updated language DTO, validation results, and an error if any.</returns>
    public async Task<(OutLanguageDto?, ValidationResult?, ErrorBase?)> UpdateAsync(
        long appId,
        long id,
        InLanguageDto dto)
    {
        // Validate the DTO
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        // Check if the app exist
        var isAppExist = await _dbContext.App!
            .Where(e => e.Id == appId &&
                        e.DeletedAt == null)
            .AnyAsync();

        if (!isAppExist)
        {
            return (null, null, new NotFoundError("App not found"));
        }

        // Update the language with the provided information
        var language = await _dbContext.TranslationLanguage!
            .AsTracking()
            .Where(e => e.Id == id &&
                        e.AppId == appId &&
                        e.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (language == null)
        {
            return (null, null, new NotFoundError("Language not found"));
        }

        language.Culture = dto.Culture.EmptyToNull();
        language.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Check if the culture is already exist
        var isCultureExist = await _dbContext.TranslationLanguage!
            .Where(e => e.Id != id &&
                        e.AppId == appId &&
                        e.Culture!.ToLower() == dto.Culture!.ToLower() &&
                        e.DeletedAt == null)
            .AnyAsync();

        if (isCultureExist)
        {
            return (null, null, new ConflictError("Language culture already exist"));
        }

        // Save changes
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(appId, id);
        return (outDto, null, outDtoError);
    }

    /// <summary>
    /// Delete a language with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the language to delete.</param>
    /// <returns>An error if any.</returns>
    public async Task<ErrorBase?> DeleteAsync(long appId, long id)
    {
        // Check if the app exist
        var isAppExist = await _dbContext.App!
            .Where(e => e.Id == appId &&
                        e.DeletedAt == null)
            .AnyAsync();

        if (!isAppExist)
        {
            return new NotFoundError("App not found");
        }

        // Delete the language
        var language = await _dbContext.TranslationLanguage!
            .AsTracking()
            .Where(e => e.Id == id &&
                        e.AppId == appId &&
                        e.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (language == null)
        {
            return new NotFoundError("Language not found");
        }

        language.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Save changes
        await _dbContext.SaveChangesAsync();
        return null;
    }
}