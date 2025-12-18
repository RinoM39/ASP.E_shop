using E_Shop_1.Data;
using E_Shop_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace E_Shop_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Products_Use_Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // حقن (Injection) الـ DbContext
        public Products_Use_Controller(ApplicationDbContext context)
        {
            _context = context;
       
        }

        [HttpGet] // هذا يعني /api/Products
        public async Task<ActionResult<IEnumerable<Products_Use_Controller>>> GetProducts()
        {
            // ⚠️ ملاحظة: يجب أن تكون لديك فئة (Class) باسم Product

            // جلب جميع المنتجات من جدول Products
            var products = await _context.Products.ToListAsync();

            if (products == null || !products.Any())
            {
                // إرجاع 204 No Content إذا كانت القائمة فارغة
                return NoContent();
            }

            // إرجاع المنتجات مع رمز 200 OK
            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }




        [HttpPost("add")]
        [Authorize(Roles = "Admin")] // 🔒 حماية الواجهة بالتوكن
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            // 1. إضافة المنتج إلى جدول المنتجات
            _context.Products.Add(product);

            // 2. حفظ التغييرات في قاعدة البيانات (تم تصحيح الكلمة هنا)
            await _context.SaveChangesAsync();

            // 3. إرجاع المنتج الذي تم إنشاؤه
            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent(); // 204
        }


    }
}
