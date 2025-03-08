using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Product.Service.ApiModels;
using Product.Service.Endpoints;
using Product.Service.Infrastructure.Data.EntityFramework;
using Xunit;

namespace Product.Tests.Api
{
    public class ProductApiTests(ProductWebApplicationFactory factory) : IntegrationTestBase(factory)
    {
        private readonly HttpClient _client = factory.CreateClient();

        



        [Fact]
        public async Task GivenProductDoesNotExist_WhenGetProduct_ThenReturnsNotFound()
        {
            // Arrange
            var productId = 999;

            // Act
            var response = await _client.GetAsync($"/{productId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GivenProductExists_WhenGetProduct_ThenReturnsProduct()
        {
            // Arrange
            var productId = 1;

            // Act
            var response = await _client.GetAsync($"/{productId}");
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadFromJsonAsync<GetProductResponse>();

            // Assert
            Assert.NotNull(product);
            Assert.Equal(productId, product.Id);
        }

        [Fact]
        public async Task GivenValidProduct_WhenCreateProduct_ThenReturnsCreated()
        {
            // Arrange
            var request = new CreateProductRequest
            (
                Name :"New Product",
                Price : 100,
                ProductTypeId :1,
                Description : "New Product Description"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GivenValidProduct_WhenUpdateProduct_ThenReturnsNoContent()
        {
            // Arrange
            // Create product
            // Arrange
            var createRequest = new CreateProductRequest
            (
                Name :"New Product",
                Price : 100,
                ProductTypeId :1,
                Description : "New Product Description"
            );
            await _client.PostAsJsonAsync("/", createRequest);

            var productId = 1;
            var updateRequest = new UpdateProductRequest
            (
                Name : "Updated Product",
                Price : 150,
                ProductTypeId : 1,
                Description : "Updated Product Description"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/{productId}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
           // Assert.Equal(productId, response.);
        }

        [Fact]
        public async Task GivenProductDoesNotExist_WhenUpdateProduct_ThenReturnsNotFound()
        {
            // Arrange
            var productId = 999;
            var request = new UpdateProductRequest
            (
                Name : "Updated Product",
                Price : 150,
                ProductTypeId : 1,
                Description : "Updated Product Description"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/{productId}", request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}