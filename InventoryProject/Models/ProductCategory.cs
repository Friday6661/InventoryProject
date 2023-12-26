using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryProject.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [ForeignKey(nameof(ApplicationUser))]
        // public string? UserId { get; set; }
        public ApplicationUser CreatedBy { get; set; } = null!;
    }
}