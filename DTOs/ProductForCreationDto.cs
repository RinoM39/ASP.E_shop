using System.ComponentModel.DataAnnotations;
namespace E_Shop_1.DTOs

{
    public class ProductForCreationDto
    {

        // Name: مطلوب (نستخدم Data Annotations للتحقق)
        [Required(ErrorMessage = "يجب إدخال اسم المنتج.")]
        [MaxLength(255)]
        public string Name { get; set; }

        // Price: مطلوب
        [Required(ErrorMessage = "يجب تحديد سعر المنتج.")]
        public decimal Price { get; set; }

        // Description: اختياري
        public string? Description { get; set; }

        // ImageUrl: اختياري
        public string? ImageUrl { get; set; }

        // CategoryId: مطلوب لربط المنتج بالفئة
        [Required(ErrorMessage = "يجب تحديد رقم الفئة المرتبط.")]
        public int CategoryId { get; set; }


    }
}
