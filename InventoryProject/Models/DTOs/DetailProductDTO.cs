using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models.DTOs
{
    public class DetailProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public ProductType ProductType { get; set; }
        public string ImageFileName { get; set; }
        public DateTime CreatedAt { get; set; }
        // public string UserId { get; set; }
        public UserProfileDTO CreatedBy { get; set; }
        // public int ProductCategoryId { get; set; }
        public DetailProductCategoryDTO ProductCategory { get; set; }
    }
}