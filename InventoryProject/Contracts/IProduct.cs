using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Contracts
{
    public interface IProduct
    {
        Task<ICollection<Product>> GetAllProductsAsync();
        Task<DetailProductResponse> GetProductByIdAsync(int id);
        Task<GeneralResponse> CreateProductAsync(ProductDTO productDTO, HttpContext httpContext);
        Task<GeneralResponse> UpdateProductAsync(int id, UpdateProductDTO productDTO, HttpContext httpContext);
        Task<GeneralResponse> DeleteProductAsync(int id);
    }
}