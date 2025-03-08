using Newtonsoft.Json;
using Services.Interfaces;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LicenseManagementService : ILicenseManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly LoggerService _logger;
        private const string BaseUrl = "https://api.cryptlex.com/v3/";
        private readonly string _accessToken;

        public LicenseManagementService(string accessToken)
        {
            _httpClient = new HttpClient();
            _logger = new LoggerService();
            _accessToken = accessToken;

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        public async Task<LicenseTemplate[]> GetAllLicenseTemplatesAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_accessToken))
                {
                    throw new ArgumentNullException("ACCESS_TOKEN Token not found!");
                }

                HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}license-templates");
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(jsonString))
                {
                    _logger.LogInformation("Could not find License Templates");
                    return Array.Empty<LicenseTemplate>();
                }
                return JsonConvert.DeserializeObject<LicenseTemplate[]>(jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }
    }
}
