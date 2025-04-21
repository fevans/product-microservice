using System.Transactions;
using ECommerce.Shared.Infrastructure.Outbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            }).RequireAuthorization();


        // create a MapPut endpoint for the /products path. Use FromServices to get the IProductStore instance. Return a Task<IResult> from the endpoint.

        routeBuilder.MapPut("/{productId}",
            async Task<IResult> (
                [FromServices] IProductStore productStore, 
                [FromServices] IOutboxStore outboxStore,
                int productId,
                UpdateProductRequest request) =>
            {
                var product = await productStore.GetById(productId);
                if (product is null)
                {
                    return TypedResults.NotFound($"Product with product id {productId} not found");
                }

                var existingPrice = product.Price;
                product.Name = request.Name;
                product.Price = request.Price;
                product.ProductTypeId = request.ProductTypeId;
                product.Description = request.Description;
                //await productStore.UpdateProduct(product);
                await outboxStore.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                    await productStore.UpdateProduct(product);
                    if (!decimal.Equals(existingPrice, request.Price))
                    {
                        await outboxStore.AddOutboxEvent(new ProductPriceUpdatedEvent(productId, request.Price));
                    }
                    scope.Complete();
                });
                
                return TypedResults.NoContent();
            }).RequireAuthorization();


    }

}
