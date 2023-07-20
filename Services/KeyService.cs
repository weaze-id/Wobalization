using FluentValidation;
using FluentValidation.Results;
using IdGen;
using Kern.Error;
using Kern.Extensions;
using Microsoft.EntityFrameworkCore;
using Wobalization.Dtos.Key;
using Wobalization.Entities;
using Wobalization.Entities.DatabaseContexts;
using Wobalization.Entities.Extensions;

namespace Wobalization.Services;

/// <summary>
/// Represents a KeyService that implements the IKeyService interface.
/// </summary>
public class KeyService
{
    private readonly DatabaseContext _dbContext;
    private readonly IdGenerator _idGenerator;
    private readonly IValidator<InKeyDto> _validator;

    /// <summary>
    /// Initializes a new instance of the KeyService class with the specified dependencies.
    /// </summary>
    /// <param name="dbContext">The DatabaseContext instance.</param>
    /// <param name="idGenerator">The IdGenerator instance.</param>
    /// <param name="validator">The IValidator of InKeyDto instance.</param>
    public KeyService(DatabaseContext dbContext, IdGenerator idGenerator, IValidator<InKeyDto> validator)
    {
        _dbContext = dbContext;
        _idGenerator = idGenerator;
        _validator = validator;
    }

    /// <summary>
    /// Retrieves a list of keys for a given app from the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <returns>
    /// A tuple containing a list of key DTOs if successful; otherwise, an error base object.
    /// </returns>
    public async Task<(List<OutKeyDto>?, ErrorBase?)> GetListAsync(long appId, string? search, long? lastId)
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

        var dtos = await _dbContext.TranslationKey!
            .HasAppId(appId)
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
    public async Task<(OutKeyDto?, ErrorBase?)> GetAsync(long appId, long id)
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

        var dto = await _dbContext.TranslationKey!
            .HasId(id)
            .HasAppId(appId)
            .NotDeleted()
            .SelectDto()
            .FirstOrDefaultAsync();

        if (dto == null)
        {
            return (null, new NotFoundError("Key not found"));
        }

        return (dto, null);
    }

    /// <summary>
    /// Adds a new key to the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="dto">The input key DTO.</param>
    /// <returns>
    /// A tuple containing the output key DTO, the validation result, and the error base.
    /// </returns>
    public async Task<(OutKeyDto?, ValidationResult?, ErrorBase?)> AddAsync(long appId, InKeyDto dto)
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

        // Check if the key already exists
        var isKeyExist = await _dbContext.TranslationKey!
            .HasAppId(appId)
            .HasKey(dto.Key!)
            .NotDeleted()
            .AnyAsync();

        if (isKeyExist)
        {
            return (null, null, new NotFoundError("Key already exists"));
        }

        // Create a new key with the provided information
        var translationKey = new TranslationKey
        {
            Id = _idGenerator.CreateId(),
            AppId = appId,
            Key = dto.Key.EmptyToNull(),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Add the key to the database and save changes
        await _dbContext.TranslationKey!.AddAsync(translationKey);
        await _dbContext.SaveChangesAsync();

        // Retrieve the added key
        var (outDto, outDtoError) = await GetAsync(appId, translationKey.Id.GetValueOrDefault());
        return (outDto, null, outDtoError);
    }

    /// <summary>
    /// Adds a new value to the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="keyId">The ID of the language.</param>
    /// <param name="dto">The input value DTO.</param>
    /// <returns>
    /// A tuple containing the output value DTO, the validation result, and the error base.
    /// </returns>
    public async Task<(ValidationResult?, ErrorBase?)> AddValueAsync(
        long appId,
        long keyId,
        InKeyValueDto dto)
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

        // Check if the key exists
        var isKeyExist = await _dbContext.TranslationKey!
            .HasId(keyId)
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isKeyExist)
        {
            return (null, new NotFoundError("Key not found"));
        }

        // Check if the language exists
        var isLanguageExist = await _dbContext.TranslationLanguage!
            .HasId(dto.LanguageId.GetValueOrDefault())
            .HasAppId(appId)
            .NotDeleted()
            .AnyAsync();

        if (isLanguageExist)
        {
            return (null, new NotFoundError("Language not found"));
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Get existing translation value, if exist delete it
        var existingTranslationValue = await _dbContext.TranslationValue!
            .AsTracking()
            .Where(e => e.TranslationKeyId == keyId &&
                        e.TranslationLanguageId == dto.LanguageId &&
                        e.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (existingTranslationValue != null)
        {
            existingTranslationValue.DeletedAt = now;
        }

        // Create a new value with the provided information
        var translationValue = new TranslationValue
        {
            Id = _idGenerator.CreateId(),
            TranslationKeyId = keyId,
            TranslationLanguageId = dto.LanguageId.GetValueOrDefault(),
            Value = dto.Value.EmptyToNull(),
            CreatedAt = now
        };

        // Add the value to the database and save changes
        await _dbContext.TranslationValue!.AddAsync(translationValue);
        await _dbContext.SaveChangesAsync();

        return (null, null);
    }

    /// <summary>
    /// Updates a key with the specified ID for a given app in the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="id">The ID of the key.</param>
    /// <param name="dto">The updated key DTO.</param>
    /// <returns>
    /// A tuple containing the updated key DTO if successful; otherwise, an error base object.
    /// </returns>
    public async Task<(OutKeyDto?, ValidationResult?, ErrorBase?)> UpdateAsync(long appId, long id, InKeyDto dto)
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

        // Check if the key already exists
        var isKeyExist = await _dbContext.TranslationKey!
            .ExceptId(id)
            .HasAppId(appId)
            .HasKey(dto.Key!)
            .NotDeleted()
            .AnyAsync();

        if (isKeyExist)
        {
            return (null, null, new NotFoundError("Key already exists"));
        }

        // Get key by ID
        var translationKey = await _dbContext.TranslationKey!
            .AsTracking()
            .HasId(id)
            .HasAppId(appId)
            .NotDeleted()
            .FirstOrDefaultAsync();

        if (translationKey == null)
        {
            return (null, null, new ConflictError("Key not found"));
        }

        translationKey.Key = dto.Key;
        await _dbContext.SaveChangesAsync();

        var (outDto, outDtoError) = await GetAsync(appId, id);
        return (outDto, null, outDtoError);
    }

    /// <summary>
    /// Deletes a key from the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="id">The ID of the key to delete.</param>
    /// <returns>
    /// An error base object if an error occurs; otherwise, null.
    /// </returns>
    public async Task<ErrorBase?> DeleteAsync(long appId, long id)
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

        // Delete the key
        var translationKey = await _dbContext.TranslationKey!
            .AsTracking()
            .HasId(id)
            .HasAppId(appId)
            .NotDeleted()
            .FirstOrDefaultAsync();

        if (translationKey == null)
        {
            return new NotFoundError("Key not found");
        }

        translationKey.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Save changes
        await _dbContext.SaveChangesAsync();
        return null;
    }
}