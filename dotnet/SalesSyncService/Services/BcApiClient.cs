using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SalesSyncService.Config;
using SalesSyncService.Models;

namespace SalesSyncService.Services
{
    /// <summary>
    /// Client HTTP générique pour l'API BC Cloud
    /// Gère : GET, POST, PATCH, DELETE avec authentification Basic Auth
    /// </summary>
    public class BcApiClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly BcApiConfig _config;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public BcApiClient(BcApiConfig config)
        {
            _config = config;
            _http = new HttpClient();

            // Basic Auth : Username + WebServiceKey
            var credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{config.Username}:{config.WebServiceKey}"));
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);

            _http.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // ── GET liste (avec filtre OData optionnel) ──────────────────────
        public async Task<ODataResponse<T>> GetListAsync<T>(string endpoint, string odataFilter = null)
        {
            var url = $"{_config.BaseUrl}/{endpoint}";
            if (!string.IsNullOrEmpty(odataFilter))
                url += $"?$filter={Uri.EscapeDataString(odataFilter)}";

            var response = await _http.GetAsync(url);
            await EnsureSuccessAsync(response);

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ODataResponse<T>>(json, _jsonOptions);
        }

        // ── GET par ID (SystemId GUID) ───────────────────────────────────
        public async Task<T> GetByIdAsync<T>(string endpoint, string systemId)
        {
            var url = $"{_config.BaseUrl}/{endpoint}({systemId})";
            var response = await _http.GetAsync(url);
            await EnsureSuccessAsync(response);

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        // ── POST (créer) ─────────────────────────────────────────────────
        public async Task<T> PostAsync<T>(string endpoint, object payload)
        {
            var url = $"{_config.BaseUrl}/{endpoint}";
            var body = new StringContent(
                JsonSerializer.Serialize(payload, _jsonOptions),
                Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, body);
            await EnsureSuccessAsync(response);

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        // ── PATCH (mise à jour partielle) ────────────────────────────────
        /// <param name="etag">Valeur ETag récupérée lors du GET (si-match: *  accepté aussi)</param>
        public async Task<T> PatchAsync<T>(string endpoint, string systemId, object payload, string etag = "*")
        {
            var url = $"{_config.BaseUrl}/{endpoint}({systemId})";

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload, _jsonOptions),
                    Encoding.UTF8, "application/json")
            };

            // BC exige If-Match pour PATCH
            request.Headers.Add("If-Match", etag);

            var response = await _http.SendAsync(request);
            await EnsureSuccessAsync(response);

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        // ── DELETE ───────────────────────────────────────────────────────
        public async Task DeleteAsync(string endpoint, string systemId, string etag = "*")
        {
            var url = $"{_config.BaseUrl}/{endpoint}({systemId})";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Add("If-Match", etag);

            var response = await _http.SendAsync(request);
            await EnsureSuccessAsync(response);
        }

        // ── Helper ───────────────────────────────────────────────────────
        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"BC API Error {(int)response.StatusCode}: {error}");
            }
        }

        public void Dispose() => _http?.Dispose();
    }
}
