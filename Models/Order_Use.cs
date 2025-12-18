using System;
using System.Collections.Generic;

namespace E_Shop_1.Models
{
    public class Order_Use
    {
        public int Id { get; set; }
        public string BuyerId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";

        // استخدام النوع المخصص الذي اخترته أنت
        public List<OrderItem_Use> OrderItems { get; set; } = new();
    }

    public class OrderItem_Use
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
    }
}