using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mx.unosquare.products.application;
using mx.unosquare.products.application.Enums;
using mx.unosquare.products.application.Models;
using mx.unosquare.products.common;
using System.Collections.Generic;

namespace ccapi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        public ProductsController(IConfiguration configuration, IProductsService productsService)
        {
            _productsService = productsService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string from, string limit)
        {
            ProductsApplication application = new ProductsApplication(_productsService);
            var result = await application.GetAllProducts(from,limit);
            if (!result.IsSuccess)
                return result.Status switch
                {
                    ResultStatus.ValidationError => BadRequest(result.Error),
                    ResultStatus.NotFound => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(result.Value);
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> Index(string productId)
        {
            ProductsApplication application = new ProductsApplication(_productsService);
            var result = await application.GetProductById(productId);
            if (!result.IsSuccess)
                return result.Status switch
                {
                    ResultStatus.ValidationError => BadRequest(result.Error),
                    ResultStatus.NotFound => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(result.Value);
        }
        [HttpPost]
        public async Task<IActionResult> Post(ProductsRequest request)
        {
            ProductsApplication application = new ProductsApplication(_productsService);
            var result = await application.CreateProduct(request);
            if (!result.IsSuccess)
                return result.Status switch
                {
                    ResultStatus.ValidationError => BadRequest(result.Error),
                    ResultStatus.NotFound => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(result.Value);
        }
        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(ProductsRequest request, string productId)
        {
            ProductsApplication application = new ProductsApplication(_productsService);
            var result = await application.UpdateProduct(productId, request);
            if (!result.IsSuccess)
                return result.Status switch
                {
                    ResultStatus.ValidationError => BadRequest(result.Error),
                    ResultStatus.NotFound => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(result.Value);
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(string productId)
        {
            ProductsApplication application = new ProductsApplication(_productsService);
            var result = await application.DeleteProduct(productId);
            if (!result.IsSuccess)
                return result.Status switch
                {
                    ResultStatus.ValidationError => BadRequest(result.Error),
                    ResultStatus.NotFound => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok();
        }
    }
}
