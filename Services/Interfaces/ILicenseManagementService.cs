using Services.DTOs;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    internal interface ILicenseManagementService
    {
        Task<LicenseTemplate[]> GetAllLicenseTemplatesAsync();
        Task<bool> CreateLicenseTemplateAsync(CreateLicenseTemplateDto licenseTemplateDto);
        Task<bool> DeletelicenseTemplateAsync(Guid licenseTemplateId);
        Task<License> GenerateLicenseKeyAsync(CreateLicenseKeyDto createLicenseKeyDto);
    }
}
