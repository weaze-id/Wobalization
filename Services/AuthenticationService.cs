using FluentValidation;
using FluentValidation.Results;
using IdGen;
using Kern.Error;
using Microsoft.EntityFrameworkCore;
using Wobalization.Database.DatabaseContexts;
using Wobalization.Database.Models;
using Wobalization.Dtos.Authentication;
using Wobalization.Models;

namespace Wobalization.Services;

/// <summary>
/// Service responsible for authentication operations.
/// </summary>
public class AuthenticationService
{
    private readonly DatabaseContext _dbContext;
    private readonly IdentityService _identityService;
    private readonly IdGenerator _idGenerator;
    private readonly IValidator<InSignInDto> _loginValidator;
    private readonly IValidator<InSignUpDto> _registerValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="loginValidator">The validator for login DTO.</param>
    /// <param name="identityService">The identity service.</param>
    /// <param name="registerValidator">The validator for register DTO.</param>
    /// <param name="idGenerator">The ID generator.</param>
    public AuthenticationService(
        DatabaseContext dbContext,
        IValidator<InSignInDto> loginValidator,
        IdentityService identityService,
        IValidator<InSignUpDto> registerValidator,
        IdGenerator idGenerator)
    {
        _dbContext = dbContext;
        _loginValidator = loginValidator;
        _identityService = identityService;
        _registerValidator = registerValidator;
        _idGenerator = idGenerator;
    }

    /// <summary>
    /// Retrieves the status of the authentication service.
    /// </summary>
    /// <returns>An error if no user found, otherwise null.</returns>
    public async Task<ErrorBase?> GetStatusAsync()
    {
        var isAnyUserExist = await _dbContext.User!.AnyAsync();
        if (isAnyUserExist)
        {
            return null;
        }

        return new NotFoundError("No user found, app has not been initialized");
    }

    /// <summary>
    /// Performs user sign-in operation.
    /// </summary>
    /// <param name="dto">The login DTO.</param>
    /// <returns>
    /// A tuple containing the generated token, validation result, and error (if any).
    /// </returns>
    public async Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignInAsync(InSignInDto dto)
    {
        // Validate the login DTO
        var validationResult = _loginValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        // Retrieve user from the database based on the provided username and validate the password
        var user = await _dbContext.User!
            .Where(e => e.Username!.ToLower() == dto.Username!.ToLower())
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

        // Create an identity for the authenticated user and generate a token
        var identity = new IdentityModel
        {
            Id = user.Id
        };

        var token = await _identityService.SignInAsync(identity);
        return (new OutTokenDto { Token = token }, null, null);
    }

    /// <summary>
    /// Performs user sign-up operation.
    /// </summary>
    /// <param name="dto">The register DTO.</param>
    /// <returns>
    /// A tuple containing the generated token, validation result, and error (if any).
    /// </returns>
    public async Task<(OutTokenDto?, ValidationResult?, ErrorBase?)> SignUpAsync(InSignUpDto dto)
    {
        // Validate the register DTO
        var validationResult = _registerValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return (null, validationResult, null);
        }

        // Check if any user already exists in the database
        var isAnyUserExist = await _dbContext.User!.AnyAsync();
        if (isAnyUserExist)
        {
            return (null, null, new ConflictError("Can't create an account, app has been initialized"));
        }

        // Create a new user with the provided information
        var user = new User
        {
            Id = _idGenerator.CreateId(),
            FullName = dto.FullName,
            Username = dto.Username,
            Password = BC.HashPassword(dto.Password),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Add the user to the database and save changes
        await _dbContext.User!.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Create an identity for the registered user and generate a token
        var identity = new IdentityModel
        {
            Id = user.Id
        };

        var token = await _identityService.SignInAsync(identity);
        return (new OutTokenDto { Token = token }, null, null);
    }

    /// <summary>
    /// Performs user sign-out operation.
    /// </summary>
    /// <returns>An error if sign-out fails, otherwise null.</returns>
    public async Task<ErrorBase?> SignOutAsync()
    {
        return await _identityService.SignOutAsync();
    }
}