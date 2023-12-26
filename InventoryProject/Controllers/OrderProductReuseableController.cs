using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Contracts;
using InventoryProject.Enum;
using InventoryProject.Models.DTOs;
using InventoryProject.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderProductReuseableController : ControllerBase
    {
        private readonly IOrderProductReuseable _orderProductReuseable;
        public OrderProductReuseableController(IOrderProductReuseable orderProductReuseable)
        {
            _orderProductReuseable = orderProductReuseable;
        }

        [HttpPut("Cancelled/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelledOrderProductReuseableAsync(int id)
        {
            var response = await _orderProductReuseable.CancelledOrderProductReuseableAsync(id, HttpContext);
            return Ok(response);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetailOrderProductReuseableAsync(int id)
        {
            var response = await _orderProductReuseable.GetOrderProductReuseableByIdAsync(id, HttpContext);
            return Ok(response);
        }
        [HttpGet("OrderId")]
        [Authorize]
        public async Task<IActionResult> GetOrderProductReuseableByOrderIdAsync(int OrderId)
        {
            var response = await _orderProductReuseable.GetOrderProductReuseableByOrderIdAsync(OrderId, HttpContext);
            return Ok(response);
        }

        [HttpPut("Rejected")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> RejectedOrderProductReuseableAsync(int id)
        {
            var response = await _orderProductReuseable.RejectedOrderProductReuseableAsync(id, HttpContext);
            return Ok(response);
        }

        [HttpPut("Returned")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ReturnedOrderProductReuseableAsync(int id)
        {
            var response = await _orderProductReuseable.ReturnedOrderProductReuseableAsync(id, HttpContext);
            return Ok(response);
        }

        [HttpPut("Reported")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ReportedOrderProductReuseableAsync(int id, ReportIssueDTO reportIssueDTO)
        {
            var response = await _orderProductReuseable.ReportOrderProductReuseableAsync(id, reportIssueDTO, HttpContext);
            return Ok(response);
        }
    }
}