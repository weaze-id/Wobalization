using System.Reflection;
using System.Security.Cryptography;
using FluentValidation;
using Kern.AspNetCore.Endpoints.Extensions;
using Kern.AspNetCore.Extensions;
using Kern.AspNetCore.Response;
using Microsoft.IdentityModel.Tokens;
using Wobalization.Api.Endpoints;
using Wobalization.Api.Extensions;
using Wobalization.Api.Services.Implementations;
using Wobalization.Api.Services.Interfaces;
using KernAuthorization = Kern.AspNetCore.Authorization.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add HttpContextAccessor to enable access to the HttpContext
builder.Services.AddHttpContextAccessor();

// Add support for ProblemDetails responses
builder.Services.AddProblemDetails();

// Add the database context and connection string configuration
builder.Services.AddDatabaseContext(builder.Configuration, builder.Environment);

// Add IdGenerator for generating unique IDs
builder.Services.AddIdGenerator();

// Add Swagger documentation generation and UI
builder.Services.AddSwagger(builder.Configuration);

// Configure RouteHandlerOptions to throw on bad requests
builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true);

// Add validators from the "Shared" assembly
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Shared"));

// Register services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Add endpoints for the Authentication API
builder.Services.AddEndpoints<AuthenticationEndpoint>();

// Configure RSA private key for JWT token generation and signing
builder.Services.AddSingleton(services =>
{
    var privateKeyBytes = Convert.FromBase64String(builder.Configuration["Jwt:PrivateKey"]!);
    var rsa = new RSACryptoServiceProvider();
    rsa.ImportCspBlob(privateKeyBytes);

    return rsa;
});

// Configure the security key for token validation
builder.Services.AddSingleton<SecurityKey>(services =>
{
    var rsa = services.GetRequiredService<RSACryptoServiceProvider>();
    return new RsaSecurityKey(rsa);
});

// Add JWT-based authorization with the specified options
KernAuthorization.AddAuthorization(
    builder.Services,
    jwtOptions: (options, securityKey) =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = securityKey,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ClockSkew = TimeSpan.Zero
        }
);

var app = builder.Build();

// Redirect HTTP to HTTPS in production environment
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseSwaggerDocs();
}

// Global exception handling
app.UseExceptionHandler();

// Configure handling of status code pages
app.UseStatusCodePages();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints for the Authentication API
app.MapEndpoints();

// Fallback endpoint for handling unknown routes
app.MapFallback(() => JsonResponse.NotFound("API endpoint not found"));

app.Run();