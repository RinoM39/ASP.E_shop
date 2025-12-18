namespace E_Shop_1.Models
{
    public class Order
    {
        public int Id { get; set; }

        // في ASP.NET Identity، غالباً ما يكون هذا المفتاح من نوع string
        public string UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } // مثال: "Pending", "Paid", "Shipped"

        public string ShippingAddress { get; set; }

        // خاصية التنقل: قائمة عناصر الطلب
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
