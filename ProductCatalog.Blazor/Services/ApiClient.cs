using System.Net;
using System.Net.Http.Json;

namespace ProductCatalog.Blazor.Services;

public sealed class ApiClient(HttpClient http)
{
    public async Task<T> GetAsync<T>(string url, CancellationToken ct = default)
    {
        var res = await http.GetAsync(url, ct);
        return await ReadOrThrow<T>(res, ct);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
    {
        var res = await http.PostAsJsonAsync(url, body, ct);
        return await ReadOrThrow<TResponse>(res, ct);
    }

    public async Task PutAsync<TRequest>(string url, TRequest body, CancellationToken ct = default)
    {
        var res = await http.PutAsJsonAsync(url, body, ct);
        await EnsureSuccessOrThrow(res, ct);
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var res = await http.DeleteAsync(url, ct);
        await EnsureSuccessOrThrow(res, ct);
    }

    private static async Task<T> ReadOrThrow<T>(HttpResponseMessage res, CancellationToken ct)
    {
        await EnsureSuccessOrThrow(res, ct);
        var data = await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        if (data is null) throw new ApiException("Respuesta vacía del servidor.");
        return data;
    }

    private static async Task EnsureSuccessOrThrow(HttpResponseMessage res, CancellationToken ct)
    {
        if (res.IsSuccessStatusCode) return;

        ProblemDetailsDto? pd = null;
        try
        {
            pd = await res.Content.ReadFromJsonAsync<ProblemDetailsDto>(cancellationToken: ct);
        }
        catch { }

        var msg = pd?.Title ?? "Error";
        var detail = pd?.Detail;

        Dictionary<string, string[]>? fieldErrors = null;
        if (pd?.Extensions is not null &&
            pd.Extensions.TryGetValue("errors", out var errorsObj) &&
            errorsObj is not null)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(errorsObj);
            fieldErrors = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(json);
        }

        throw new ApiException(
            message: msg,
            statusCode: (int)res.StatusCode,
            detail: detail,
            fieldErrors: fieldErrors);
    }
}

public sealed class ApiException : Exception
{
    public int StatusCode { get; }
    public string? Detail { get; }
    public Dictionary<string, string[]>? FieldErrors { get; }

    public ApiException(string message, int statusCode = 0, string? detail = null, Dictionary<string, string[]>? fieldErrors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Detail = detail;
        FieldErrors = fieldErrors;
    }
}

public sealed class ProblemDetailsDto
{
    public int? Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }
}
