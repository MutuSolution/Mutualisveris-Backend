using System.Reflection;
using System.Text;

namespace Application.Extensions;
public static class OrderQueryBuilder
{
    public static string CreateOrderQuery<T>(string orderByQueryString)
    {
        var orderQueryBuilder = new StringBuilder();
        var propertyInfos = typeof(T)
           .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var orderParams = orderByQueryString.Trim().Split(',');

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param)) continue;
            var propertyFromQueryName = param.Split(' ')[0];
            var objectProperty = propertyInfos
                .FirstOrDefault(x => x.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
            if (objectProperty == null) continue;
            var direction = param.EndsWith(" desc") ? "descending" : "ascending";
            orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction}");
        }

        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
        return orderQuery;
    }
}
