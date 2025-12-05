using Microsoft.AspNetCore.Builder;
using Simpchat.Application;

using Simpchat.Infrastructure;
using Simpchat.Infrastructure.Persistence.Extentions;
using Simpchat.Infrastructure.Persistence.Interfaces;
using Simpchat.Shared;
using Simpchat.Web;
using Simpchat.Web.Middlewares;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services
    .AddWeb(builder.Configuration)
    .AddShared(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

using (var scope = app.Services.CreateAsyncScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();