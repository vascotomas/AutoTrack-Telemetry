using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientSDK
{
    public class Client
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public Client(HttpClient httpClient)
        {
            _httpClient = httpClient;
     
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
                .WaitAndRetryAsync(3, intento => TimeSpan.FromSeconds(Math.Pow(2, intento)));
        }

        public void SetAuthorizationToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> EnviarChasisAsync(string apiUrl, object payload)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.PostAsJsonAsync($"{apiUrl.TrimEnd('/')}/api/telemetry", payload));

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                string errorDetalle = await response.Content.ReadAsStringAsync();
                return (false, $"El servidor rechazó el envío ({(int)response.StatusCode}). Detalle: {errorDetalle}");
            }
            catch (Exception ex)
            {
                return (false, $"Fallo de conexión crítico tras 3 intentos. Detalle: {ex.Message}");
            }
        }

        public async Task<AuthResponse> LoginAsync(string authUrl, string username, string password, string clientSecret)
        {
            var requestBody = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", "3d6f9a1c-4f64-49f8-b8e5-0a7c4e97f017"},
                {"client_secret", clientSecret},
                {"username", username},
                {"password", password},
                {"scope", "AutoTelemetry"}
            };

            try
            {
                var response = await _httpClient.PostAsync($"{authUrl.TrimEnd('/')}/connect/token", new FormUrlEncodedContent(requestBody));

                if (response.IsSuccessStatusCode)
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonStr);

                    string token = doc.RootElement.GetProperty("access_token").GetString() ?? string.Empty;
                    return new AuthResponse { IsSuccess = true, AccessToken = token };
                }

                return new AuthResponse { IsSuccess = false, ErrorMessage = "Credenciales incorrectas o acceso denegado." };
            }
            catch (Exception ex)
            {
                return new AuthResponse { IsSuccess = false, ErrorMessage = $"Error de red: {ex.Message}" };
            }
        }
    }

    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}