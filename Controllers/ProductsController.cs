using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printawyapis.Data;
using Printawyapis.Models;

namespace Printawyapis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private const string ProductApiPassword = "msbg3122011";

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // POST: api/products/check-password
        [HttpPost("check-password")]
        public IActionResult CheckPassword([FromBody] PasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { success = false, message = "Password is required." });

            bool isValid = request.Password == ProductApiPassword;
            return Ok(new { success = isValid });
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> AddProduct(
            [FromHeader(Name = "X-Api-Password")] string apiPassword,
            [FromBody] Product product)
        {
            if (apiPassword != ProductApiPassword)
                return Unauthorized("Invalid API password.");

            if (product == null)
                return BadRequest("Product cannot be null.");

            if (string.IsNullOrWhiteSpace(product.Name))
                return BadRequest("Product name is required.");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllProducts), new { id = product.Id }, product);
        }



        [HttpGet("{id}")]
public async Task<IActionResult> GetProductById(int id)
{
    var product = await _context.Products.FindAsync(id);

    if (product == null)
        return NotFound("Product not found.");

    return Ok(product);
}

        // PUT: api/products/{id}  --> Edit product
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct(
            int id,
            [FromHeader(Name = "X-Api-Password")] string apiPassword,
            [FromBody] Product updatedProduct)
        {
            if (apiPassword != ProductApiPassword)
                return Unauthorized("Invalid API password.");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            // Update fields
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Photo = updatedProduct.Photo;

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        // DELETE: api/products/{id}  --> Delete product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(
            int id,
            [FromHeader(Name = "X-Api-Password")] string apiPassword)
        {
            if (apiPassword != ProductApiPassword)
                return Unauthorized("Invalid API password.");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // Request model for password
    public class PasswordRequest
    {
        public string Password { get; set; }
    }
}