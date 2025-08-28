using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mx.unosquare.products.common;
using mx.unosquare.products.infrastructure;
using System.Text;
using UnoSquareProducts.API.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UnoSquareProductsAPIContextConnection") ?? throw new InvalidOperationException("Connection string 'UnoSquareProductsAPIContextConnection' not found.");;
builder.Services.AddDbContext<UnoSquareProductsAPIContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddMvc(config =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(new string[] { "Usuario API" })
        .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<UnoSquareProductsAPIContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    /*options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.ClaimsIssuer = Configuration["Authentication:Issuer"];*/
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        //ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"])),
        RequireExpirationTime = true,
        //ClockSkew = TimeSpan.FromSeconds(0)u
    };
});

// Add services to the container.
builder.Services.AddScoped<IProductsService, ProductsService>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
