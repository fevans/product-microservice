using Microsoft.Extensions.DependencyInjection;
using Product.Service.Infrastructure.Data.EntityFramework;

namespace Product.Tests;

public abstract class IntegrationTestBase : IClassFixture<ProductWebApplicationFactory>
{
    internal readonly ProductContext  ProductContext;
    internal readonly HttpClient HttpClient;
    
    protected IntegrationTestBase(ProductWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        ProductContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
        HttpClient = factory.CreateClient();
    }
    
}