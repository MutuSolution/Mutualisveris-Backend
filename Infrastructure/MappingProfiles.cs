using AutoMapper;
using Common.Request.Category;
using Common.Requests.Products;
using Common.Responses.Addresses;
using Common.Responses.Cart;
using Common.Responses.Identity;
using Common.Responses.Orders;
using Common.Responses.Products;
using Domain;
using Domain.Responses;

namespace Infrastructure;

internal class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // ✅ Identity Mapping
        CreateMap<ApplicationUser, UserResponse>();
        CreateMap<ApplicationRole, RoleResponse>();
        CreateMap<ApplicationRoleClaim, RoleClaimViewModel>().ReverseMap();

        // ✅ Requests Mapping (Create & Update)
        CreateMap<CreateProductRequest, Product>().ReverseMap();
        CreateMap<CreateCategoryRequest, Category>().ReverseMap();
        CreateMap<UpdateCategoryRequest, Category>().ReverseMap();
        CreateMap<UpdateProductRequest, Product>().ReverseMap();

        // ✅ Product Mapping
        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()))
            .ForMember(dest => dest.OrderItemCount, opt => opt.MapFrom(src => src.OrderItems.Count))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0));

        // ✅ Product Image Mapping
        CreateMap<ProductImage, ProductImageResponse>();

        // ✅ Category Mapping (Daha Güvenli ve Optimize)
        CreateMap<Category, CategoryResponse>()
            .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => src.IsVisible))
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : string.Empty))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories ?? new List<Category>()))
            .MaxDepth(2)
            .PreserveReferences();

        // ✅ 🛠 **Cart Mapping Güncellendi!**
        CreateMap<Cart, CartResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items != null ? src.Items : new List<CartItem>())) // 🛠 Null listesi için önlem
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Items != null ? src.Items.Sum(i => i.Quantity * i.UnitPrice) : 0)); // 🛠 Null check

        // ✅ 🛠 **CartItem Mapping Güncellendi!**
        CreateMap<CartItem, CartItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Bilinmeyen Ürün")); // 🛠 Null check

        // ✅ Address Mapping
        CreateMap<Address, AddressResponse>();

        // ✅ Order Mapping (Enum Dönüşümü Optimize Edildi)
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems ?? new List<OrderItem>()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.GetName(typeof(OrderStatus), src.Status)));  // Daha performanslı Enum Dönüşümü

        // ✅ Order Item Mapping
        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

        // ✅ Payment Mapping
        CreateMap<Payment, PaymentResponse>();
    }
}
