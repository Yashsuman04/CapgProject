using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EduPlatform.Api.Data;
using EduPlatform.Api.Models; // Assuming your DTOs might be here or a dedicated DTOs folder
// using EduPlatform.Api.Services; // For a potential token service

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.

// Configure DbContext with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add ASP.NET Core Identity or a custom user management if preferred
// For simplicity, if not using Identity, ensure you handle password hashing securely.

// Configure JWT Authentication
// Store JWT settings in appsettings.json
// Example appsettings.json section:
// "Jwt": {
//   "Key": "YourSuperSecretKeyThatIsLongAndComplex",
//   "Issuer": "yourdomain.com",
//   "Audience": "yourdomain.com"
// }
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new ArgumentNullException("JWT Key, Issuer or Audience is not configured in appsettings.json");
}

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Add Swagger for API documentation and testing

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", // Give your policy a name
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Your React app's URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // More detailed errors in development
}

app.UseHttpsRedirection();

app.UseRouting(); // Must be before UseCors, UseAuthentication, UseAuthorization

app.UseCors("AllowReactApp"); // Apply the CORS policy

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
