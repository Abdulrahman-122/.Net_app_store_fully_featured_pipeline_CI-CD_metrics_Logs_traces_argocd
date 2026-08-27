using ElectroAPI.DTOs;
using ElectroAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    // GET api/products?search=phone&categoryId=1&minPrice=100&maxPrice=5000
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        var products = await _productService.GetAllAsync(search, categoryId, minPrice, maxPrice);

        // Adjust image URLs that reference local /images/ so they use the current request host
        foreach (var p in products)
        {
            if (string.IsNullOrWhiteSpace(p.ImageUrl)) continue;
            try
            {
                if (p.ImageUrl.Contains("/images/"))
                {
                    var uriOk = Uri.TryCreate(p.ImageUrl, UriKind.RelativeOrAbsolute, out var uri);
                    var path = uriOk && uri.IsAbsoluteUri ? uri.AbsolutePath : (p.ImageUrl.StartsWith("/") ? p.ImageUrl : "/" + p.ImageUrl);
                    p.ImageUrl = $"{Request.Scheme}://{Request.Host}{path}";
                }
            }
            catch { }
        }

        return Ok(products);
    }

    // GET api/products/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { message = "Product not found." });

        // Adjust image URL if it references local /images/
        if (!string.IsNullOrWhiteSpace(product.ImageUrl) && product.ImageUrl.Contains("/images/"))
        {
            try
            {
                var uriOk = Uri.TryCreate(product.ImageUrl, UriKind.RelativeOrAbsolute, out var uri);
                var path = uriOk && uri.IsAbsoluteUri ? uri.AbsolutePath : (product.ImageUrl.StartsWith("/") ? product.ImageUrl : "/" + product.ImageUrl);
                product.ImageUrl = $"{Request.Scheme}://{Request.Host}{path}";
            }
            catch { }
        }

        return Ok(product);
    }

    // POST api/products  [Admin فقط]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var created = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
    }

    // PUT api/products/5  [Admin فقط]
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var updated = await _productService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(new { message = "Product not found." });

        return Ok(updated);
    }

    // DELETE api/products/5  [Admin فقط]
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = "Product not found." });

        return Ok(new { message = "Product deleted successfully." });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file");

        var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var path = Path.Combine(folder, fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Use the request's host instead of hardcoded localhost
        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host.Value;
        var url = $"{scheme}://{host}/images/{fileName}";

        return Ok(new { imageUrl = url });
    }
}