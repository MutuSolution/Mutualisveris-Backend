using AutoMapper;
using Common.Responses.Addresses; // AddressResponse, vb.
using Common.Responses.Carts;    // CartResponse, CartItemResponse, vb.
using Common.Responses.Identity; // UserResponse, RoleResponse, RoleClaimViewModel, vb.
using Common.Responses.Orders;   // OrderResponse, OrderItemResponse, PaymentResponse, vb.
using Common.Responses.Products; // ProductResponse, LikeResponse, ProductImageResponse, vb.
using Domain;
using Domain.Responses; // Örneğin: CategoryResponse, ProductReportResponse, vb.
using Infrastructure.Models;

namespace Infrastructure
{
    internal class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Identity Mapping
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationRole, RoleResponse>();
            CreateMap<ApplicationRoleClaim, RoleClaimViewModel>().ReverseMap();

            // Product Mapping
            CreateMap<Product, ProductResponse>()
                // Ürünle ilişkili kategori varsa, kategori adını alıyoruz.
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                // Ürün resimlerinden URL listesini oluşturuyoruz.
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()))
                // Sipariş öğelerinin sayısını hesaplıyoruz.
                .ForMember(dest => dest.OrderItemCount, opt => opt.MapFrom(src => src.OrderItems.Count));

            // Ürün Resim Mapping (Varsayalım DTO'su var)
            CreateMap<ProductImage, ProductImageResponse>();

            // Category Mapping
            CreateMap<Category, CategoryResponse>()
             .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => src.IsVisible))
             .ForMember(dest => dest.ParentCategoryName, opt =>
             opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : string.Empty))
             .ForMember(dest => dest.ProductCount, opt =>
             opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
             .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
             .MaxDepth(3)
             .PreserveReferences();


            // Cart Mapping (Varsayalım CartResponse tanımlı)
            CreateMap<Cart, CartResponse>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // Cart Item Mapping (Varsayalım CartItemResponse tanımlı)
            CreateMap<CartItem, CartItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            // Address Mapping (Varsayalım AddressResponse tanımlı)
            CreateMap<Address, AddressResponse>();

            // Order Mapping (Varsayalım OrderResponse tanımlı)
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));

            // Order Item Mapping (Varsayalım OrderItemResponse tanımlı)
            CreateMap<OrderItem, OrderItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            // Payment Mapping (Varsayalım PaymentResponse tanımlı)
            CreateMap<Payment, PaymentResponse>();

            // Bu mapping profili, API'ye döndüreceğiniz DTO'ların (Response modellerinin) domain modellerinden doğru şekilde dönüştürülmesini sağlar.
        }
    }
}
