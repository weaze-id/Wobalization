using System.Reflection;
using System.Security.Cryptography;
using FluentValidation;
using Kern.AspNetCore.Extensions;
using Kern.AspNetCore.Response;
using Microsoft.IdentityModel.Tokens;
using Wobalization.Api.Extensions;
using KernAuthorization = Kern.AspNetCore.Authorization.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

builder.Services.AddDatabaseContext(builder.Configuration, builder.Environment);
builder.Services.AddIdGenerator();
builder.Services.AddSwagger(builder.Configuration);
builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true);

builder.Services.AddValidatorsFromAssembly(Assembly.Load("Shared"));

builder.Services.AddSingleton(services =>
{
    var privateKeyBytes = Convert.FromBase64String(builder.Configuration["Jwt:PrivateKey"]!);
    var rsa = new RSACryptoServiceProvider();
    rsa.ImportCspBlob(privateKeyBytes);

    return rsa;
});

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

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseSwaggerDocs();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallback(() => JsonResponse.NotFound("API endpoint not found"));

app.Run();