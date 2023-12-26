using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models
{
    public class ProductIssue
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public IssueStatus Status  { get; set; }
        public int Quantity { get; set; }
        public string ImageFile { get; set; }
        
        [ForeignKey(nameof(ApplicationUser))]
        public ApplicationUser CreatedBy { get; set; }

        [ForeignKey(nameof(ProductId))]
        public int ProductId { get; set;}
        public Product Product { get; set;}
    }
}