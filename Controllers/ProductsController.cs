using AutoMapper;
using E_Shop_1.Data; 
using E_Shop_1.DTOs;
using E_Shop_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Shop_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // تعريف متغير خاص لقاعدة البيانات (DbContext)
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper; // خاصية الـ AutoMapper
                                          // Constructor: حقن التبعية (Dependency Injection) لجلب الـ DbContext
        public ProductsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        // ...  GET

        // GET: /api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {



            // 1. نبدأ بالاستعلام الأساسي مع تضمين التصنيف
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // 2. إذا كان هناك نص للبحث، نقوم بالفلترة
            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower)
                                      || p.Description.ToLower().Contains(searchLower));
            }

            // 3. فلترة الحد الأدنى للسعر
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            // 4. فلترة الحد الأعلى للسعر
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var products = await query.ToListAsync(); // ✅ هنا نخبر البرنامج أن يستخدم الاستعلام الذي يحتوي على البحث

            // تحويل المنتجات إلى DTOs
            var productsDto = _mapper.Map<List<ProductDto>>(products);

            // إرجاع قائمة المنتجات مع كود HTTP 200 OK
            return Ok(productsDto);


        }

        // GET: /api/Products/5
        // {id} هو متغير مسار، على سبيل المثال 5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            // البحث عن المنتج باستخدام FindAsync (أو FirstOrDefaultAsync)
            var product = await _context.Products
                                 .Include(p => p.Category) 
                                 .FirstOrDefaultAsync(p => p.Id == id);

            // التحقق مما إذا كان المنتج موجودًا
            if (product == null)
            {
                return NotFound(); // إذا لم يوجد المنتج، نرجع خطأ 404 Not Found
            }

            // إرجاع المنتج مع كود HTTP 200 OK
            return Ok(product);
        }



        // POST: /api/Products 
        // لاستقبال البيانات من الـ Body
        [HttpPost]
        public async Task<IActionResult> PostProduct(ProductForCreationDto productDto)
        {
            // التحقق من صلاحية البيانات المرسلة (الـ Validation)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // إذا كانت البيانات غير صالحة، نرجع خطأ 400
            }
            var product = _mapper.Map<Product>(productDto); // <<<< التغيير 2
            // 1. إضافة المنتج إلى ذاكرة التتبع في DbContext
            _context.Products.Add(product);

            // 2. حفظ التغييرات فعلياً في قاعدة البيانات (SQL Server)
            await _context.SaveChangesAsync();

            // 3. إرجاع استجابة 201 Created مع رابط المنتج الذي تم إنشاؤه
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }





        // PUT: /api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductForCreationDto productDto)
        {

            // 2. البحث عن الكائن القديم أولاً في قاعدة البيانات
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                // إذا لم يتم العثور عليه (وهو سبب خطأ 404 لديك)، نرجع 404 مباشرة.
                return NotFound();
            }

            // 2. استخدام الـ Mapper لنسخ الخصائص من الـ DTO إلى الكائن الموجود
            _mapper.Map(productDto, existingProduct); // <<<< التغيير 2: تحديث الخصائص تلقائياً

           

            // 4. تعيين حالة الكائن إلى Modified (معظم الأحيان غير مطلوب بعد التحديث اليدوي)
            // _context.Entry(existingProduct).State = EntityState.Modified; 

            // 5. حفظ التغييرات (EF Core الآن يعرف أن existingProduct تغير)
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // إذا لم يتم العثور عليه، فهو NotFound، ولكننا تحققنا من ذلك في الخطوة 2
                throw;
            }

            return NoContent();
        }


        // DELETE: /api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // 1. البحث عن المنتج المراد حذفه
            var product = await _context.Products.FindAsync(id);

            // 2. التحقق من وجود المنتج
            if (product == null)
            {
                return NotFound(); // خطأ 404 إذا لم يتم العثور عليه
            }

            // 3. إزالة المنتج من مجموعة DbSet (في الذاكرة)
            _context.Products.Remove(product);

            // 4. تطبيق الحذف على قاعدة البيانات
            await _context.SaveChangesAsync();

            // 5. إرجاع كود 204 No Content (للإشارة إلى نجاح العملية بدون إرجاع محتوى)
            return NoContent();
        }


        // ---------------------tastttttt---------------
        [HttpGet("BMW")]
        [Authorize] // <--- هذه السمة هي التي تحمي الواجهة
        public IActionResult GetSecureData()
        {
            // يمكنك هنا الوصول لبيانات المستخدم عبر User.Identity
            return Ok(new { Message = "بيانات سرية! لقد نجح التوكن." });
        }
//----------------------------------------------------------------------------
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("الرجاء اختيار صورة صالحة");

            // 1. تحديد مسار الحفظ (wwwroot/images)
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // إنشاء المجلد إذا لم يكن موجوداً
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // 2. إنشاء اسم فريد للملف
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            // 3. حفظ الصورة فعلياً
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 4. توليد رابط الصورة للوصول إليه عبر المتصفح
            var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";

            return Ok(new { ImageUrl = imageUrl });
        }



    }
}
