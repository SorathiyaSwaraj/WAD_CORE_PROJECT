using Microsoft.EntityFrameworkCore;

namespace wad_core_project.Models
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(DbContextOptions<InventoryContext> options)
        : base(options)
        {
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=wad_core_database;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationship between Transactions and Products
            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Products)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.ProductsId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define the relationship between Transactions and Suppliers
            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Suppliers)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.SuppliersId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Products>()
                .HasOne(p => p.User)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Suppliers>()
                .HasOne(s => s.User)
                .WithMany(u => u.Suppliers)
                .HasForeignKey(s => s.UserId);

        }
    }
}
