using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProxyAPI.Data;
using ProxyAPI.Models;
using ProxyAPI.Repositories;
using ProxyAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURATION SETUP
// ============================================

// Bind JWT settings from appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Bind SMTP settings from appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// ============================================
// DATABASE SETUP (EF Core + MySQL)
// ============================================

// Configure Entity Framework Core with MySQL (Pomelo provider)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// ============================================
// DEPENDENCY INJECTION
// ============================================

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();

// Register Dapper repository (uses same connection string as EF Core)
builder.Services.AddScoped<IDapperRepository, DapperRepository>();

// ============================================
// JWT AUTHENTICATION SETUP
// ============================================

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? string.Empty))
    };
});

// ============================================
// AUTHORIZATION SETUP
// ============================================

builder.Services.AddAuthorization();

// ============================================
// CONTROLLERS & API SETUP
// ============================================

builder.Services.AddControllers();

// ============================================
// SWAGGER/OPENAPI SETUP (with JWT support)
// ============================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProxyAPI - JWT Authentication Demo",
        Version = "v1",
        Description = "ASP.NET Core Web API with JWT Authentication, EF Core, and Dapper"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================
// CORS SETUP (Optional - for frontend integration)
// ============================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================
// BUILD APPLICATION
// ============================================

var app = builder.Build();

// ============================================
// MIDDLEWARE PIPELINE
// ============================================

// Enable Swagger in all environments (you can restrict to Development only)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProxyAPI v1");
    c.RoutePrefix = string.Empty; // Swagger UI at root URL
});

// Enable CORS
app.UseCors("AllowAll");

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// ============================================
// RUN APPLICATION
// ============================================

Console.WriteLine("==============================================");
Console.WriteLine("ProxyAPI is running!");
Console.WriteLine("Swagger UI: https://localhost:{PORT}");
Console.WriteLine("==============================================");

app.Run();
