using Domain;
using Infrastructure.Models;
using System.Linq.Dynamic.Core;

namespace Application.Extensions;

public static class UserExtensions
{
    public static IQueryable<ApplicationUser> SortUser(this IQueryable<ApplicationUser> users, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return users.OrderBy(x => x.UserName);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<ApplicationUser>(orderByQueryString);

        if (orderQuery is null) return users.OrderBy(x => x.UserName);

        return users.OrderBy(orderQuery);
    }
}