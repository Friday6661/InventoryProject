using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Models.DTOs;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Contracts
{
    public interface IOrderProductConsumable
    {
        Task<OrderProductConsumableResponse> GetAllOrderProductConsumablesAsync(HttpContext httpContext);
        Task<DetailOrderProductConsumableResponse> GetOrderProductConsumableByIdAsync(int id, HttpContext httpContext);
        Task<OrderProductConsumableResponse> GetOrderProductConsumableByOrderIdAsync(int orderId, HttpContext httpContext);
        Task<GeneralResponse> RejectedOrderProductConsumableAsync(int OrderId, HttpContext httpContext);
        Task<GeneralResponse> CancelledOrderProductConsumableAsync(int OrderId, HttpContext httpContext); 
    }
}