using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Product.Service.Infrastructure.Data.EntityFramework;

namespace Product.Tests;

public class ProductWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private ProductContext? _productContext;
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddConfiguration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.tests.json")
                .Build());
        });
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ApplyMigrations);
    }

   

    private void ApplyMigrations(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        _productContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
        _productContext.Database.Migrate();
       
        
    }

    private async Task SeedDatabase()
         
    {
        var productContext = this.Services.GetRequiredService<ProductContext>();
        new List<Service.Models.Product>()
        {
            new() { Id = 1, Name = "Nike Air Max", Price = 100.00m, ProductTypeId = 1 },
            new() { Id = 2, Name = "Adidas Superstar", Price = 80.00m, ProductTypeId = 1 },
            new() { Id = 3, Name = "Nike Shorts", Price = 20.00m, ProductTypeId = 2 },
            new() { Id = 4, Name = "Adidas Shorts", Price = 15.00m, ProductTypeId = 2 }
        }.ForEach(p => productContext.Products.Add(p));
        await productContext.SaveChangesAsync();
        
    }
     Task IAsyncLifetime.InitializeAsync()
     {
          _ = SeedDatabase();
         return Task.CompletedTask;
     }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _productContext?.Database.EnsureDeletedAsync() ?? Task.CompletedTask;
    }
}