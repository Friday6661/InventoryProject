using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Contracts
{
    public interface IOrder
    {
        Task<GeneralResponse> CreateOrderAsync(ICollection<OrderHelperDTO> orderHelperDTOs, HttpContext httpContext);
        Task<ICollection<OrderDTO>> GetAllOrdersAsync();
        Task<DetailOrderResponse> GetOrderByIdAsync(int id, HttpContext httpContext);
        Task<GeneralResponse> ApprovedOrderAsync(int OrderId, HttpContext httpContext);
        Task<GeneralResponse> RejectedOrderAsync(int OrderId, HttpContext httpContext);
        Task<GeneralResponse> CancelledOrderAsync(int OrderId, HttpContext httpContext);
        Task<GeneralResponse> OnHandOrderAsync(int OrderId, HttpContext httpContext);
        Task<GeneralResponse> CompleteOrderAsync(int id, HttpContext httpContext);
    }
}