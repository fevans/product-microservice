using Microsoft.EntityFrameworkCore;

namespace Product.Service.Infrastructure.Data.EntityFramework;

public static class ProductContextSeed
{
    public static void MigrateDatabase(this WebApplication webapp)
    {
        using var scope = webapp.Services.CreateScope();
        using var productContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
        productContext.Database.Migrate();
        
    }
}