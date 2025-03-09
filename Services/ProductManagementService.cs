using Newtonsoft.Json;
using Services.DTOs;
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
    public class ProductManagementService : IProductManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly LoggerService _logger;
        private const string BaseUrl = "https://api.cryptlex.com/v3/";
        private readonly string _accessToken;
        public ProductManagementService(string accessToken)
        {
            _httpClient = new HttpClient();
            _logger = new LoggerService();
            _accessToken = accessToken;            

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        public async Task<Product[]> GetProductsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_accessToken))
                {
                    throw new ArgumentNullException("ACCESS_TOKEN Token not found!");
                }

                HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}products");
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(jsonString)) 
                {
                    _logger.LogInformation("Could not find products");
                    return Array.Empty<Product>();
                }
                return JsonConvert.DeserializeObject<Product[]>(jsonString);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public async Task<bool> CreateProductAsync(CreateProductDto productDto)
        {
            try
            {
                if (string.IsNullOrEmpty(_accessToken))
                {
                    throw new ArgumentNullException("ACCESS_TOKEN Token not found!");
                }

                string jsonData = JsonConvert.SerializeObject(productDto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}products", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
            
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            try
            {
                if (string.IsNullOrEmpty(_accessToken))
                {
                    throw new ArgumentNullException("ACCESS_TOKEN Token not found!");
                }                

                HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}products/{productId}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
    }
}
