using System.ComponentModel.DataAnnotations;

namespace E_Shop_1.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تكون كلمة المرور 6 أحرف على الأقل.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        public string Email { get; set; }
    }
}
