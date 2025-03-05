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
    }
}