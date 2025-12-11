using Microsoft.EntityFrameworkCore;
using eShopModernized.Models;

namespace eShopModernized.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public DbSet<CatalogItem> CatalogItems { get; set; } = null!;
        public DbSet<CatalogBrand> CatalogBrands { get; set; } = null!;
        public DbSet<CatalogType> CatalogTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // CatalogType configuration
            builder.Entity<CatalogType>(entity =>
            {
                entity.ToTable("CatalogType");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // CatalogBrand configuration
            builder.Entity<CatalogBrand>(entity =>
            {
                entity.ToTable("CatalogBrand");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Brand)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // CatalogItem configuration
            builder.Entity<CatalogItem>(entity =>
            {
                entity.ToTable("Catalog");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .ValueGeneratedNever(); // Matches EF6 DatabaseGeneratedOption.None

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.PictureFileName)
                    .IsRequired();

                entity.Ignore(e => e.PictureUri);

                // Relationships
                entity.HasOne(e => e.CatalogBrand)
                    .WithMany()
                    .HasForeignKey(e => e.CatalogBrandId)
                    .IsRequired();

                entity.HasOne(e => e.CatalogType)
                    .WithMany()
                    .HasForeignKey(e => e.CatalogTypeId)
                    .IsRequired();
            });
        }
    }
}
