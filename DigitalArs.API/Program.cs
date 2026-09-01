using System.Text.Json.Serialization;
using DigitalArs.API.Filters;
using DigitalArs.Application;
using DigitalArs.Infrastructure;
using DigitalArs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using BCrypt.Net;

//Authetication
using DigitalArs.Application.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

string adminHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
string userHash = BCrypt.Net.BCrypt.HashPassword("User123!");
Console.WriteLine($"Admin123! -> {adminHash}");
Console.WriteLine($"User123!  -> {userHash}");

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<FluentValidationActionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
            ValidationErrorResponseFactory.FromModelState(context.ModelState);
    });

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "DigitalArs API";
        document.Info.Version = "v1";
        document.Info.Description =
            "Billetera digital: roles, usuarios, cuentas y transacciones. " +
            "Ver los endpoints en Swagger no requiere migraciones. Ejecutar Try it out contra SQL Server si requiere la base creada.";
        return Task.CompletedTask;
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddDbContext<DigitalArsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "La configuración JWT no está disponible.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key) //key vacia
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "DigitalArs API v1");
        options.DocumentTitle = "DigitalArs API";
        options.RoutePrefix = "swagger";
        options.EnableTryItOutByDefault();
        options.DisplayRequestDuration();
        options.DocExpansion(DocExpansion.List);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Enlaza Roles/Users/Accounts/Transactions

app.Run();
