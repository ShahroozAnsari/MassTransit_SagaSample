using Microsoft.EntityFrameworkCore;
using Order.Models;

namespace Warehouse.Models
{
    public class WarehouseContext : DbContext
    {

        public DbSet<Item> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=MS.Warehouse;Trusted_Connection=True;");
        }
    }
}
