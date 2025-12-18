using AutoMapper;
using E_Shop_1.Models;
using E_Shop_1.DTOs;


namespace E_Shop_1.Helpers

{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category.Name));

            // 2. التعديل: تحويل من DTO إلى Model (لـ POST/PUT)
            CreateMap<ProductForCreationDto, Product>();

            CreateMap<Category, CategoryDto>();

        }
    }
}
