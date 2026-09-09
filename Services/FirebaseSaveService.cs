using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using RumblingFishBackend.Models.Firebase;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RumblingFishBackend.Services
{
    public class FirebaseSaveService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly GoogleCredential _credential;

        public FirebaseSaveService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _credential = FirebaseApp.DefaultInstance.Options.Credential;
        }

        public async Task<FirebaseSaveData?> GetSaveDataAsync(string firebaseUid)
        {
            var databaseUrl = _configuration["Firebase:DatabaseUrl"];

            if (string.IsNullOrWhiteSpace(databaseUrl))
            {
                throw new InvalidOperationException("Firebase:DatabaseUrl is not configured.");
            }

            // Get access token service account
            var accessToken = await _credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            var url = $"{databaseUrl}/users/{firebaseUid}/Save.json";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Firebase returned {response.StatusCode}: {json}");
            }

            if (string.IsNullOrWhiteSpace(json) || json == "null")
            {
                return null;
            }

            return JsonSerializer.Deserialize<FirebaseSaveData>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
