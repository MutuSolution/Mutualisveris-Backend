using Application.Services;
using Application.Services.Identity;
using AutoMapper;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using Domain.Responses;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService currentUserService;
    private readonly IMapper _mapper;


    public CategoryService(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
    {
        _context = context;
        this.currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Name == category.Name);
        if (categoryInDb is not null)
            await ResponseWrapper.FailAsync("[ML63] Category already taken.");
     
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public Task<int> DeleteProductAsync(CategoryResponse category)
    {
        throw new NotImplementedException();
    }

    public Task<CategoryResponse> GetCategoryByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<PaginationResult<CategoryResponse>> GetPagedCategoriesAsync(CategoryParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<IResponseWrapper> SoftDeleteCategory(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Category> UpdateCategoryAsync(CategoryResponse category)
    {
        throw new NotImplementedException();
    }
}
