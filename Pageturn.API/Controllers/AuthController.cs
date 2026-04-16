using Microsoft.AspNetCore.Mvc;
using Pageturn.Core.DTOs.Auth;
using Pageturn.Core.Interfaces;

namespace Pageturn.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        // Set refresh token cookie
        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized();
        }

        var result = await _authService.RefreshAsync(refreshToken);

        // Set new refresh token cookie
        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(result);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return NoContent();
        }

        await _authService.RevokeAsync(refreshToken);

        // Delete refresh token cookie
        Response.Cookies.Delete("refreshToken");

        return NoContent();
    }
}
