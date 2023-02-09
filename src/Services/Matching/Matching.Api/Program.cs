using Carter;
using FluentValidation;
using Matching.Api.Extensions;
using Matching.Application;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddCache(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddCarter();
builder.Services.AddDomainFactories();
builder.Services.AddUtils();
builder.Services.AddAutoMapper(typeof(IApplication));
builder.Services.AddMediatR(typeof(IApplication));
builder.Services.AddValidatorsFromAssemblyContaining(typeof(IApplication));

// Add services to the container.

var app = builder.Build();
app.UseStaticFiles();
app.MapCarter();
app.Run();

public partial class Program { }
