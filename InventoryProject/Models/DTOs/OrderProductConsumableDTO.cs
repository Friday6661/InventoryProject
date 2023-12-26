using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models.DTOs
{
    public class OrderProductConsumableDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public OrderProductStatus OrderProductStatus { get; set; }
        public int ProductId { get; set; }
        public string CreatedById { get; set; }
    }
}