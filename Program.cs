using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChessTournamentCalendarBackend.API.Data;
using ChessTournamentCalendarBackend.API.Repositories.Interfaces;
using ChessTournamentCalendarBackend.API.Repositories.Impl;
using ChessTournamentCalendarBackend.API.Services.Interfaces;
using ChessTournamentCalendarBackend.API.Services.Impl;
using ChessTournamentCalendarBackend.API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Asp.Versioning;
using ChessTournamentCalendarBackend.API.DTOs;
using TournamentApp.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

//
// 📌 CORS CONFIGURATION (Required for frontend to connect to backend)
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:9988", "http://localhost:3000") // Add frontend ports here
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();

    });
});

//
// 1️⃣ CONTROLLERS
//
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//
// 2️⃣ SWAGGER
//
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token. Example: Bearer <JWT_TOKEN>"
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

//
// 3️⃣ DATABASE (POSTGRESQL)
//
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//
// 4️⃣ AUTOMAPPER
//
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//
// 5️⃣ JWT SETTINGS
//
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

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

        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key)
        )
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));
});

//
// 6️⃣ VERSIONING
//
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});

//
// 7️⃣ DEPENDENCY INJECTION
//

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

//
// 8️⃣ MIDDLEWARE PIPELINE
//

app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//
// Authentication middleware order important
//
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    response.ContentType = "application/json";

    
    var jsonOptions = new JsonSerializerOptions 
    { 
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
    };

    if (response.StatusCode == 401)
    {
        await response.WriteAsync(JsonSerializer.Serialize(
            ApiResponse<object>.ErrorResponse("Please login first to access this resource."), jsonOptions
        ));
    }
    else if (response.StatusCode == 403)
    {
        await response.WriteAsync(JsonSerializer.Serialize(
            ApiResponse<object>.ErrorResponse("You do not have permission to perform this action."), jsonOptions
        ));
    }
});
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();