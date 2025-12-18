namespace E_Shop_1.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        // المفاتيح الأجنبية
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; } // السعر وقت إنشاء الطلب

        // خاصيات التنقل
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
