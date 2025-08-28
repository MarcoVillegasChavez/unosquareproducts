using mx.unosquare.products.domain;

namespace mx.unosquare.products.application.Models;
public class ProductsResult
{
    public int totalRecords { get; set; }
    public int filteredRecords { get; set; }
    public List<Products> productsLst { get; set; }
}
