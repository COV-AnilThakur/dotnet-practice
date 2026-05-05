using AutoMapper;
using Ecommerce_DBFirst.Models;
using Ecommerce_DBFirst.ViewModels;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map the Database Entity to the ViewModel
        CreateMap<Product, ProductVM>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty));

        // Map for create/edit form
        CreateMap<Product, ProductFormVM>();
        CreateMap<ProductFormVM, Product>()
            // Stock is managed via Inventory API module, not Product form.
            .ForMember(dest => dest.StockQuantity, opt => opt.Ignore());
    }
}
