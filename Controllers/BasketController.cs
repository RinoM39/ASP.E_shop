using E_Shop_1.Data;
using E_Shop_1.Models; // تأكد أن هذا هو اسم مشروعك الصحيح
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Shop_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 حماية السلة: يجب تسجيل الدخول لاستخدامها
    public class BasketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BasketController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> AddItemToBasket(int productId, int quantity)
        {
            // 1. جلب معرف المستخدم من التوكن (الذي سجل الدخول)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. البحث عن سلة المستخدم أو إنشاء واحدة جديدة
            var basket = await _context.Baskets
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.BuyerId == userId);

            if (basket == null)
            {
                basket = new Basket { BuyerId = userId };
                _context.Baskets.Add(basket);
            }

            // 3. إضافة المنتج أو تحديث الكمية
            var item = basket.Items.FirstOrDefault(p => p.ProductId == productId);
            if (item == null)
            {
                basket.Items.Add(new BasketItem { ProductId = productId, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
            }

            // 4. حفظ في قاعدة البيانات
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return StatusCode(201); // تم الإضافة بنجاح
            return BadRequest("حدث خطأ أثناء إضافة المنتج للسلة");
        }

        [HttpGet]
        public async Task<ActionResult<Basket>> GetBasket()
        {
            // 1. جلب معرف المستخدم من التوكن
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. جلب السلة مع "تضمين" العناصر وتفاصيل المنتجات (Include)
            var basket = await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product) // مهم جداً لجلب اسم المنتج وسعره
                .FirstOrDefaultAsync(x => x.BuyerId == userId);

            if (basket == null)
            {
                return NotFound("السلة فارغة حالياً");
            }

            return Ok(basket);
        }
        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId)
        {
            // 1. جلب معرف المستخدم من التوكن
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. البحث عن سلة المستخدم مع عناصرها
            var basket = await _context.Baskets
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.BuyerId == userId);

            if (basket == null) return NotFound("لم يتم العثور على سلة لهذا المستخدم");

            // 3. البحث عن المنتج المطلوب حذفه داخل السلة
            var item = basket.Items.FirstOrDefault(p => p.ProductId == productId);
            if (item == null) return NotFound("هذا المنتج غير موجود في السلة أصلاً");

            // 4. حذف العنصر من قائمة العناصر
            basket.Items.Remove(item);

            // 5. حفظ التغييرات في قاعدة البيانات
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok("تم حذف المنتج من السلة بنجاح");

            return BadRequest("حدث خطأ أثناء محاولة حذف المنتج");
        }
        [HttpPut]
        public async Task<ActionResult> UpdateItemQuantity(int productId, int quantity)
        {
            // 1. التأكد من أن الكمية ليست صفراً أو سالبة
            if (quantity <= 0) return BadRequest("يجب أن تكون الكمية أكبر من صفر (أو استخدم الحذف)");

            // 2. جلب معرف المستخدم
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 3. البحث عن السلة
            var basket = await _context.Baskets
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.BuyerId == userId);

            if (basket == null) return NotFound("السلة غير موجودة");

            // 4. البحث عن المنتج داخل السلة
            var item = basket.Items.FirstOrDefault(p => p.ProductId == productId);
            if (item == null) return NotFound("المنتج غير موجود في السلة لتعديله");

            // 5. تحديث الكمية بالقيمة الجديدة
            item.Quantity = quantity;

            // 6. حفظ التغييرات
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok("تم تحديث الكمية بنجاح");

            return BadRequest("لم يتم إجراء أي تغيير على الكمية");
        }
    }
}
