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

// Create a request model for clarity
public class PasswordRequest
{
    public string Password { get; set; }
}

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> AddProduct(
            [FromHeader(Name = "X-Api-Password")] string apiPassword,
            [FromBody] Product product)
        {
            // Check password
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
    }
}