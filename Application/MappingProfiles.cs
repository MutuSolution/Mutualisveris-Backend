using AutoMapper;
using Common.Requests.Products;
using Common.Responses.Products;
using Domain;

namespace Application;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateProductRequest, Product>();
        CreateMap<ProductReportRequest, ProductReport>();
        CreateMap<Product, ProductResponse>().ReverseMap();
        CreateMap<Like, LikeResponse>().ReverseMap();
        CreateMap<Like, ProductResponse>().ReverseMap();
        CreateMap<ProductReport, ProductReportResponse>().ReverseMap();

    }
}
