using ECommerce.Shared.Infrastructure.EventBus;
using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Product.Service.ApiModels;
using Product.Service.Infrastructure.Data;
using Product.Service.IntegrationEvents;

namespace Product.Service.Endpoints;

public static class ProductApiEndpoints
{
    // create a MapGet endpoint for the /products path. Use FromServices to get the IProductStore instance. Return a TasK<IResult> from the endpoint.
    public static void RegisterEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/{productId}",
            async Task<IResult> ([FromServices] IProductStore productStore, int productId) =>
            {
                var product = await productStore.GetById(productId);
                return product is null
                    ? TypedResults.NotFound("Product not found")
                    : TypedResults.Ok(new GetProductResponse(
                        product.Id,
                        product.Name,
                        product.Price,
                        product.ProductType?.Type ?? string.Empty,
                        product.Description));

            });

        // create a MapPost endpoint for the /products path. Use FromServices to get the IProductStore instance. Return a Task<IResult> from the endpoint.  
        routeBuilder.MapPost("/",
            async Task<IResult> ([FromServices] IProductStore productStore, CreateProductRequest request) =>
            {
                var product = new Models.Product
                {
                    Name = request.Name,
                    Price = request.Price,
                    ProductTypeId = request.ProductTypeId,
                    Description = request.Description
                };
                await productStore.CreateProduct(product);
                return TypedResults.Created(product.Id.ToString());
            });


        // create a MapPut endpoint for the /products path. Use FromServices to get the IProductStore instance. Return a Task<IResult> from the endpoint.

        routeBuilder.MapPut("/{productId}",
            async Task<IResult> ([FromServices] IProductStore productStore, 
                [FromServices] IEventBus eventBus,
                int productId,
                UpdateProductRequest request) =>
            {
                var product = await productStore.GetById(productId);
                if (product is null)
                {
                    return TypedResults.NotFound($"Product with product id {productId} not found");
                }

                var existingPrices = product.Price;

                product.Name = request.Name;
                product.Price = request.Price;
                product.ProductTypeId = request.ProductTypeId;
                product.Description = request.Description;
                await productStore.UpdateProduct(product);
                
                if (!existingPrices.Equals(product.Price))
                {
                    await eventBus.PublishAsync(new ProductPriceUpdatedEvent(
                        productId, request.Price));
                }
                return TypedResults.NoContent();
            });


    }

}
