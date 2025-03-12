using Services.Constants;
using Services.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CryptlexAPIServices : ICryptlexAPIServices
    {
        private readonly HttpClient _httpClient;
        private readonly LoggerService _logger;

        public CryptlexAPIServices()
        {
            _httpClient = new HttpClient();
            _logger = new LoggerService();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("ACCESS_TOKEN")}");
        }

        public async Task<string> CreateRequest(string data, string endpoint)
        {
            try
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await WaitAndRetryAsync(async () => await _httpClient.PostAsync(Url.BaseUrl + endpoint, content));
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the POST request.");
                return null;
            }
        }

        public async Task<bool> DeleteRequest(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await WaitAndRetryAsync(async () => await _httpClient.DeleteAsync(Url.BaseUrl + endpoint));
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the DELETE request.");
                return false;
            }
        }

        public async Task<string> GetRequest(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await WaitAndRetryAsync(async () => await _httpClient.GetAsync(Url.BaseUrl + endpoint));

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the GET request.");
                return null;
            }
        }

        private async Task<HttpResponseMessage> WaitAndRetryAsync(
         Func<Task<HttpResponseMessage>> sendRequest)
        {       
            try
            {
                CheckAccessToken();

                int maxRetries = 3;
                HttpResponseMessage response;

                do
                {
                    response = await sendRequest();

                    if (response.StatusCode != (HttpStatusCode)429)
                    {
                        break;
                    }

                    CheckRateLimit(response);
                    maxRetries--;
                }
                while (maxRetries > 0);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private void CheckAccessToken()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                throw new ArgumentNullException("ACCESS_TOKEN - Token not found!");
            }
        }

        private async void CheckRateLimit(HttpResponseMessage response)
        {
            if (response.Headers.Contains("X-Rate-Limit-Reset"))
            {
                string rateLimitReset = response.Headers.GetValues("X-Rate-Limit-Reset").FirstOrDefault();

                DateTime resetTime = DateTime.Parse(rateLimitReset, null, System.Globalization.DateTimeStyles.RoundtripKind);
                TimeSpan waitTime = resetTime - DateTime.UtcNow;

                await Task.Delay(waitTime);
            }
        }
    }
}
