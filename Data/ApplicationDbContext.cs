using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IMS.Models;
using System.Reflection.Metadata;

namespace IMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<IMS.Models.Branch> Branch { get; set; }
        public DbSet<IMS.Models.Product> Product { get; set; }
        public DbSet<IMS.Models.Supplier> Supplier { get; set; }
        //public DbSet<IMS.Models.CustomerGroup>? CustomerGroup { get; set; }
        public DbSet<IMS.Models.Customer> Customer { get; set; }
        public DbSet<IMS.Models.PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<IMS.Models.SalesOrder> SalesOrder { get; set; }
        public DbSet<IMS.Models.Invoice> Invoice { get; set; }
        public DbSet<IMS.Models.Shipment> Shipment { get; set; }
        public DbSet<IMS.Models.Warehouse> Warehouse { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PurchaseOrder>()
                .HasOne(e => e.Product)
                .WithMany(e => e.PurchaseOrder)
                .OnDelete(DeleteBehavior.ClientCascade);


            modelBuilder
                .Entity<SalesOrder>()
                .HasOne(e => e.Product)
                .WithMany(e => e.SalesOrder)
                .OnDelete(DeleteBehavior.ClientCascade);

            base.OnModelCreating(modelBuilder);
        }
    }


}