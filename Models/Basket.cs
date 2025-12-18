using Microsoft.EntityFrameworkCore; // لعمل Include و ToListAsync
using Microsoft.AspNetCore.Authorization; // لاستخدام [Authorize]
using Microsoft.AspNetCore.Mvc; // لاستخدام ControllerBase
using E_Shop_1.Models; // لكي يعرف الكنترولر ما هو الـ Product والـ Basket
using System.Linq; // 👈 مهم جداً لعمليات البحث
using System.Threading.Tasks; // 👈 مهم لاستخدام async/await

namespace E_Shop_1.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public string BuyerId { get; set; } // هذا هو الـ ID الخاص بالمستخدم من Identity
        public List<BasketItem> Items { get; set; } = new(); // قائمة المنتجات داخل السلة
    }

    public class BasketItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } // الكمية المطلوبة

        // ربط العنصر بالمنتج
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // ربط العنصر بالسلة
        public int BasketId { get; set; }
    }
}
