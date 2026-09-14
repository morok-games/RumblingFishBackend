namespace RumblingFishBackend.Services
{
    public class GeoIpService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeoIpService> _logger;

        public GeoIpService(HttpClient httpClient, IConfiguration configuration, ILogger<GeoIpService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> GetCountryCodeAsync(string? ip)
        {
            var serviceUrl = _configuration["GeoIp:BaseUrl"];

            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new InvalidOperationException("GeoIp:BaseUrl is not configured.");
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<CountryResponse>($"{serviceUrl}{ip}");

                return response?.CountryCode;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get IP data");
                return null;
            }
        }
    }

    public class CountryResponse
    {
        public string? CountryCode { get; set; }
    }
}
