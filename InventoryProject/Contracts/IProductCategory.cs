using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Contracts
{
    public interface IProductCategory
    {
        Task<ICollection<ProductCategory>> GetAllProductCategoriesAsync();
        Task<DetailProductCategoryResponse> GetProductCategoryByIdAsync(int id);
        Task<GeneralResponse> CreateProductCategoryAsync(ProductCategoryDTO productCategoryDTO, HttpContext httpContext);
        Task<GeneralResponse> UpdateProductCategoryAsync(int id, UpdateProductCategoryDTO productCategoryDTO, HttpContext httpContext);
        Task<GeneralResponse> DeleteProductCategoryAsync(int id);

    }
}