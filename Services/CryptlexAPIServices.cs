using Newtonsoft.Json;
using Services.Constants;
using Services.Interfaces;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
                CheckAccessToken();

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(Url.BaseUrl + endpoint, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public async Task<bool> DeleteRequest(string endpoint)
        {
            try
            {
                CheckAccessToken();

                HttpResponseMessage response = await _httpClient.DeleteAsync(Url.BaseUrl + endpoint);
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<string> GetRequest(string endpoint)
        {
            try
            {
                CheckAccessToken();

                HttpResponseMessage response = await _httpClient.GetAsync(Url.BaseUrl + endpoint);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
    }
}
