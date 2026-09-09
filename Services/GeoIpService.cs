namespace RumblingFishBackend.Services
{
    public class GeoIpService
    {
        private readonly HttpClient _httpClient;

        public GeoIpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetCountryCodeAsync(string? ip)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<CountryResponse>($"https://countries.dev/ip/{ip}");

                return response?.CountryCode;
            }
            catch
            {
                return null;
            }
        }
    }

    public class CountryResponse
    {
        public string? CountryCode { get; set; }
    }
}
