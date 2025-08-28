namespace mx.unosquare.products.domain
{
    public class Products
    {
        public int Id { get; set; } 
        public string? Description { get; set; }
        public string SKU { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
