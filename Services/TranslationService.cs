using Kern.Error;
using Microsoft.EntityFrameworkCore;
using Wobalization.Database.DatabaseContexts;
using Wobalization.Dtos.Translation;

namespace Wobalization.Services;

/// <summary>
/// Represents a TranslationService to get translation by locale.
/// </summary>
public class TranslationService
{
    private readonly DatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the TranslationService class with the specified dependencies.
    /// </summary>
    /// <param name="dbContext">The DatabaseContext instance.</param>
    public TranslationService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves a list of translations for a given app from the database.
    /// </summary>
    /// <param name="appKey">The key of the app.</param>
    /// <param name="locale">The translation locale.</param>
    /// <returns>
    /// A tuple containing a list of translation DTOs if successful; otherwise, an error base object.
    /// </returns>
    public async Task<(List<OutTranslationDto>?, ErrorBase?)> GetListAsync(Guid appKey, string locale)
    {
        var dtos = await _dbContext.TranslationKey!
            .Where(e => e.App!.Key == appKey &&
                        e.DeletedAt == null)
            .Select(e => new OutTranslationDto
            {
                Key = e.Key,
                Value = e.TranslationValues!
                    .Where(el => el.TranslationLanguage!.Locale == locale &&
                                 el.DeletedAt == null &&
                                 el.TranslationLanguage.DeletedAt == null)
                    .Select(el => el.Value)
                    .FirstOrDefault()
            })
            .ToListAsync();

        if (dtos.Count == 0)
        {
            return (null, new NotFoundError("Translation not found"));
        }

        return (dtos, null);
    }
}