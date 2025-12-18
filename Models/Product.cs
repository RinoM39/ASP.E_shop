namespace E_Shop_1.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        // استخدام Decimal للسعر أمر ضروري ودقيق للعمليات المالية
        public decimal Price { get; set; }

        public int StockQuantity { get; set; } // كمية المخزون

        public string ImageUrl { get; set; }

        // المفتاح الأجنبي لربط المنتج بالتصنيف
        public int? CategoryId { get; set; }

        // خاصيات التنقل
        public Category? Category { get; set; } // يمثل التصنيف الذي ينتمي إليه

        // يمثل علاقة المنتجات بعناصر الطلبات
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
}
}
