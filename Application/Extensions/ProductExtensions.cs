using Domain;
using System.Linq.Dynamic.Core;

namespace Application.Extensions;

public static class ProductExtensions
{
    public static IQueryable<Product> SortProduct(this IQueryable<Product> products,
        string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return products.OrderBy(x => x.Id);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Product>(orderByQueryString);

        if (orderQuery is null) return products.OrderBy(x => x.Id);

        return products.OrderBy(orderQuery);
    }
}
