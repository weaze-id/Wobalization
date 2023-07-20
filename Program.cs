using System.Reflection;
using System.Security.Cryptography;
using FluentValidation;
using Kern.AspNetCore.Endpoints.Extensions;
using Kern.AspNetCore.Extensions;
using Kern.AspNetCore.Response;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Wobalization.Endpoints;
using Wobalization.Entities.DatabaseContexts;
using Wobalization.Extensions;
using Wobalization.Services;
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

// Add validators from the "Wobalization" assembly
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Wobalization"));

// Register services
builder.Services.AddScoped<AppService>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<KeyService>();
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<UserService>();

// Add endpoints for the Authentication API
builder.Services.AddEndpoints<AppEndpoint>();
builder.Services.AddEndpoints<AuthenticationEndpoint>();
builder.Services.AddEndpoints<KeyEndpoint>();
builder.Services.AddEndpoints<LanguageEndpoint>();
builder.Services.AddEndpoints<UserEndpoint>();

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
    builder =>
    {
        builder.AddJwtBearer();
        builder.AddCookie();
    },
    new[]
    {
        JwtBearerDefaults.AuthenticationScheme,
        CookieAuthenticationDefaults.AuthenticationScheme
    },
    context =>
    {
        string? authorizationHeader = context.Request.Headers[HeaderNames.Authorization];
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            return JwtBearerDefaults.AuthenticationScheme;
        }

        return CookieAuthenticationDefaults.AuthenticationScheme;
    }
);

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<SecurityKey>((options, securityKey) =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = securityKey,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services
    .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure(options =>
    {
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
        };
    });

// Setup cors policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5077")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Run database migration when app started
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await dbContext.Database.MigrateAsync();
}

// Redirect HTTP to HTTPS in production environment
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseSwaggerDocs(app, app.Environment);
}

// Global exception handling
app.UseExceptionHandler();

// Configure handling of status code pages
app.UseStatusCodePages();

// Enable cors middleware
app.UseCors();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints for the Authentication API
app.MapEndpoints();

// Fallback endpoint for handling unknown routes
app.MapFallback(() => JsonResponse.NotFound("API endpoint not found"));

app.Run();