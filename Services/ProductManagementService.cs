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
        private readonly LoggerService _logger;
        private readonly CryptlexAPIServices _cryptlexAPIServices;

        public ProductManagementService()
        {
            _logger = new LoggerService();
            _cryptlexAPIServices = new CryptlexAPIServices();
        }

        public async Task<Product[]> GetProductsAsync()
        {
            try
            {
                string response = await _cryptlexAPIServices.GetRequest("products");

                if (string.IsNullOrEmpty(response)) 
                {
                    _logger.LogInformation("Could not find products");
                    return Array.Empty<Product>();
                }
                return JsonConvert.DeserializeObject<Product[]>(response);
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
                string jsonData = JsonConvert.SerializeObject(productDto);
                string response = await _cryptlexAPIServices.CreateRequest(jsonData, "products");

                if (string.IsNullOrEmpty(response))
                {
                    _logger.LogInformation("Could not Create product");
                    return false;
                }

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
                bool isDeleted = await _cryptlexAPIServices.DeleteRequest($"products/{productId}");

                if (!isDeleted)
                {
                    _logger.LogInformation($"Could not Delete product with id: {productId}");
                    return false;
                }

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
