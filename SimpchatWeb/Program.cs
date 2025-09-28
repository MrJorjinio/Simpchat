using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpchatWeb.Services.Auth;
using SimpchatWeb.Services.DataInserter;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.DataInserter;
using SimpchatWeb.Services.Settings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpchatWeb", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         { securitySchema, new[] { "Bearer" } }
     });
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IGroupDataInserter, DataInserter>();
builder.Services.AddScoped<IGroupDataInserter, DataInserter>();
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<AppSettings>>().Value
);

builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddDbContext<SimpchatDbContext>(options =>
{
    var appSettings = builder.Configuration.Get<AppSettings>();
    options.UseNpgsql(appSettings!.ConnectionStrings.Default);
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer((options) =>
    {
        var appSettings = builder.Configuration.Get<AppSettings>();
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appSettings!.JwtSettings.Issuer,
            ValidAudience = appSettings!.JwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings!.JwtSettings.Key))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateAsyncScope()){
    var dataInserter = scope.ServiceProvider.GetRequiredService<IGroupDataInserter>();
    dataInserter.InsertSysGroupPermissions();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
