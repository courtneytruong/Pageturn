using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pageturn.Core.Interfaces;

namespace Pageturn.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query parameter 'q' is required");

        var result = await _bookService.SearchBooksAsync(q);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(Guid id)
    {
        var result = await _bookService.GetBookAsync(id);
        return Ok(result);
    }
}
