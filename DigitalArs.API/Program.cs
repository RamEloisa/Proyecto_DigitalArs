using System.Text.Json.Serialization; // JsonStringEnumConverter: enums como texto en Swagger
using DigitalArs.Application; // AddApplication (servicios que usan IUnitOfWork)
using DigitalArs.Infrastructure; // AddInfrastructure (DbContext + IUnitOfWork)
using Swashbuckle.AspNetCore.SwaggerUI; // DocExpansion y opciones de la UI

using DigitalArs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

//Authetication
using DigitalArs.Application.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers() // Descubre Controllers/ y los publica como endpoints
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Type: "Deposit" en vez de 0
    });

builder.Services.AddOpenApi(options => // Genera /openapi/v1.json (lo que consume Swagger UI)
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "DigitalArs API"; // Título de la UI
        document.Info.Version = "v1";
        document.Info.Description =
            "Billetera digital: roles, usuarios, cuentas y transacciones. " +
            "Ver los endpoints en Swagger no requiere migraciones. Ejecutar Try it out contra SQL Server si requiere la base creada.";
        return Task.CompletedTask;
    });
});

builder.Services.AddApplication(); // IRoleService, IUserService, etc. → IUnitOfWork
builder.Services.AddInfrastructure(builder.Configuration); // DbContext + IUnitOfWork Scoped

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

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)
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
    app.MapOpenApi(); // Expone GET /openapi/v1.json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "DigitalArs API v1"); // Spec que pinta la UI
        options.DocumentTitle = "DigitalArs API"; // Título de la pestaña del browser
        options.RoutePrefix = "swagger"; // UI en /swagger
        options.EnableTryItOutByDefault(); // Try it out abierto
        options.DisplayRequestDuration(); // Muestra ms de cada request
        options.DocExpansion(DocExpansion.List); // Tags abiertos, operaciones cerradas
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Enlaza Roles/Users/Accounts/Transactions

app.Run();
