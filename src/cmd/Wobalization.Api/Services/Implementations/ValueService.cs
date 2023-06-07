using FluentValidation;
using FluentValidation.Results;
using IdGen;
using Kern.AspNetCore.Extensions;
using Kern.Error;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Value;
using Shared.Entities;
using Shared.Entities.DatabaseContexts;
using Shared.Entities.Extensions;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Services.Implementations;

/// <summary>
/// Represents a ValueService that implements the ValueService interface.
/// </summary>
public class ValueService : IValueService
{
    private readonly DatabaseContext _dbContext;
    private readonly IdGenerator _idGenerator;
    private readonly IValidator<InValueDto> _validator;

    /// <summary>
    /// Initializes a new instance of the ValueService class with the specified dependencies.
    /// </summary>
    /// <param name="dbContext">The DatabaseContext instance.</param>
    /// <param name="idGenerator">The IdGenerator instance.</param>
    /// <param name="validator">The IValidator of InValueDto instance.</param>
    public ValueService(DatabaseContext dbContext, IdGenerator idGenerator, IValidator<InValueDto> validator)
    {
        _dbContext = dbContext;
        _idGenerator = idGenerator;
        _validator = validator;
    }

    /// <summary>
    /// Retrieves a list of values for a given app and language from the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="languageId">The ID of the language.</param>
    /// <returns>
    /// A tuple containing a list of value DTOs if successful; otherwise, an error base object.
    /// </returns>
    public async Task<(List<OutValueDto>?, ErrorBase?)> GetListAsync(long appId, long languageId, string? search, long? lastId)
    {
        // Check if the app exists
        var isAppExist = await _dbContext.App!
            .HasId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isAppExist)
        {
            return (null, new NotFoundError("App not found"));
        }

        // Check if the language exists
        var isLanguageExist = await _dbContext.Language!
            .HasId(languageId)
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isLanguageExist)
        {
            return (null, new ConflictError("Language not found"));
        }

        var dtos = await _dbContext.TranslationValue!
            .HasAppId(appId)
            .HasLanguageId(languageId)
            .NotDeleted()
            .SelectDto()
            .ToListAsync();

        return (dtos, null);
    }

    /// <summary>
    /// Retrieves a key with the specified ID for a given app from the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="id">The ID of the key.</param>
    /// <returns>
    /// A tuple containing the key DTO if found; otherwise, an error base object.
    /// </returns>
    public async Task<(OutValueDto?, ErrorBase?)> GetAsync(long appId, long languageId, long id)
    {
        // Check if the app exists
        var isAppExist = await _dbContext.App!
            .HasId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isAppExist)
        {
            return (null, new NotFoundError("App not found"));
        }

        // Check if the language exists
        var isLanguageExist = await _dbContext.Language!
            .HasId(languageId)
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isLanguageExist)
        {
            return (null, new ConflictError("Language not found"));
        }

        var dto = await _dbContext.TranslationValue!
            .HasId(id)
            .HasAppId(appId)
            .HasLanguageId(languageId)
            .NotDeleted()
            .SelectDto()
            .FirstOrDefaultAsync();

        if (dto == null)
        {
            return (null, new NotFoundError("Value not found"));
        }

        return (dto, null);
    }

    /// <summary>
    /// Adds a new value to the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="languageId">The ID of the language.</param>
    /// <param name="dto">The input value DTO.</param>
    /// <returns>
    /// A tuple containing the output value DTO, the validation result, and the error base.
    /// </returns>
    public async Task<(OutValueDto?, ValidationResult?, ErrorBase?)> AddAsync(long appId, long languageId, InValueDto dto)
    {
        // Check if the app exists
        var isAppExist = await _dbContext.App!
            .HasId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isAppExist)
        {
            return (null, null, new NotFoundError("App not found"));
        }

        // Check if the language exists
        var isLanguageExist = await _dbContext.Language!
            .HasId(languageId)
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isLanguageExist)
        {
            return (null, null, new ConflictError("Language not found"));
        }

        // Check if the key exists
        var isKeyExist = await _dbContext.TranslationKey!
            .HasId(dto.KeyId.GetValueOrDefault())
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isKeyExist)
        {
            return (null, null, new ConflictError("Key not found"));
        }

        // Create a new value with the provided information
        var translationValue = new TranslationValue
        {
            Id = _idGenerator.CreateId(),
            TranslationKeyId = dto.KeyId.GetValueOrDefault(),
            LanguageId = languageId,
            Value = dto.Value.EmptyToNull(),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Add the value to the database and save changes
        await _dbContext.TranslationValue!.AddAsync(translationValue);
        await _dbContext.SaveChangesAsync();

        // Retrieve the added value
        var (outDto, outDtoError) = await GetAsync(appId, languageId, translationValue.Id.GetValueOrDefault());
        return (outDto, null, outDtoError);
    }

    /// <summary>
    /// Updates a value with the specified ID for a given app, language, and key in the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="languageId">The ID of the language.</param>
    /// <param name="id">The ID of the value.</param>
    /// <param name="dto">The updated value DTO.</param>
    /// <returns>
    /// A tuple containing the updated value DTO if successful; otherwise, an error base object.
    /// </returns>
    public async Task<(OutValueDto?, ValidationResult?, ErrorBase?)> UpdateAsync(long appId, long languageId, long id, InValueDto dto)
    {
        // Check if the app exists
        var isAppExist = await _dbContext.App!
            .HasId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isAppExist)
        {
            return (null, null, new NotFoundError("App not found"));
        }

        // Check if the language exists
        var isLanguageExist = await _dbContext.Language!
            .HasId(languageId)
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isLanguageExist)
        {
            return (null, null, new ConflictError("Language not found"));
        }

        // Check if the key exists
        var isKeyExist = await _dbContext.TranslationKey!
            .HasId(dto.KeyId.GetValueOrDefault())
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isKeyExist)
        {
            return (null, null, new ConflictError("Key not found"));
        }

        // Get value by ID
        var translationValue = await _dbContext.TranslationValue!
            .AsTracking()
            .HasId(id)
            .HasAppId(appId)
            .HasLanguageId(languageId)
            .NotDeleted()
            .FirstOrDefaultAsync();

        if (translationValue == null)
        {
            return (null, null, new ConflictError("Value not found"));
        }

        translationValue.Value = dto.Value;
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(appId, languageId, id);
        return (outDto, null, outDtoError);
    }

    /// <summary>
    /// Deletes a value from the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="languageId">The ID of the language.</param>
    /// <param name="id">The ID of the key to delete.</param>
    /// <returns>
    /// An error base object if an error occurs; otherwise, null.
    /// </returns>
    public async Task<ErrorBase?> DeleteAsync(long appId, long languageId, long id)
    {
        // Check if the app exists
        var isAppExist = await _dbContext.App!
            .HasId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isAppExist)
        {
            return new NotFoundError("App not found");
        }

        // Check if the language exists
        var isLanguageExist = await _dbContext.Language!
            .HasId(languageId)
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isLanguageExist)
        {
            return new ConflictError("Language not found");
        }

        // Delete the key
        var translationValue = await _dbContext.TranslationValue!
            .AsTracking()
            .HasId(id)
            .HasAppId(appId)
            .HasLanguageId(languageId)
            .NotDeleted()
            .FirstOrDefaultAsync();

        if (translationValue == null)
        {
            return new NotFoundError("Value not found");
        }

        translationValue.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Save changes
        await _dbContext.SaveChangesAsync();
        return null;
    }
}