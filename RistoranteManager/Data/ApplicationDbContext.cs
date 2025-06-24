using Microsoft.EntityFrameworkCore;
using RistoranteManager.Models;

namespace RistoranteManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemExtra> OrderItemExtras { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DoughType> DoughTypes { get; set; }
        public DbSet<Extra> Extras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura la precisione e la scala per i campi decimali
            modelBuilder.Entity<DoughType>()
                .Property(d => d.AdditionalPrice)
                .HasPrecision(8, 2); // 8 cifre totali, 2 decimali

            modelBuilder.Entity<Extra>()
                .Property(e => e.Price)
                .HasPrecision(8, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.Price)
                .HasPrecision(8, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.TotalPrice)
                .HasPrecision(8, 2);

            modelBuilder.Entity<OrderItemExtra>()
                .Property(e => e.Price)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(8, 2);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed tables
            for (int i = 1; i <= 12; i++)
            {
                modelBuilder.Entity<Table>().HasData(
                    new Table
                    {
                        Id = i,
                        Number = i,
                        Status = TableStatus.Free,
                        Covers = 0,
                        UseCount = 0,
                        IsClosed = false
                    }
                );
            }

            // Seed products
            modelBuilder.Entity<Product>().HasData(
                // Antipasti
                new Product { Id = 1, Name = "Bruschetta al Pomodoro", Price = 8, Category = "antipasti", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 2, Name = "Antipasto Misto", Price = 12, Category = "antipasti", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 3, Name = "Caprese", Price = 10, Category = "antipasti", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 4, Name = "Frittura di Mare", Price = 12, Category = "antipasti", Type = "kitchen", IsCustomizable = false },

                // Pasta
                new Product { Id = 5, Name = "Spaghetti alla Carbonara", Price = 14, Category = "pasta", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 6, Name = "Penne all'Arrabbiata", Price = 14, Category = "pasta", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 7, Name = "Tagliatelle ai Funghi Porcini", Price = 16, Category = "pasta", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 8, Name = "Risotto ai Frutti di Mare", Price = 18, Category = "pasta", Type = "kitchen", IsCustomizable = false },

                // Pizza
                new Product { Id = 9, Name = "Margherita", Price = 9, Category = "pizza", Type = "pizzeria", IsCustomizable = true },
                new Product { Id = 10, Name = "Diavola", Price = 11, Category = "pizza", Type = "pizzeria", IsCustomizable = true },
                new Product { Id = 11, Name = "Quattro Formaggi", Price = 12, Category = "pizza", Type = "pizzeria", IsCustomizable = true },
                new Product { Id = 12, Name = "Capricciosa", Price = 13, Category = "pizza", Type = "pizzeria", IsCustomizable = true },
                new Product { Id = 13, Name = "Napoletana", Price = 10, Category = "pizza", Type = "pizzeria", IsCustomizable = true },
                new Product { Id = 14, Name = "Prosciutto e Funghi", Price = 12, Category = "pizza", Type = "pizzeria", IsCustomizable = true },

                // Dessert
                new Product { Id = 15, Name = "Tiramisù", Price = 6, Category = "dessert", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 16, Name = "Panna Cotta", Price = 6, Category = "dessert", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 17, Name = "Cannoli Siciliani", Price = 7, Category = "dessert", Type = "kitchen", IsCustomizable = false },
                new Product { Id = 18, Name = "Gelato Artigianale", Price = 6, Category = "dessert", Type = "kitchen", IsCustomizable = false }
            );

            // Seed dough types
            modelBuilder.Entity<DoughType>().HasData(
                new DoughType { Id = 1, Name = "Classica", AdditionalPrice = 0 },
                new DoughType { Id = 2, Name = "Napoli", AdditionalPrice = 0 },
                new DoughType { Id = 3, Name = "Cereali", AdditionalPrice = 2 },
                new DoughType { Id = 4, Name = "Senza Glutine", AdditionalPrice = 2 }
            );

            // Seed extras
            modelBuilder.Entity<Extra>().HasData(
                new Extra { Id = 1, Name = "Mozzarella senza lattosio", Price = 1.5m },
                new Extra { Id = 2, Name = "Bufala", Price = 2 },
                new Extra { Id = 3, Name = "Funghi porcini", Price = 2.5m },
                new Extra { Id = 4, Name = "Prosciutto crudo", Price = 2 }
            );
        }
    }
}