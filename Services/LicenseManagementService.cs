using Cryptlex;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.DTOs;
using Services.Interfaces;
using Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LicenseManagementService : ILicenseManagementService
    {
        private readonly LoggerService _logger;
        private readonly CryptlexAPIServices _cryptlexAPIServices;

        public LicenseManagementService()
        {
            _logger = new LoggerService();
            _cryptlexAPIServices = new CryptlexAPIServices();
        }

        public async Task<LicenseTemplate[]> GetAllLicenseTemplatesAsync()
        {
            try
            {
                string response = await _cryptlexAPIServices.GetRequest("license-templates");

                if (string.IsNullOrEmpty(response))
                {
                    _logger.LogInformation("Could not find License Templates.");
                    return Array.Empty<LicenseTemplate>();
                }

                return JsonConvert.DeserializeObject<LicenseTemplate[]>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the License Templates.");
                return null;
            }
        }

        public async Task<bool> CreateLicenseTemplateAsync(CreateLicenseTemplateDto licenseTemplateDto)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(licenseTemplateDto);
                string response = await _cryptlexAPIServices.CreateRequest(jsonData, "license-templates");

                if (string.IsNullOrEmpty(response))
                {
                    _logger.LogInformation("Could not Create Licence Template.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the Licence Template.");
                return false;
            }
        }

        public async Task<bool> DeletelicenseTemplateAsync(Guid licenseTemplateId)
        {
            try
            {
                bool isDeleted = await _cryptlexAPIServices.DeleteRequest($"license-templates/{licenseTemplateId}");

                if (!isDeleted)
                {
                    _logger.LogInformation($"Could not Delete License Temolate with id: {licenseTemplateId}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the License Temolate.");
                return false;
            }
        }

        public async Task<License> GenerateLicenseKeyAsync(CreateLicenseKeyDto createLicenseKeyDto)
        {
            try
            {         
                string jsonData = JsonConvert.SerializeObject(createLicenseKeyDto);
                string response = await _cryptlexAPIServices.CreateRequest(jsonData, "licenses");

                if (string.IsNullOrEmpty(response))
                {
                    _logger.LogInformation("Could not  generating the License Key.");
                    return null;
                }

                return JsonConvert.DeserializeObject<License>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the License Key.");
                return null;
            }
        }

        public void SetProductFiles()
        {
            try
            {
                string[] productDatFiles = Directory.GetFiles("Config/ProductDat");

                if (productDatFiles == null || productDatFiles.Length == 0)
                {
                    _logger.LogInformation("Could not find Product.dat Files.");
                    return;
                }

                foreach (var productDatFile in productDatFiles)
                {
                    LexActivator.SetProductFile(productDatFile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while setting Product.dat Files.");
            }
        }

        public string[] GetProductIds()
        {           
            try
            {
                string ProductIdsFilePath = Path.Combine("Config", "ProductId", "ProductId.json");

                var jsonString = File.ReadAllText(ProductIdsFilePath);
                var jsonObject = JObject.Parse(jsonString);
                return jsonObject["ProductIds"].ToObject<string[]>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting Product Ids.");
                return null;
            }
        }

        public int ActivateLicense(string licenseKey, string productId)
        {
            try
            {                
                LexActivator.SetProductId(productId, LexActivator.PermissionFlags.LA_USER);
                LexActivator.SetLicenseKey(licenseKey);

                return LexActivator.ActivateLicense();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Activating License");
                return -1;
            }
        }
    }
}
