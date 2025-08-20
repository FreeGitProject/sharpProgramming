using Microsoft.EntityFrameworkCore;
using Test.Scenario.Data;
using Test.Scenario.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connecctionstring = builder.Configuration.GetConnectionString("DataBase");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(connecctionstring)
);

builder.Services.AddScoped<IResumeTextExtractor, ResumeTextExtractor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

    app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
