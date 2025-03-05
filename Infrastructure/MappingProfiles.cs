using AutoMapper;
using Common.Responses.Identity;
using Common.Responses.Products;
using Domain;
using Infrastructure.Models;
using Domain.Responses; // CategoryResponse için

namespace Infrastructure
{
    internal class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationRole, RoleResponse>();
            CreateMap<Product, LikeResponse>();
            CreateMap<ProductReport, ProductReportResponse>().ReverseMap();
            CreateMap<ApplicationRoleClaim, RoleClaimViewModel>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();

            CreateMap<Category, CategoryResponse>()
            .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => src.isVisible))
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : string.Empty))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
            .MaxDepth(3) // Örneğin, 3 seviyeye kadar derinliği sınırlandırabilirsiniz.
            .PreserveReferences();


        }
    }
}
