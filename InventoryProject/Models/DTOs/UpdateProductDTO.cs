using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models.DTOs
{
    public class UpdateProductDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public ProductType ProductType { get; set; }

        [Required]
        public IFormFile ImageFileName { get; set; }

        [Required]
        public int ProductCategoryId { get; set; }
    }
}