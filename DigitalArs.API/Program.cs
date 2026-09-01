using System.Text.Json.Serialization;
using DigitalArs.API.Filters;
using DigitalArs.Application;
using DigitalArs.Infrastructure;
using DigitalArs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

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
app.MapControllers();

app.Run();
