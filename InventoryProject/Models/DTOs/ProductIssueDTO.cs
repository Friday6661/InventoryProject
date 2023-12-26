using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models.DTOs
{
    public class ProductIssueDTO
    {
        public IssueStatus Status { get; set; }
        public int Quantity { get; set; }
        public IFormFile ImageFile { get; set; }
        public int ProductId { get; set; }
    }
}