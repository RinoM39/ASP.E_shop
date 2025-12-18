using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using E_Shop_1.Data;
using E_Shop_1.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Shop_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Categories___2__Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Categories___2__Controller(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<Categories___2__Controller>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            // 1. جلب جميع الفئات
            var categories = await _context.Categories.ToListAsync();

            // 2. تحويل الفئات إلى DTOs
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);

            // 3. إرجاع قائمة الـ DTOs
            return Ok(categoriesDto);
        }

        // GET api/<Categories___2__Controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Categories___2__Controller>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Categories___2__Controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Categories___2__Controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
