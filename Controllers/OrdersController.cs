using E_Shop_1.Data;
using E_Shop_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ضروري جداً لعمل Include
using System.Security.Claims;

namespace E_Shop_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult> CreateOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. جلب السلة
            var basket = await _context.Baskets
                .Include(b => b.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == userId);

            if (basket == null || !basket.Items.Any())
                return BadRequest("السلة فارغة");

            // 2. تحويل عناصر السلة إلى OrderItem_Use
            var items = basket.Items.Select(item => new OrderItem_Use
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Price = item.Product.Price,
                Quantity = item.Quantity
            }).ToList();

            // 3. إنشاء الطلب الجديد باستخدام Order_Use
            var order = new Order_Use
            {
                BuyerId = userId,
                OrderItems = items,
                TotalPrice = items.Sum(i => i.Price * i.Quantity),
                OrderDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Orders_Use.Add(order);
            _context.Baskets.Remove(basket); // تفريغ السلة

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok(new { OrderId = order.Id, Message = "تمت عملية الشراء بنجاح" });
            return BadRequest("فشلت عملية إتمام الطلب");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order_Use>>> GetOrders()
        {
            // 1. جلب معرف المستخدم من التوكن (JWT)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. جلب الطلبات الخاصة بهذا المستخدم فقط من قاعدة البيانات
            // نستخدم Include لجلب قائمة OrderItems_Use المرتبطة بكل طلب
            var orders = await _context.Orders_Use
                .Include(o => o.OrderItems)
                .Where(x => x.BuyerId == userId)
                .OrderByDescending(o => o.OrderDate) // الترتيب من الأحدث للأقدم
                .ToListAsync();

            // 3. التحقق مما إذا كان لدى المستخدم طلبات سابقة
            if (orders == null || !orders.Any())
            {
                return NotFound("لا يوجد لديك طلبات سابقة حتى الآن.");
            }

            return Ok(orders);
        }

    }
}