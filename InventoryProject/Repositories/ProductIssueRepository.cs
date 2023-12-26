using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Contracts;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Repositories
{
    public class ProductIssueRepository : IProductIssue
    {
        private readonly InventoryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public ProductIssueRepository(InventoryDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<GeneralResponse> ReportProductAsync(ProductIssueDTO productIssueDTO, HttpContext httpContext)
        {
            using (var transaction =  _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(httpContext.User);
                    if (user is null) return new GeneralResponse(false, "Invalid User");

                    if (productIssueDTO.ImageFile == null)
                    {
                        return new GeneralResponse(false, "Image is required");
                    }

                    string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    imageFileName += Path.GetExtension(productIssueDTO.ImageFile.FileName);
                    string imagesFolder = _env.WebRootPath + "/images/productIssues";

                    using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
                    {
                        productIssueDTO.ImageFile.CopyTo(stream);
                    }
                    
                    var productIssue = new ProductIssue
                    {
                        CreatedAt = DateTime.Now,
                        Status = productIssueDTO.Status,
                        Quantity = productIssueDTO.Quantity,
                        ImageFile = imageFileName,
                        CreatedBy = user,
                        ProductId = productIssueDTO.ProductId,
                    };
                    _context.ProductIssues.Add(productIssue);
                    await _context.SaveChangesAsync();
                    var product = await _context.Products.Where(p => p.Id == productIssueDTO.ProductId).FirstOrDefaultAsync();
                    if (product is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid Product");
                    }
                    product.StockQuantity -= productIssueDTO.Quantity;
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return new GeneralResponse(true, "Product Reported successfully");
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(true, "Product Failed to report");
                }
            }
        }

    }
}