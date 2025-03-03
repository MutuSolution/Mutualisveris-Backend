using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Responses.Pagination;

public class CategoryParameters : PaginationParams
{
    public bool IsVisible { get; set; }

    public CategoryParameters()
    {
        OrderBy = "id";
    }
}
