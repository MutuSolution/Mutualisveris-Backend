using Domain;
using System.Linq.Dynamic.Core;

namespace Application.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> SortById<T>(this IQueryable<T> query, string orderByQueryString)
        {
            // Eğer orderBy sorgusu boşsa, varsayılan olarak "Id" üzerinden sıralar.
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return query.OrderBy("Id");

            // OrderQueryBuilder ile dinamik sıralama ifadesi oluşturuluyor.
            var orderQuery = OrderQueryBuilder.CreateOrderQuery<T>(orderByQueryString);

            // Eğer sıralama ifadesi oluşturulamazsa yine varsayılan "Id" sıralaması yapılır.
            if (string.IsNullOrWhiteSpace(orderQuery))
                return query.OrderBy("Id");

            return query.OrderBy(orderQuery);
        }


        public static IQueryable<ApplicationUser> SortUser(this IQueryable<ApplicationUser> users, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return users.OrderBy(x => x.UserName);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<ApplicationUser>(orderByQueryString);

            if (orderQuery is null) return users.OrderBy(x => x.UserName);

            return users.OrderBy(orderQuery);
        }
    }
}
