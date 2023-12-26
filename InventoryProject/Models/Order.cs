using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int TotalQuantity { get; set; }
        public int ProcessQuantity { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public ApplicationUser CreatedBy { get; set; }
        
        public virtual ICollection<OrderProductReusable> OrderProductReusables { get; set; }
        public virtual ICollection<OrderProductConsumable> OrderProductConsumables { get; set; }
    }
}