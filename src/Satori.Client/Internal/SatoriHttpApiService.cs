using System.Net.Http.Json;
using System.Text.Json;

namespace Satori.Client.Internal;

internal class SatoriHttpApiService : ISatoriApiService {
    private readonly HttpClient _http;

    private readonly SatoriClient _client;

    internal SatoriHttpApiService(Uri baseUri, string? token, SatoriClient client) {
        _http = new HttpClient { BaseAddress = baseUri };
        _client = client;

        if (token is not null) {
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }

    public async Task<TData> SendAsync<TData>(string endpoint, string platform, string selfId, object? body) {
        HttpRequestMessage request = new(HttpMethod.Post, endpoint);
        request.Headers.Add("X-Platform", platform);
        request.Headers.Add("X-Self-ID", selfId);
        request.Content = JsonContent.Create(body);

        HttpResponseMessage response = await _http.SendAsync(request);

        try {
            return (await response.Content.ReadFromJsonAsync<TData>(SatoriClient.JsonOptions))!;
        } catch (JsonException) {
            _client.Log(LogLevel.Critical, $"Non-JSON Response received! Content: {await response.Content.ReadAsStringAsync()}");

            throw;
        }
    }
}
