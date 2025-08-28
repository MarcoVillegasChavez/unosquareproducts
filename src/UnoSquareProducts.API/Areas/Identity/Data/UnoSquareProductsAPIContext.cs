using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UnoSquareProducts.API.Areas.Identity.Data;

namespace UnoSquareProducts.API.Data;

public class UnoSquareProductsAPIContext : IdentityDbContext<IdentityUser>
{
    public UnoSquareProductsAPIContext(DbContextOptions<UnoSquareProductsAPIContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
