using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ClientSDK
{
    public class Client
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public Client(string apiUrl, string token)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
                .WaitAndRetryAsync(3, intento => TimeSpan.FromSeconds(Math.Pow(2, intento)));
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> EnviarChasisAsync(object payload)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.PostAsJsonAsync("api/telemetry", payload));

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
    }
}
