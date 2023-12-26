using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Models.DTOs;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Contracts
{
    public interface IProductIssue
    {
        Task<GeneralResponse> ReportProductAsync(ProductIssueDTO productIssueDTO, HttpContext httpContext);
    }
}