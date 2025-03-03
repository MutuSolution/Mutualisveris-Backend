using AutoMapper;
using Common.Request.Category;
using Common.Requests.Products;
using Common.Responses.Products;
using Domain;
using Domain.Responses;

namespace Application;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        //responses
        CreateMap<Product, ProductResponse>().ReverseMap();
        CreateMap<Category, CategoryResponse>().ReverseMap();
        CreateMap<Like, ProductResponse>().ReverseMap();
        CreateMap<Like, LikeResponse>().ReverseMap();
        CreateMap<ProductReport, ProductReportResponse>().ReverseMap();

        //requests
        CreateMap<ProductReportRequest, ProductReport>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<CreateCategoryRequest, Category>();

    }
}
