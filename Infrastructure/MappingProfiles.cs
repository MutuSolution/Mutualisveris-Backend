using AutoMapper;
using Common.Requests.Addresses;
using Common.Requests.Category;
using Common.Requests.Payments;
using Common.Requests.Products;
using Common.Responses.Addresses;
using Common.Responses.Cart;
using Common.Responses.Identity;
using Common.Responses.Orders;
using Common.Responses.Payments;
using Common.Responses.Products;
using Domain;
using Domain.Entities;
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

        // ✅ Category Mapping (Optimize Edildi)
        CreateMap<Category, CategoryResponse>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : string.Empty))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories ?? new List<Category>()))
            .MaxDepth(2)
            .PreserveReferences();

        // ✅ Cart Mapping
        CreateMap<Cart, CartResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items ?? new List<CartItem>()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Items != null ? src.Items.Sum(i => i.Quantity * i.UnitPrice) : 0));

        // ✅ CartItem Mapping
        CreateMap<CartItem, CartItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Bilinmeyen Ürün"));

        // ✅ Address Mapping
        CreateMap<Address, AddressResponse>();
        CreateMap<CreateAddressRequest, Address>();
        CreateMap<UpdateAddressRequest, Address>();

        // ✅ Order Mapping
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User != null ? src.User : null)) // ✅ Kullanıcı bilgilerini ekledik
            .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
            .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.BillingAddress))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems ?? new List<OrderItem>()));

        // ✅ Order Item Mapping
        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Bilinmeyen Ürün"))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.Product != null ? src.Product.SKU : "N/A"))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product != null && src.Product.Images.Any() ? src.Product.Images.FirstOrDefault().ImageUrl : string.Empty));

        // ✅ Payment Mapping
        CreateMap<Payment, PaymentResponse>();
        CreateMap<PaymentRequest, Payment>();
    }
}
