using mx.unosquare.products.domain;

namespace mx.unosquare.products.common
{
    public interface IProductsService
    {
        Task<List<Products>> GetAllProducts();
        Task<Products> GetProductById(int id);
        Task<Products> CreateProduct(Products product);
        Task<Products> UpdateProduct(int id, Products product);
        Task<bool> DeleteProduct(int id);
    }
}
