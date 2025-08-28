using mx.unosquare.products.application.Models;
using mx.unosquare.products.common;
using mx.unosquare.products.domain;

namespace mx.unosquare.products.application
{
    public class ProductsApplication
    {
        private readonly IProductsService _productsService;

        public ProductsApplication(IProductsService productsService)
        {
            _productsService = productsService;
        }

        public async Task<Result<ProductsResult>> GetAllProducts(string? from, string? limit)
        {
            var productsLst = await _productsService.GetAllProducts();
            if (productsLst.Count == 0)
                return Result<ProductsResult>.Fail("No products found", Enums.ResultStatus.UnexpectedError);
            int totalRecords = productsLst.Count();
            int productFilteredCounter = 0;
            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(limit))
            {
                int ifrom;
                int ilimit;
                if (!int.TryParse(from, out ifrom))
                    return Result<ProductsResult>.Fail("The field from must to be int type.", Enums.ResultStatus.ValidationError);
                if (!int.TryParse(limit, out ilimit))
                    return Result<ProductsResult>.Fail("The field limit must to be int type.", Enums.ResultStatus.ValidationError);

                var productsFiltered = productsLst.Skip(ifrom).Take(ilimit).ToList();
                productFilteredCounter = productsFiltered.Count();
                productsLst = productsFiltered;
            }

            return Result<ProductsResult>.Success(new ProductsResult
            {
                productsLst = productsLst,
                filteredRecords = productFilteredCounter,
                totalRecords = totalRecords
            });
        }
        public async Task<Result<Products>> GetProductById(string productId)
        {
            int id;
            if (!int.TryParse(productId, out id))
                return Result<Products>.Fail("The field productId must to be a valid type. It must to be int type", Enums.ResultStatus.ValidationError);
            var productFinded = await _productsService.GetProductById(id);
            if (productFinded is null)
                return Result<Products>.Fail("There was a error. Please notice to the admin.", Enums.ResultStatus.UnexpectedError);
            if (productFinded.Id == 0)
                return Result<Products>.Fail("Product not found.", Enums.ResultStatus.NotFound);

            return Result<Products>.Success(productFinded);

        }
        public async Task<Result<Products>> CreateProduct(ProductsRequest product)
        {
            var validationProduct = ValidateProduct(product);
            if (!validationProduct.IsSuccess)
                return Result<Products>.Fail(validationProduct.Error, Enums.ResultStatus.ValidationError);

            var createdProduct = await _productsService.CreateProduct(new Products { Id = 0, Description = product.Description, SKU = product.SKU, Price = product.Price });
            if (createdProduct == null)
                return Result<Products>.Fail("The product could not be created.", Enums.ResultStatus.UnexpectedError);
            return Result<Products>.Success(createdProduct);
        }
        public async Task<Result<Products>> UpdateProduct(string productId, ProductsRequest product)
        {
            int id;
            if (!int.TryParse(productId, out id))
                return Result<Products>.Fail("The field productId must to be a valid type. It must to be int type", Enums.ResultStatus.ValidationError);
            var validationProduct = ValidateProduct(product);
            if (!validationProduct.IsSuccess)
                return Result<Products>.Fail(validationProduct.Error, Enums.ResultStatus.ValidationError);

            var productFinded = await _productsService.GetProductById(id);
            if (productFinded is null)
                return Result<Products>.Fail("There was a error. Please notice to the admin.", Enums.ResultStatus.UnexpectedError);
            if (productFinded.Id == 0)
                return Result<Products>.Fail("Product not found.", Enums.ResultStatus.NotFound);

            var updatedProduct = await _productsService.UpdateProduct(id, new Products { Description = product.Description, SKU = product.SKU, Price = product.Price });
            if (updatedProduct == null)
                return Result<Products>.Fail("The product could not be updated.", Enums.ResultStatus.UnexpectedError);
            return Result<Products>.Success(updatedProduct);
        }
        public async Task<Result<bool>> DeleteProduct(string productId)
        {
            int id;
            if (!int.TryParse(productId, out id))
                return Result<bool>.Fail("The field productId must to be a valid type. It must to be int type", Enums.ResultStatus.ValidationError);
            var productFinded = await _productsService.GetProductById(id);
            if (productFinded is null)
                return Result<bool>.Fail("There was a error. Please notice to the admin.", Enums.ResultStatus.UnexpectedError);
            if (productFinded.Id == 0)
                return Result<bool>.Fail("Product not found.", Enums.ResultStatus.NotFound);

            if (!await _productsService.DeleteProduct(id))
                return Result<bool>.Fail("The product could not be deleted.", Enums.ResultStatus.UnexpectedError);
            return Result<bool>.Success(true);
        }

        private Result<Products> ValidateProduct(ProductsRequest product)
        {
            if (product == null)
                return Result<Products>.Fail("The product is null.", Enums.ResultStatus.ValidationError);
            if (string.IsNullOrEmpty(product.Description))
                return Result<Products>.Fail("The field description is required.", Enums.ResultStatus.ValidationError);
            if (string.IsNullOrEmpty(product.SKU))
                return Result<Products>.Fail("The field SKU is required.", Enums.ResultStatus.ValidationError);
            if (product.Price <= 0)
                return Result<Products>.Fail("The field Price must be greater than zero.", Enums.ResultStatus.ValidationError);

            return Result<Products>.Success(new Products { Description = product.Description, SKU = product.SKU, Price = product.Price });

        }

    }
}
