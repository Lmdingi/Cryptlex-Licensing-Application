using Services.DTOs;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductManagementService
    {
        Task<Product[]> GetProductsAsync();
        Task<bool> CreateProductAsync(CreateProductDto productDto);
        Task<bool> DeleteProductAsync(Guid productId);
    }
}
