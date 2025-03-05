using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Product.Service.Infrastructure.Data.EntityFramework;

internal class ProductTypeConfiguration: IEntityTypeConfiguration<Models.ProductType>
{
    public void Configure(EntityTypeBuilder<Models.ProductType> builder)
    {
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Type)
            .IsRequired()
            .HasMaxLength(100);
        // Explain the following line
        builder.HasData(
            new Models.ProductType { Id = 1, Type = "Shoes" },
            new Models.ProductType { Id = 2, Type = "Shorts" }
        );
    }
}