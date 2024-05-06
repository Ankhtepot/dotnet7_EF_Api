global using dotnet_rpg.Services.CharacterService;
global using Microsoft.EntityFrameworkCore;
global using dotnet_rpg.Data;
global using dotnet_rpg.Services.FightService;
global using dotnet_rpg.Services.WeaponService;
global using Microsoft.OpenApi.Models;
using dotnet_rpg.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.Load($".env.{builder.Environment.EnvironmentName.ToLower()}");

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION"));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(configuration =>
{
    configuration.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = """Standard Authorization header using the Bearer scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });
    configuration.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IWeaponService, WeaponService>();
builder.Services.AddScoped<IFightService, FightService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();