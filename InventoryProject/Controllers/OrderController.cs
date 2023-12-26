using InventoryProject.Contracts;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderRepository;
        
        public OrderController(IOrder orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Manager, User, Admin")]
        public async Task<IActionResult> CreateOrderAsync(ICollection<OrderHelperDTO> orderHelperDTOs)
        {
            var response = await _orderRepository.CreateOrderAsync(orderHelperDTOs, HttpContext);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles ="Manager, Admin")]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var response = await _orderRepository.GetAllOrdersAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Manager, Admin")]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            var response = await _orderRepository.GetOrderByIdAsync(id, HttpContext);
            return Ok(response);
        }

        [HttpPut("Approving")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> ApproveOrderAsync(int id)
        {
            var response = await _orderRepository.ApprovedOrderAsync(id, HttpContext);
            return Ok(response);
        }

        [HttpPut("Rejecting")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> RejectedOrderAsync(int id)
        {
            var response = await _orderRepository.RejectedOrderAsync(id, HttpContext);
            return Ok(response);
        }

        [HttpPut("Cancelling")]
        [Authorize]
        public async Task<IActionResult> CancelOrderAsync(int id)
        {
            var response = await _orderRepository.CancelledOrderAsync(id, HttpContext);
            return Ok(response);
        }

        [HttpPut("OnHand")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> OnHandOrderAsync(int id)
        {
            var response = await _orderRepository.OnHandOrderAsync(id, HttpContext);
            return Ok(response);
        }
        
        [HttpPut("Completed")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> CompleteOrderAsync(int id)
        {
            var response = await _orderRepository.CompleteOrderAsync(id, HttpContext);
            return Ok(response);
        }

    }
}