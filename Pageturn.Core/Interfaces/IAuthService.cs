using Pageturn.Core.DTOs.Auth;

namespace Pageturn.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RefreshAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}
