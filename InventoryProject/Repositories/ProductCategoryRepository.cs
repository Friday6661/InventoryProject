using System.Security.Claims;
using InventoryProject.Contracts;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static InventoryProject.Models.DTOs.ServiceResponse;


namespace InventoryProject.Repositories
{
    public class ProductCategoryRepository : IProductCategory
    {
        private readonly InventoryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductCategoryRepository(InventoryDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<GeneralResponse> CreateProductCategoryAsync(ProductCategoryDTO productCategoryDTO, HttpContext httpContext)
        {
            // throw new NotImplementedException();
            // var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new GeneralResponse(false, "User not authenticated");
            
            if (productCategoryDTO is null) return new GeneralResponse(false, "Model is Empty");
            var productCategory = new ProductCategory()
            {
                Name = productCategoryDTO.Name,
                Description = productCategoryDTO.Description,
                // CreatedBy.Id = userId
            };
            productCategory.CreatedBy = new ApplicationUser
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };

            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();

            return new GeneralResponse(true, "New Product Category Created successfully");
        }

        public async Task<GeneralResponse> DeleteProductCategoryAsync(int id)
        {
            var productCategory = _context.ProductCategories.Where(pc => pc.Id == id).FirstOrDefault();
            if (productCategory is null) return new GeneralResponse(false, "Product Category not found");

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();

            return new GeneralResponse(true, "Product Category deleted successfully");
        }

        public async Task<ICollection<ProductCategory>> GetAllProductCategoriesAsync()
        {
            return await _context.ProductCategories.ToListAsync();

        }

        public async Task<DetailProductCategoryResponse> GetProductCategoryByIdAsync(int id)
        {
            var productCategory = await _context.ProductCategories.Where(pc => pc.Id == id)
                                                                .Include(pc => pc.CreatedBy)
                                                                .FirstOrDefaultAsync();
            if (productCategory is null) return new DetailProductCategoryResponse(false, null!, "Product Category not found");
            var roles = await _userManager.GetRolesAsync(productCategory.CreatedBy);
            var userRole = roles.FirstOrDefault();
            var productCategoryResponse = new DetailProductCategoryDTO()
            {
                Id = productCategory.Id,
                Name = productCategory.Name,
                Description = productCategory.Description,
                IsActive = productCategory.IsActive,
                CreatedAt = productCategory.CreatedAt,
                CreatedBy = new UserProfileDTO
                {
                    UserName = productCategory.CreatedBy.UserName,
                    FirstName = productCategory.CreatedBy.FirstName,
                    LastName = productCategory.CreatedBy.LastName,
                    Email = productCategory.CreatedBy.Email,
                    Role = userRole ?? "No Role Assigned"
                }
            };

            return new DetailProductCategoryResponse(true, productCategoryResponse, "Load detail product category successfully");

        }

        public async Task<GeneralResponse> UpdateProductCategoryAsync(int id, UpdateProductCategoryDTO productCategoryDTO, HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            // var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user is null) return new GeneralResponse(false, "User not authenticated");

            var productCategory = await _context.ProductCategories.Where(pc => pc.Id == id).FirstOrDefaultAsync();
            if (productCategory is null) return new GeneralResponse(false, "Product Category not found");

            productCategory.Name = productCategoryDTO.Name;
            productCategory.Description = productCategoryDTO.Description;
            productCategory.IsActive = productCategoryDTO.IsActive;
            productCategory.CreatedAt = DateTime.Now;
            productCategory.CreatedBy.Id = user.Id;

            try
            {
                await _context.SaveChangesAsync();
                return new GeneralResponse(true, "Updated Product Category successfully");
            }
            catch (DbUpdateException)
            {
                
                return new GeneralResponse(false, "Update failed");
            }
        }
    }
}