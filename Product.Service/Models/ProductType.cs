namespace Product.Service.Models;

/// <summary>
/// Product type model.
/// </summary>
internal class ProductType
{
    public int Id { get; set; }

    public required string Type { get; set; }
}