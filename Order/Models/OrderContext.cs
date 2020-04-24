using Microsoft.EntityFrameworkCore;

namespace Order.Models
{
    public class OrderContext:DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Item> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=MS.Order;Trusted_Connection=True;");
        }
    }
}
