namespace mx.unosquare.products.application.Models;
public class ProductsRequest
{
    public string? Description { get; set; }
    public string SKU { get; set; } = null!;
    public decimal Price { get; set; }
}
