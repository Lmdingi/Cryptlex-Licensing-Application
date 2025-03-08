using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICryptlexService
    {
        Task<string> GetProductsAsync();
        Task<string> CreateLicenseAsync(string productId);
    }
}
