using Application.Extensions;
using Application.Services;
using Application.Services.Identity;
using Common.Requests.Product.Report;
using Common.Requests.Products;
using Common.Responses.Pagination;
using Common.Responses.Products;
using Common.Responses.Wrappers;
using Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ProductService(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ProductReport> ReportProductAsync(ProductReport productReport)
    {
        await _context.ProductReports.AddAsync(productReport);
        await _context.SaveChangesAsync();
        return productReport;
    }
    public async Task<Product> CreateProductAsync(Product product)
    {
        var productInDb = await _context.Products.FirstOrDefaultAsync(x => x.Name == product.Name);
        if (productInDb != null)
        {
            var isReportedProduct = await _context.ProductReports
                .FirstOrDefaultAsync(x => x.ProductId == productInDb.Id && x.IsChecked);
            if (isReportedProduct != null)
            {
                throw new InvalidOperationException("[ML116] Product is not allowed.");
            }
        }
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<int> DeleteProductAsync(ProductResponse productResponse)
    {
        var product = await _context.Products.FindAsync(productResponse.Id);
        if (product == null)
        {
            throw new ArgumentException("[ML117] Product not found.");
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }

    public async Task<ProductResponse> GetProductByIdAsync(int id)
    {
        var productInDb = await _context.Products
            .Where(product => product.Id == id).Select(product => new ProductResponse
            {
                Id = product.Id,
                Title = product.Name,
                Description = product.Description,
                IsPublic = product.IsPublic,
                IsDeleted = product.IsDeleted,
                LikeCount = product.LikeCount
            })
            .FirstOrDefaultAsync();
        return productInDb;
    }

    public async Task<List<ProductResponse>> GetProductListAsync()
    {
        var likedProductIds = _context.Likes
       .Where(x => x.UserName == _currentUserService.UserName)
       .Select(x => x.ProductId)
       .ToHashSet();

        return await _context.Products.Select(product => new ProductResponse
        {
            Id = product.Id,
            Title = product.Name,
            Description = product.Description,
            IsPublic = product.IsPublic,
            IsDeleted = product.IsDeleted,
            LikeCount = product.LikeCount,
            IsLiked = likedProductIds.Contains(product.Id)
        }).ToListAsync();
    }
    public async Task<List<Product>> GetHomeProductListAsync()
    {
        return await _context.Products
            .Where(x => (x.IsPublic == true) && (x.IsDeleted == false))
            .OrderByDescending(x => x.LikeCount)
            .Take(25)
            .ToListAsync();
    }
    public async Task<List<Product>> GetPublicProductWithUsernameAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            return new List<Product>();
        return await _context.Products
            .Where(x =>
            (x.IsPublic == true) &&
            (x.IsDeleted == false) &&
            (x.Name.ToLower() == userName.ToLower()))
            .OrderByDescending(x => x.LikeCount)
            .Take(25)
            .ToListAsync();
    }
    public async Task<Product> UpdateProductAsync(ProductResponse productResponse)
    {
        var product = await _context.Products.FindAsync(productResponse.Id);
        if (product == null)
        {
            throw new ArgumentException("[ML118] Product not found.");
        }

        product.Name = productResponse.Title;
        product.Description = productResponse.Description;
        product.IsPublic = productResponse.IsPublic;
        product.IsDeleted = productResponse.IsDeleted;
        product.LikeCount = productResponse.LikeCount;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<PaginationResult<ProductResponse>> GetPagedProductsAsync(ProductParameters parameters)
    {
        var likedProductIds = _context.Likes
    .Where(x => x.UserName == _currentUserService.UserName).Select(x => x.ProductId).ToHashSet();
        var searhTerm = CleanSearchTerm(parameters.SearchTerm);
        var query = _context.Set<Product>().AsQueryable()
            .Where(x =>
                // Filtering
                (x.IsPublic == parameters.IsPublic) &&
                (x.LikeCount >= parameters.MinLikeCount) &&
                (x.IsDeleted == parameters.IsDeleted) &&
                (string.IsNullOrEmpty(searhTerm) ||
                // Searching with case-insensitive comparison
                x.Name.ToLower().Contains(searhTerm.ToLower()) ||
                x.Description.ToLower().Contains(searhTerm.ToLower())))
            .SortProduct(parameters.OrderBy);

        var totalCount = await query.CountAsync();
        var totalPage = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / parameters.ItemsPerPage) : 0;
        if (totalCount == 0) parameters.ItemsPerPage = 0;

        var items = await query
                .Skip(parameters.Skip)
                .Take(parameters.ItemsPerPage)
                 .Select(product => new ProductResponse
                 {
                     Id = product.Id,
                     Title = product.Name,
                     Description = product.Description,
                     IsPublic = product.IsPublic,
                     IsDeleted = product.IsDeleted,
                     LikeCount = product.LikeCount,
                     IsLiked = likedProductIds.Contains(product.Id)
                 })
                .ToListAsync();

        return new PaginationResult<ProductResponse>(items, totalCount, totalPage, parameters.Page, parameters.ItemsPerPage);
    }

    public async Task<PaginationResult<ProductResponse>> GetPagedProductsByUserNameAsync(ProductsByUserNameParameters parameters)
    {
        var likedProductIds = _context.Likes
     .Where(x => x.UserName == _currentUserService.UserName).Select(x => x.ProductId).ToHashSet();
        var searhTerm = CleanSearchTerm(parameters.SearchTerm);

        var query = _context.Set<Product>().AsQueryable()
    .Where(x =>
        // Filtering
        (string.IsNullOrEmpty(parameters.IsPublic) ||
        (parameters.IsPublic.ToLower() == "true" && x.IsPublic == true) ||
        (parameters.IsPublic.ToLower() == "false" && x.IsPublic == false)) &&
        (x.Name == parameters.UserName) &&
        (x.LikeCount >= parameters.MinLikeCount) &&
        (x.IsDeleted == parameters.IsDeleted) &&
        (string.IsNullOrEmpty(searhTerm) ||
        // Searching with case-insensitive comparison
        x.Name.ToLower().Contains(searhTerm.ToLower()) ||
        x.Description.ToLower().Contains(searhTerm.ToLower())));

        query = query.SortProduct(parameters.OrderBy);

        var totalCount = await query.CountAsync();
        var totalPage = totalCount > 0 ?
            (int)Math.Ceiling((double)totalCount / parameters.ItemsPerPage) : 0;
        if (totalCount == 0) parameters.ItemsPerPage = 0;

        var items = await query
                .Skip(parameters.Skip)
                .Take(parameters.ItemsPerPage)
                 .Select(product => new ProductResponse
                 {
                     Id = product.Id,
                     Title = product.Name,
                     Description = product.Description,
                     IsPublic = product.IsPublic,
                     IsDeleted = product.IsDeleted,
                     LikeCount = product.LikeCount,
                     IsLiked = likedProductIds.Contains(product.Id)
                 })
                .ToListAsync();

        return new PaginationResult<ProductResponse>(items, totalCount, totalPage, parameters.Page, parameters.ItemsPerPage);
    }



    public async Task<PaginationResult<ProductResponse>> GetPagedLikesByUserNameAsync(LikesByUserNameParameters parameters)
    {
        var likedProductIds = _context.Likes
       .Where(x => x.UserName == _currentUserService.UserName).Select(x => x.ProductId).ToHashSet();
        var searhTerm = CleanSearchTerm(parameters.SearchTerm);

        var query = _context.Set<Like>().Include(l => l.Product).AsQueryable()
            .Where(x =>
                // Filtering
                (x.UserName == parameters.UserName) &&
                (string.IsNullOrEmpty(searhTerm) ||
                // Searching with case-insensitive comparison
                x.UserName.ToLower().Contains(searhTerm.ToLower())
              ));

        query = query.SortLike(parameters.OrderBy);

        var totalCount = await query.CountAsync();
        var totalPage = totalCount > 0 ?
            (int)Math.Ceiling((double)totalCount / parameters.ItemsPerPage) : 0;
        if (totalCount == 0) parameters.ItemsPerPage = 0;

        var items = await query
                .Skip(parameters.Skip)
                .Take(parameters.ItemsPerPage)
                 .Select(product => new ProductResponse
                 {
                     Id = product.Id,
                     Title = product.Product.Name,
                     Description = product.Product.Description,
                     IsPublic = product.Product.IsPublic,
                     IsDeleted = product.Product.IsDeleted,
                     LikeCount = product.Product.LikeCount,
                     IsLiked = likedProductIds.Contains(product.Product.Id)
                 })
                .ToListAsync();

        return new PaginationResult<ProductResponse>(items, totalCount, totalPage, parameters.Page, parameters.ItemsPerPage);
    }

    public async Task<IResponseWrapper> SoftDeleteProduct(SoftDeleteProductRequest request)
    {
        var productInDb = _context.Products.Find(request.ProductId);
        productInDb.IsDeleted = true;
        _context.Products.Update(productInDb);
        await _context.SaveChangesAsync();
        return ResponseWrapper.Success("[ML74] Product successfully deleted.");
    }

    public async Task<IResponseWrapper> LikeProductAsync(LikeProductRequest request, CancellationToken cancellationToken)
    {
        var isLiked = await _context.Likes.FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.UserName == _currentUserService.UserName);
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);

        if (product == null)
            return await ResponseWrapper.FailAsync("[ML82] Product does not found.");

        if (isLiked != null)
        {
            product.LikeCount -= 1;
            _context.Likes.Remove(isLiked);
            await _context.SaveChangesAsync(cancellationToken);
            return ResponseWrapper.Success("[ML83] Product successfully unliked.");
        }


        var like = new Like
        {
            UserName = _currentUserService.UserName,
            ProductId = request.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Likes.Add(like);
        product.LikeCount += 1;
        await _context.SaveChangesAsync(cancellationToken);

        return ResponseWrapper.Success("[ML79] Product successfully liked.");

    }

    public async Task<List<ProductReportResponse>> GetProductReportsAsync()
    {
        var productReports = await _context.ProductReports
            .Where(x => x.IsChecked == false)
            .OrderByDescending(report => report.Id)
            .ToListAsync();

        return productReports.Select(report => new ProductReportResponse
        {
            Id = report.Id,
            ProductId = report.ProductId,
            Message = report.Message,
            IsChecked = report.IsChecked
        }).ToList();
    }



    public string CleanSearchTerm(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return searchTerm;
        }

        return searchTerm.Replace("#", "");
    }

    public Task<ProductReportResponse> UpdateReportProductAsync(ProductReportIsCheckedRequest request)
    {
        var productReport = _context.ProductReports.FirstOrDefault(x => x.Id == request.ReportId);
        if (productReport == null)
        {
            return Task.FromResult(new ProductReportResponse());
        }
        productReport.IsChecked = request.IsChecked;
        _context.ProductReports.Update(productReport);
        _context.SaveChanges();
        return Task.FromResult(new ProductReportResponse
        {
            ProductId = productReport.ProductId,
            Message = productReport.Message,
            IsChecked = productReport.IsChecked
        });
    }
}

