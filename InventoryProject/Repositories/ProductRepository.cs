using System.Security.Claims;
using InventoryProject.Contracts;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Repositories
{
    public class ProductRepository : IProduct
    {
        private readonly InventoryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProductRepository(InventoryDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        public async Task<GeneralResponse> CreateProductAsync(ProductDTO productDTO, HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return new GeneralResponse(false, "User not authenticated");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return new GeneralResponse(false, "User not found");

            if (productDTO is null) return new GeneralResponse(false, "Model is Empty");
            if (productDTO.ImageFileName == null) return new GeneralResponse(false, "Image file not found");

            var productCategory = await _context.ProductCategories.FindAsync(productDTO.ProductCategoryId);
            if (productCategory is null) return new GeneralResponse(false, "Invalid Product Category");

            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            imageFileName += Path.GetExtension(productDTO.ImageFileName.FileName);
            string imagesFolder = _env.WebRootPath + "/images/products/";
            using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
            {
                productDTO.ImageFileName.CopyTo(stream);
            }

            var product = new Product()
            {
                Name = productDTO.Name,
                StockQuantity = productDTO.StockQuantity,
                ProductType = productDTO.ProductType,
                ImageFileName = imageFileName,
                ProductCategoryId = productDTO.ProductCategoryId,
                // UserId = userId,
                CreatedAt = DateTime.Now,
            };

            product.CreatedBy = new ApplicationUser
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };

            product.ProductCategory = new ProductCategory
            {
                Name = productCategory.Name,
                Description = productCategory.Description,
                IsActive = productCategory.IsActive,
                CreatedAt = productCategory.CreatedAt,
                CreatedBy = productCategory.CreatedBy
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new GeneralResponse(true, "New Product created successfully");
        }

        public async Task<GeneralResponse> DeleteProductAsync(int id)
        {
            var product = await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (product is null) return new GeneralResponse(false, "Product not found");

            string imagesFolder = _env.WebRootPath + "/images/products/";
            System.IO.File.Delete(imagesFolder + product.ImageFileName);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new GeneralResponse(true, "Product deleted successfully");
        }

        public async Task<ICollection<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                            .Include(p => p.ProductCategory)
                            .ToListAsync();
        }

        public async Task<DetailProductResponse> GetProductByIdAsync(int id)
        {

            var product = await _context.Products.Where(p => p.Id == id)
                                                .Include(p => p.ProductCategory)
                                                .Include(p => p.CreatedBy)
                                                .FirstOrDefaultAsync();
            if (product is null) return new DetailProductResponse(false, null!, "Product not found");

            var roles = await _userManager.GetRolesAsync(product.CreatedBy);
            var userRole = roles.FirstOrDefault();

            var detailProduct = new DetailProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                ProductType = product.ProductType,
                ImageFileName = product.ImageFileName,
                // ProductCategoryId = product.ProductCategoryId,
                // UserId = product.CreatedBy.Id,
                CreatedAt = product.CreatedAt,
                CreatedBy = new UserProfileDTO
                {
                    UserName = product.CreatedBy.UserName,
                    FirstName = product.CreatedBy.FirstName,
                    LastName = product.CreatedBy.LastName,
                    Email = product.CreatedBy.Email,
                    Role = userRole ?? "No Role Assigned"
                },
                ProductCategory = new DetailProductCategoryDTO
                {
                    Id = product.ProductCategory.Id,
                    Name = product.ProductCategory.Name,
                    Description = product.ProductCategory.Description,
                    IsActive = product.ProductCategory.IsActive,
                    CreatedAt = product.ProductCategory.CreatedAt,
                    // CreatedBy = new UserProfileDTO
                    // {
                    //     UserName = product.ProductCategory.CreatedBy.UserName,
                    //     FirstName = product.ProductCategory.CreatedBy.FirstName,
                    //     LastName = product.ProductCategory.CreatedBy.LastName,
                    //     Email = product.ProductCategory.CreatedBy.Email,
                    //     Role = (await _userManager.GetRolesAsync(product.ProductCategory.CreatedBy)).FirstOrDefault() ?? "No Role Assigned"
                    // }
                }
            };

            return new DetailProductResponse(true, detailProduct, "Load Product Details successfully");
        }

        public async Task<GeneralResponse> UpdateProductAsync(int id, UpdateProductDTO productDTO, HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return new GeneralResponse(false, "User not authenticated");

            var product = await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (product is null) return new GeneralResponse(false, "Product not found");

            string imageFileName = product.ImageFileName;
            if (productDTO.ImageFileName != null)
            {
                imageFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                imageFileName += Path.GetExtension(productDTO.ImageFileName.FileName);

                string imagesFolder = _env.WebRootPath + "/images/products";
                using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
                {
                    productDTO.ImageFileName.CopyTo(stream);
                }
            }

            product.Name = productDTO.Name;
            product.StockQuantity = productDTO.StockQuantity;
            product.ProductType = productDTO.ProductType;
            product.ImageFileName = imageFileName;
            product.ProductCategoryId = productDTO.ProductCategoryId;
            product.CreatedAt = DateTime.Now;
            product.CreatedBy.Id = userId;

            try
            {
                await _context.SaveChangesAsync();
                return new GeneralResponse(true, "Updated Product successfully");
            }
            catch (DbUpdateException)
            {
                
                return new GeneralResponse(false, "Update failed");
            }
        }
    }
}