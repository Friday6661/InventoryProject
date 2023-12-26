using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;
using InventoryProject.Models.DTOs;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Contracts
{
    public interface IOrderProductReuseable
    {
        Task<OrderProductReuseableResponse> GetAllOrderProductReuseablesAsync(HttpContext httpContext);
        Task<DetailOrderProductReuseableResponse> GetOrderProductReuseableByIdAsync(int id, HttpContext httpContext);
        Task<OrderProductReuseableResponse> GetOrderProductReuseableByOrderIdAsync(int orderId, HttpContext httpContext);
        Task<GeneralResponse> RejectedOrderProductReuseableAsync(int id, HttpContext httpContext);
        Task<GeneralResponse> CancelledOrderProductReuseableAsync(int id, HttpContext httpContext);
        Task<GeneralResponse> ReturnedOrderProductReuseableAsync(int id, HttpContext httpContext);
        Task<GeneralResponse> ReportOrderProductReuseableAsync(int id, ReportIssueDTO reportIssueDTO, HttpContext httpContext);  
    }
}