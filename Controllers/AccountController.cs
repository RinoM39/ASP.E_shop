using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Shop_1.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using E_Shop_1.Services;

namespace E_Shop_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // تعريف متغيرات الخدمات
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        // Constructor: حقن التبعية
        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, ITokenService tokenService)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }


        // POST: api/Account/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // 1. إنشاء كائن المستخدم
            var user = new IdentityUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            // 2. محاولة إنشاء المستخدم في قاعدة البيانات
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            // بعد نجاح إنشاء المستخدم (result.Succeeded)
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                // إنشاء الأدوار إذا لم تكن موجودة في قاعدة البيانات
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            // إضافة المستخدم الجديد لدور "User" تلقائياً
            await _userManager.AddToRoleAsync(user, "User");
            // 4. في حالة الفشل، نرجع الأخطاء
            return BadRequest(result.Errors);
        }

        // POST: api/Account/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // 1. البحث عن المستخدم باسم المستخدم
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null)
            {
                return Unauthorized(new { Message = "اسم المستخدم أو كلمة المرور غير صحيحين." });
            }

            // 2. محاولة تسجيل الدخول باستخدام كلمة المرور (دون إنشاء كوكيز)
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded)
            {
                // 🚀 الخطوة الجديدة: إنشاء التوكن وإرجاعه 🚀
                var token = _tokenService.CreateToken(user);

                // 3. إذا نجح التحقق، ننشئ رمز JWT ونرجعه
                // return Ok(new { Token = _tokenService.CreateToken(user) }); // (سنتعامل معها لاحقاً)
                return Ok(new
                {
                    Token = token, // إرجاع الرمز المميز
                    Username = user.UserName // إرجاع بيانات بسيطة إضافية
                });
            }
                return Unauthorized(new { Message = "اسم المستخدم أو كلمة المرور غير صحيحين." });
            
        }
    }
}
