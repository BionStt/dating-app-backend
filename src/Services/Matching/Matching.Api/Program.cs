using Application.Shared.Middlewares;
using Carter;
using FluentValidation;
using Matching.Api.Extensions;
using Matching.Application;
using Matching.Application.Infrastructure.Persistence;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddCache(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddCarter();
builder.Services.AddDomainFactories();
builder.Services.AddPipes();
builder.Services.AddUtils();
builder.Services.AddAutoMapper(typeof(IApplication));
builder.Services.AddMediatR(typeof(IApplication));
builder.Services.AddValidatorsFromAssemblyContaining(typeof(IApplication));
builder.Services.AddSwagger();
// Add services to the container.


var app = builder.Build();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseStaticFiles();
app.UseOpenApi();
app.UseSwaggerUi3();
app.MapCarter();
app.MigrateDatabase<MatchingDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<MatchingDbContext>>();
    logger?.LogInformation("Seed Successful");

}).Run();

public partial class Program { }
