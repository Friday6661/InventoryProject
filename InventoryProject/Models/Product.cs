using System.ComponentModel.DataAnnotations.Schema;
using InventoryProject.Enum;

namespace InventoryProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public ProductType ProductType { get; set; }
        public string ImageFileName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ApplicationUser))]
        // public string UserId { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        [ForeignKey(nameof(ProductCategoryId))]
        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

    }
}