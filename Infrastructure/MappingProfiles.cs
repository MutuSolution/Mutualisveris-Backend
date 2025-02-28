using AutoMapper;
using Common.Responses.Identity;
using Common.Responses.Products;
using Domain;
using Infrastructure.Models;

namespace Infrastructure;

internal class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ApplicationUser, UserResponse>();
        CreateMap<ApplicationRole, RoleResponse>();
        CreateMap<Product, LikeResponse>();
        CreateMap<ProductReport, ProductReportResponse>().ReverseMap();
        CreateMap<ApplicationRoleClaim, RoleClaimViewModel>().ReverseMap();
    }
}
