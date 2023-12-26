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
    public class ProductIssueController : ControllerBase
    {
        private readonly IProductIssue _productIssue;
        public ProductIssueController(IProductIssue productIssue)
        {
            _productIssue = productIssue;
        }

        [HttpPost("CreateProductIssue")]
        [Authorize]
        public async Task<IActionResult> ReportProductIssueAsync([FromForm] ProductIssueDTO productIssueDTO)
        {
            var response = await _productIssue.ReportProductAsync(productIssueDTO, HttpContext);
            return Ok(response);
        }
    }
}