namespace E_Shop_1.Models
{
    public class Category
    {
  
        // المفتاح الرئيسي (Primary Key)
        public int Id { get; set; }

        // اسم التصنيف الذي سيظهر في المتجر
        public string Name { get; set; }

        // خاصية التنقل: قائمة المنتجات التي تنتمي لهذا التصنيف
        public ICollection<Product> Products { get; set; } = new List<Product>();
    
}
}
