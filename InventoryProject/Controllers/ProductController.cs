using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Contracts;
using InventoryProject.Models.DTOs;
using InventoryProject.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProduct _productRepository;
        public ProductController(IProduct productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateProductAsync([FromForm] ProductDTO productDTO)
        {
            var response = await _productRepository.CreateProductAsync(productDTO, HttpContext);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var response = await _productRepository.GetAllProductsAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var response = await _productRepository.GetProductByIdAsync(id);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var response = await _productRepository.DeleteProductAsync(id);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> UpdateProductAsync(int id, UpdateProductDTO productDTO)
        {
            var response = await _productRepository.UpdateProductAsync(id, productDTO, HttpContext);
            return Ok(response);
        }
    }
}