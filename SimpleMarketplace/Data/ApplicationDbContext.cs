using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Models;

namespace SimpleMarketplace.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Item entity
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2000);
                
                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);
                
                // Configure relationship with ApplicationUser (Seller)
                entity.HasOne(e => e.Seller)
                    .WithMany(u => u.ItemsPosted)
                    .HasForeignKey(e => e.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Configure relationship with Category
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Items)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            
            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Configure Interest entity
            modelBuilder.Entity<Interest>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Configure relationship with ApplicationUser (Buyer)
                entity.HasOne(e => e.Buyer)
                    .WithMany(u => u.Interests)
                    .HasForeignKey(e => e.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Configure relationship with Item
                entity.HasOne(e => e.Item)
                    .WithMany(i => i.InterestedBuyers)
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Ensure a user can only mark interest once per item
                entity.HasIndex(e => new { e.BuyerId, e.ItemId })
                    .IsUnique();
            });
        }
    }
}
