using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus OrderStatus { get; set; }
        // public string UserId { get; set; }
        public UserProfileDTO CreatedBy { get; set; }
        public ICollection<OrderProductReuseableDTO> OrderProductReusables { get; set; }
        public ICollection<OrderProductConsumableDTO> OrderProductConsumables { get; set; }
    }
}