using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Kern.Error;
using Kern.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Wobalization.Database.DatabaseContexts;
using Wobalization.Models;

namespace Wobalization.Services;

public class IdentityService
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SecurityKey _securityKey;

    /// <summary>
    /// Initializes a new instance of the IdentityService class.
    /// </summary>
    /// <param name="httpContextAccessor">The http context accessor for accessing http context for current request.</param>
    /// <param name="securityKey">The security key for jwt.</param>
    /// <param name="configuration">The app configuration.</param>
    /// <param name="dbContext">The database context for accessing database.</param>
    public IdentityService(
        IHttpContextAccessor httpContextAccessor,
        SecurityKey securityKey,
        IConfiguration configuration,
        DatabaseContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _securityKey = securityKey;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public long? UserId { get; set; }

    public ErrorBase? ParseClaims(IEnumerable<Claim> claims)
    {
        var claimId = claims.FirstOrDefault(e => e.Type == "Id")?.Value;

        UserId = LongUtils.Parse(claimId);

        if (UserId == null)
        {
            return new AuthenticationError("Invalid credential");
        }

        return null;
    }

    /// <summary>
    /// Generate JWT and set cookie for authentication
    /// </summary>
    /// <param name="identity">Identity for authentication</param>
    /// <returns>Generated JWT (access token and refresh token).</returns>
    public async Task<(string, string)> SignInAsync(IdentityModel identity)
    {
        var claims = identity.ToClaims();

        // Setup jwt configuration.
        var signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.RsaSha256);

        var accessToken = new JwtSecurityToken(
            _configuration["Jwt:AccessToken:Issuer"],
            _configuration["Jwt:AccessToken:Audience"],
            claims,
            expires: DateTime.UtcNow.AddSeconds(int.Parse(_configuration["Jwt:AccessToken:ExpiredIn"]!)),
            signingCredentials: signingCredentials
        );

        var refreshToken = new JwtSecurityToken(
            _configuration["Jwt:RefreshToken:Issuer"],
            _configuration["Jwt:RefreshToken:Audience"],
            claims,
            expires: DateTime.UtcNow.AddSeconds(int.Parse(_configuration["Jwt:RefreshToken:ExpiredIn"]!)),
            signingCredentials: signingCredentials
        );

        // Cookie authentication
        var httpContext = _httpContextAccessor.HttpContext!;
        var claimsPrincipal =
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        // Generate JWT and return it
        return (new JwtSecurityTokenHandler().WriteToken(accessToken),
            new JwtSecurityTokenHandler().WriteToken(refreshToken));
    }

    /// <summary>
    /// Sign out current user
    /// </summary>
    /// <returns>Generated JWT.</returns>
    public async Task<ErrorBase?> SignOutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext!;
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return null;
    }
}