using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models
{
    public class OrderProductReusable
    {
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public OrderProductStatus OrderProductStatus { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public string CreatedById { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(OrderId))]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey(nameof(ProductId))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}