using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printawyapis.Data;
using Printawyapis.Models;

namespace Printawyapis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET CART BY USER
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(cartItems);
        }

        // ✅ ADD TO CART
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            var item = new CartItem
            {
                ProductId = dto.ProductId,
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                PhotoUrl = dto.PhotoUrl,
                Size = dto.Size,
                FrontDesign = dto.FrontDesign,
                BackDesign = dto.BackDesign,
                Quantity = dto.Quantity,
                UserId = dto.UserId
            };

            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        // ✅ DELETE ITEM
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var item = await _context.CartItems.FindAsync(id);

            if (item == null)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }

    // DTO (IMPORTANT - FIXES YOUR BIG JSON PROBLEMS)
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string Description { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string Size { get; set; } = "";
        public string FrontDesign { get; set; } = "";
        public string BackDesign { get; set; } = "";
        public int Quantity { get; set; }
        public string UserId { get; set; } = "";
    }
}