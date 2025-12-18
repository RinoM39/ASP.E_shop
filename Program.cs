using E_Shop_1.Data;
using E_Shop_1.Helpers;
using E_Shop_1.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using E_Shop_1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;



// تأكد من إضافة الـ using namespace الخاص بالـ DbContext إذا كان في مجلد مختلف (مثلاً: Data)

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddJsonOptions(options =>

 {
        // إضافة إعداد لمعالجة المراجع الدائرية (Circular References)
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
 });

builder.Services.AddAutoMapper(typeof(MappingProfiles));

// ... (بقية الخدمات)

// 1. قراءة سلسلة الاتصال
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. تسجيل ApplicationDbContext كخدمة
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // 3. تحديد المزود (SQL Server) واستخدام سلسلة الاتصال التي قرأناها
    options.UseSqlServer(connectionString);
});

// ربطها بالـ DbContext الخاص بك

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

//اضافة خدمة مصادقة JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 1. التحقق من المفتاح السري
        ValidateIssuerSigningKey = true,
        // قراءة المفتاح من ملف appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        builder.Configuration["JwtSettings:Secret"] // القراءة من قسم JwtSettings وحقل Secret
    )),

        // 2. التحقق من الناشر (Issuer)
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // القراءة من قسم JwtSettings وحقل Issuer

        // 3. التحقق من الجمهور (Audience)
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"], // القراءة من قسم JwtSettings وحقل Audience

        // 4. التحقق من الصلاحية
        ValidateLifetime = true
    };
});

// تسجيل خدمة ITokenService
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();
// ...






// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication(); 

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles(); // يسمح بالوصول للصور عبر الرابط

app.Run();
