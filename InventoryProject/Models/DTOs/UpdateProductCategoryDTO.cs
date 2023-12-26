using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryProject.Models.DTOs
{
    public class UpdateProductCategoryDTO
    {
        [Required]
        [MaxLength(35)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
    }
}