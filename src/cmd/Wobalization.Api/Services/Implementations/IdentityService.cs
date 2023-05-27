using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Kern.Error;
using Kern.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Wobalization.Api.Models;
using Wobalization.Api.Services.Interfaces;

namespace Wobalization.Api.Services.Implementations;

public class IdentityService : IIdentityService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SecurityKey _securityKey;

    private IdentityModel? _identity;

    /// <summary>
    /// Initializes a new instance of the IdentityService class.
    /// </summary>
    /// <param name="httpContextAccessor">The http context accessor for accessing http context for current request.</param>
    /// <param name="securityKey">The security key for jwt.</param>
    /// <param name="configuration">The app configuration.</param>
    public IdentityService(
        IHttpContextAccessor httpContextAccessor,
        SecurityKey securityKey,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _securityKey = securityKey;
        _configuration = configuration;
    }

    /// <summary>
    /// Get user identity from user claims.
    /// </summary>
    /// <returns>Identity model with user identity.</returns>
    public (IdentityModel?, ErrorBase?) Get()
    {
        if (_identity != null)
        {
            return (_identity, null);
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var claims = httpContext!.User.Claims;

        // Get and parse user claim from claim list
        var claimId = claims.FirstOrDefault(e => e.Type == "Id")?.Value;
        var id = LongUtils.Parse(claimId);

        if (id == null)
        {
            return (null, new AuthenticationError("Invalid credential"));
        }

        _identity = new IdentityModel { Id = id.GetValueOrDefault() };

        return (_identity, null);
    }

    /// <summary>
    /// Generate JWT and set cookie for authentication
    /// </summary>
    /// <param name="identity">Identity for authentication</param>
    /// <returns>Generated JWT.</returns>
    public async Task<string> SignInAsync(IdentityModel identity)
    {
        var claims = identity.ToClaims();

        // Setup jwt configuration.
        var signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.RsaSha256);
        var securityToken = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddSeconds(int.Parse(_configuration["Jwt:ExpiredIn"]!)),
            signingCredentials: signingCredentials
        );

        // Cookie authentication
        var httpContext = _httpContextAccessor.HttpContext!;
        var claimsPrincipal =
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        // Generate JWT and return it
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    /// <summary>
    /// Sign out current user
    /// </summary>
    /// <returns>Generated JWT.</returns>
    public async Task<ErrorBase?> SignOutAsync()
    {
        var (identity, identityError) = Get();
        if (identityError != null)
        {
            return identityError;
        }

        var httpContext = _httpContextAccessor.HttpContext!;
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return null;
    }
}