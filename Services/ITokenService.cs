using Microsoft.AspNetCore.Identity;

namespace E_Shop_1.Services
{
    public interface ITokenService
    {
        // تستقبل كائن المستخدم وترجع رمز (Token) كـ String
        string CreateToken(IdentityUser user);
    }
}
