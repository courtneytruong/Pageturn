using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Pageturn.Core.DTOs.Books;
using System.Text.Json;

namespace Pageturn.Infrastructure.Services;

public class GoogleBooksService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public GoogleBooksService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<List<BookSearchResultDto>> SearchBooksAsync(string query)
    {
        var httpClient = _httpClientFactory.CreateClient("GoogleBooks");
        var apiKey = _configuration["GOOGLE_BOOKS_API_KEY"] ?? "";

        var requestUrl = $"volumes?q={Uri.EscapeDataString(query)}&key={apiKey}";
        var response = await httpClient.GetAsync(requestUrl);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);
        var root = jsonDoc.RootElement;

        var results = new List<BookSearchResultDto>();

        if (root.TryGetProperty("items", out var items))
        {
            foreach (var item in items.EnumerateArray())
            {
                try
                {
                    var volumeInfo = item.GetProperty("volumeInfo");

                    var title = volumeInfo.TryGetProperty("title", out var titleProp)
                        ? titleProp.GetString() ?? ""
                        : "";

                    var author = "";
                    if (volumeInfo.TryGetProperty("authors", out var authors) && authors.ValueKind == JsonValueKind.Array)
                    {
                        var firstAuthor = authors.EnumerateArray().FirstOrDefault();
                        if (firstAuthor.ValueKind == JsonValueKind.String)
                        {
                            author = firstAuthor.GetString() ?? "";
                        }
                    }

                    var coverImageUrl = "";
                    if (volumeInfo.TryGetProperty("imageLinks", out var imageLinks))
                    {
                        if (imageLinks.TryGetProperty("thumbnail", out var thumbnail))
                        {
                            coverImageUrl = thumbnail.GetString() ?? "";
                        }
                    }

                    var externalId = item.TryGetProperty("id", out var idProp)
                        ? idProp.GetString() ?? ""
                        : "";

                    results.Add(new BookSearchResultDto
                    {
                        ExternalId = externalId,
                        Title = title,
                        Author = author,
                        CoverImageUrl = coverImageUrl
                    });
                }
                catch
                {
                    // Skip malformed items
                    continue;
                }
            }
        }

        return results;
    }
}
