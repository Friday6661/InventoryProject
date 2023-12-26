using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryProject.Models
{
    public class InventoryDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
    {
        internal readonly object _userManager;

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProductReusable> OrderProductReusables { get; set; }
        public DbSet<OrderProductConsumable> OrderProductConsumables { get; set; }
        public DbSet<ProductIssue> ProductIssues { get; set; }

    }
}
