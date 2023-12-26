using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Contracts;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategory _productCategory;
        public ProductCategoryController(IProductCategory productCategory)
        {
            _productCategory = productCategory;
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryDTO productCategoryDTO)
        {
            var response = await _productCategory.CreateProductCategoryAsync(productCategoryDTO, HttpContext);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var response = await _productCategory.DeleteProductCategoryAsync(id);
            return Ok(response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProductCategories()
        {
            var response = await _productCategory.GetAllProductCategoriesAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCategoryById(int id)
        {
            var response = await _productCategory.GetProductCategoryByIdAsync(id);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateProductCategory(int id, UpdateProductCategoryDTO productCategoryDTO)
        {
            var response = await _productCategory.UpdateProductCategoryAsync(id, productCategoryDTO, HttpContext);
            return Ok(response);
        }
    }
}