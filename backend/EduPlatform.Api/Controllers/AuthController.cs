using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduPlatform.Api.Data;
using EduPlatform.Api.Models; // Your User model
using System.ComponentModel.DataAnnotations; // Added for DTO attributes
// It's good practice to use DTOs (Data Transfer Objects) for API requests/responses
// For example, UserForRegistrationDto, UserForLoginDto, TokenDto

// Placeholder DTOs (ideally in a separate DTOs folder/namespace)
public class UserForRegistrationDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string Role { get; set; } = string.Empty; // "Student" or "Instructor"
}

public class UserForLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public UserDetailsDto User { get; set; } = new UserDetailsDto();
}

public class UserDetailsDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserForRegistrationDto registrationDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == registrationDto.Email))
        {
            return BadRequest("Email already exists.");
        }

        // In a real app, use a robust password hashing library like BCrypt.Net
        // Ensure BCrypt.Net-Next is added as a NuGet package
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = registrationDto.Name,
            Email = registrationDto.Email,
            PasswordHash = passwordHash,
            Role = registrationDto.Role // Validate role if necessary (e.g., only "Student" or "Instructor")
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Optionally log the user in immediately and return a token, or just return success
        // For now, just returning a CreatedAtAction or Ok response
        var userDetails = new UserDetailsDto { UserId = user.UserId, Name = user.Name, Email = user.Email, Role = user.Role };
        return CreatedAtAction(nameof(Register), new { id = user.UserId }, userDetails); // Or Ok("User registered successfully")
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = GenerateJwtToken(user);
        var userDetails = new UserDetailsDto { UserId = user.UserId, Name = user.Name, Email = user.Email, Role = user.Role };

        return Ok(new TokenDto { Token = token, User = userDetails });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        // var jwtAudience = _configuration["Jwt:Audience"]; // Audience can also be validated if needed

        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) )
        {
            throw new InvalidOperationException("JWT Key or Issuer is not configured properly.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Subject (user ID)
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name), // Custom claim for user's full name
            new Claim(ClaimTypes.Role, user.Role), // Role claim
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID (unique token identifier)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            //audience: jwtAudience, // Specify audience if you have it configured and need it
            claims: claims,
            expires: DateTime.Now.AddMinutes(120), // Token expiration time (e.g., 2 hours)
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 