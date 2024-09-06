using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Data.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration) // Read logging configuration from appsettings.json
  .Enrich.FromLogContext() // Enrich logs with additional context information
  .CreateLogger(); // Create the logger instance

builder.Logging.ClearProviders(); // Remove any existing logging providers
builder.Logging.AddSerilog(logger); // Add Serilog as the logging provider

// Add controller services
builder.Services.AddControllers();

// Configure the PostgreSQL DbContext with the connection string
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));

// Configure JWT authentication
builder.Services.AddAuthentication(opt => {
    // Set JWT Bearer as the default authentication scheme
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Token validation parameters for JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Validate the issuer of the token
        ValidateAudience = true, // Validate the audience of the token
        ValidateLifetime = true, // Ensure the token hasn't expired
        ValidateIssuerSigningKey = true, // Validate the signing key used to issue the token
        // Set the issuer and audience from the app settings
        ValidIssuer = builder.Configuration.GetSection("AppSettings")["LocalhostUrl"],
        ValidAudience = builder.Configuration.GetSection("AppSettings")["LocalhostUrl"],
        // Symmetric key for signing the token (ensure the key is stored securely)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings")["Secret"]))
    };
});

// Swagger/OpenAPI configuration for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add a security definition for JWT Bearer tokens
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, // Token will be passed in the Authorization header
        Description = "Please insert JWT with Bearer into field", // Description for Swagger UI
        Name = "Authorization", // Name of the header
        Type = SecuritySchemeType.ApiKey // Type of security scheme
    });
    // Apply the security requirement to all API endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer" // Use the "Bearer" scheme defined earlier
            }
        },
        new string[] { } // No specific scopes are required for this API
    }
  });
});

// Dependency Injection for services and repositories
builder.Services.AddScoped<IUserService, UserService>(); // User service interface mapped to its implementation
builder.Services.AddScoped<IUserRepository, UserRepository>(); // User repository interface mapped to its implementation
builder.Services.AddScoped<IUserTaskService, UserTaskService>(); // Task service interface
builder.Services.AddScoped<IUserTaskRepository, UserTaskRepository>(); // Task repository interface

// Add Authorization services
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply database migrations at runtime (this ensures the latest migrations are applied)
using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
await dbContext.Database.MigrateAsync(); // Apply pending migrations to the database

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI and JSON documentation for development
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

// Map the controllers to route HTTP requests to their corresponding actions
app.MapControllers();

app.Run(); // Run the web application
