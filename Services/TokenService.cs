using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Shop_1.Services
{
    public class TokenService: ITokenService
    {
        // نحتاج لـ IConfiguration لقراءة الإعدادات من appsettings.json
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // يتم جلب المفتاح السري من الإعدادات وتحويله إلى كائن SymmetricSecurityKey
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
        }

        public string CreateToken(IdentityUser user)
        {
            // 1. إنشاء قائمة المطالبات (Claims)
            // وهي المعلومات التي سنضعها داخل التوكن (مثل اسم المستخدم والـ ID)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // 2. إنشاء بيانات الاعتماد للتوقيع (Signing Credentials)
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 3. وصف التوكن (Token Descriptor)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // صلاحية التوكن (مثلاً، يوم واحد)
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
                // قراءة الناشر والجمهور من الإعدادات
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"]
            };

            // 4. إنشاء التوكن الفعلي وتوليد الـ String
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
