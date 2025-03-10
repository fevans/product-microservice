using Microsoft.EntityFrameworkCore;

namespace Product.Service.Infrastructure.Data.EntityFramework;

public  static class EntityFrameworkExtensions
{
    public static void AddSqlServerDatastore(this IServiceCollection services, 
        IConfigurationManager configuration)
    {
        services.AddDbContext<ProductContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default"),
                sqlServerOptionsAction: sqlOptions => {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(40),
                        errorNumbersToAdd: new List<int> { 0 }.AsReadOnly());
                }));
        services.AddScoped<IProductStore, ProductContext>();
    }
}