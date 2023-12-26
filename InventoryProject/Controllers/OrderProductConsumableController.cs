using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderProductConsumableController : ControllerBase
    {
        private readonly IOrderProductConsumable _orderProductConsumable;
        public OrderProductConsumableController(IOrderProductConsumable orderProductConsumable)
        {
            _orderProductConsumable = orderProductConsumable;
        }

        [HttpPut("Cancelled")]
        [Authorize]
        public async Task<IActionResult> CancelledOrderProductConsumableAsync(int id)
        {
            var response = await _orderProductConsumable.CancelledOrderProductConsumableAsync(id, HttpContext);
            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllOrderProductConsumablesAsync()
        {
            var response = await _orderProductConsumable.GetAllOrderProductConsumablesAsync(HttpContext);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderProductConsumableByIdAsync(int id)
        {
            var response = await _orderProductConsumable.GetOrderProductConsumableByIdAsync(id, HttpContext);
            return Ok(response);
        }

        [HttpGet("OrderId")]
        [Authorize]
        public async Task<IActionResult> GetOrderProductConsumableByOrderIdAsync(int orderId)
        {
            var response = await _orderProductConsumable.GetOrderProductConsumableByOrderIdAsync(orderId, HttpContext);
            return Ok(response);
        }

        [HttpPut("Rejected")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> RejectedOrderProductConsumableAsync(int id)
        {
            var response = await _orderProductConsumable.RejectedOrderProductConsumableAsync(id, HttpContext);
            return Ok(response);
        }
        
    }
}