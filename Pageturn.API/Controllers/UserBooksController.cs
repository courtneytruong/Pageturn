using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pageturn.Core.DTOs.UserBooks;
using Pageturn.Core.Interfaces;

namespace Pageturn.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserBooksController : ControllerBase
{
    private readonly IUserBookService _userBookService;

    public UserBooksController(IUserBookService userBookService)
    {
        _userBookService = userBookService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("User ID not found in token");
        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetLibrary()
    {
        var userId = GetUserId();
        var result = await _userBookService.GetLibraryAsync(userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] AddUserBookRequestDto request)
    {
        var userId = GetUserId();
        var result = await _userBookService.AddBookAsync(userId, request);
        return CreatedAtAction(nameof(GetLibrary), result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserBookStatusDto request)
    {
        var userId = GetUserId();
        var result = await _userBookService.UpdateStatusAsync(userId, id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveBook(Guid id)
    {
        var userId = GetUserId();
        await _userBookService.RemoveBookAsync(userId, id);
        return NoContent();
    }
}
