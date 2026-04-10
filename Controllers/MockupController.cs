using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Printawyapis.Data;
using Printawyapis.Models;

namespace Printawyapis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockupController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MockupController(AppDbContext context)
        {
            _context = context;
        }

        public class MockupRequest
        {
            public string DesignImage { get; set; } = string.Empty;
            public int ProductId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMockup([FromBody] MockupRequest request)
        {
            // -------------------------
            // VALIDATION
            // -------------------------
            if (request == null)
                return BadRequest("Request is null");

            if (string.IsNullOrWhiteSpace(request.DesignImage))
                return BadRequest("Design image is required");

            // -------------------------
            // GET PRODUCT
            // -------------------------
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product == null)
                return BadRequest("Invalid ProductId");

            if (string.IsNullOrWhiteSpace(product.Photo))
                return BadRequest("Product image is missing");

            try
            {
                // -------------------------
                // LOAD PRODUCT IMAGE
                // -------------------------
                using var http = new HttpClient();
                var productBytes = await http.GetByteArrayAsync(product.Photo);

                using var productStream = new MemoryStream(productBytes);
                using var productImg = Image.Load<Rgba32>(productStream);

                // -------------------------
                // DECODE BASE64 DESIGN
                // -------------------------
                var base64 = request.DesignImage;

                if (base64.Contains(","))
                    base64 = base64.Split(',')[1];

                byte[] designBytes;

                try
                {
                    designBytes = Convert.FromBase64String(base64);
                }
                catch
                {
                    return BadRequest("Invalid base64 image");
                }

                using var designStream = new MemoryStream(designBytes);
                using var designImg = Image.Load<Rgba32>(designStream);

                // -------------------------
                // 🎯 CENTER PRINT AREA (BOX)
                // -------------------------
                int boxWidth = (int)(productImg.Width * 0.3);
                int boxHeight = (int)(productImg.Height * 0.52);

                int boxX = (productImg.Width - boxWidth) / 2;
                int boxY = (int)(productImg.Height * 0.24);

                // -------------------------
                // RESIZE DESIGN TO FIT BOX
                // -------------------------
                designImg.Mutate(x =>
                {
                    x.Resize(boxWidth, boxHeight);
                });

                // -------------------------
                // DRAW DESIGN INSIDE BOX
                // -------------------------
                productImg.Mutate(x =>
                {
                    x.DrawImage(designImg, new Point(boxX, boxY), 1f);
                });

                // -------------------------
                // RETURN FINAL MOCKUP
                // -------------------------
                using var ms = new MemoryStream();
                productImg.SaveAsPng(ms);

                return File(ms.ToArray(), "image/png", "mockup.png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Mockup generation failed: {ex.Message}");
            }
        }
    }
}