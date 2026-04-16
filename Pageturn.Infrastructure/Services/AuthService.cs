using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pageturn.Core.DTOs.Auth;
using Pageturn.Core.Entities;
using Pageturn.Core.Exceptions;
using Pageturn.Core.Interfaces;

namespace Pageturn.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check for existing email
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ConflictException("Email is already registered");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        // Generate JWT token
        var accessToken = GenerateAccessToken(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = string.Empty,
            Username = user.Username
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Retrieve user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        // Generate tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Update user with refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Username = user.Username
        };
    }

    public async Task<AuthResponseDto> RefreshAsync(string refreshToken)
    {
        // Find user by refresh token
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            throw new UnauthorizedException("Refresh token is invalid");
        }

        // Check if refresh token has expired
        if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token has expired");
        }

        // Generate new tokens
        var accessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        // Update user with new refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            Username = user.Username
        };
    }

    public async Task RevokeAsync(string refreshToken)
    {
        // Find user by refresh token
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            throw new UnauthorizedException("Refresh token is invalid");
        }

        // Clear refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        await _userRepository.UpdateAsync(user);
    }

    private string GenerateAccessToken(User user)
    {
        var jwtSecret = _configuration["JWT_SECRET"] ?? throw new InvalidOperationException("JWT_SECRET is not configured");
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("sub", user.Id.ToString()),
                new System.Security.Claims.Claim("email", user.Email),
                new System.Security.Claims.Claim("username", user.Username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
