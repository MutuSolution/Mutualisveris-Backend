using Common.Requests.Orders;
using Common.Responses.Orders;
using Common.Responses.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services;

public interface IOrderService
{
    /// 🛍 Yeni bir sipariş oluşturur
    Task<IResponseWrapper<OrderResponse>> CreateOrderAsync(CreateOrderRequest request);

    /// 📦 Mevcut bir siparişi günceller
    Task<IResponseWrapper<OrderResponse>> UpdateOrderAsync(UpdateOrderRequest request);

    /// ❌ Siparişi siler (geri dönüş sadece başarı/hata mesajı olur)
    Task<IResponseWrapper> RemoveOrderAsync(int orderId);

    /// 🔎 Belirli bir siparişi getirir
    Task<IResponseWrapper<OrderResponse>> GetOrderByIdAsync(int orderId);

    /// 🔄 Belirli bir kullanıcının tüm siparişlerini getirir
    Task<IResponseWrapper<List<OrderResponse>>> GetUserOrdersAsync(string userId);

    /// 📊 Tüm siparişleri getirir (Admin yetkisi gerektirir)
    Task<IResponseWrapper<List<OrderResponse>>> GetAllOrdersAsync();
}
