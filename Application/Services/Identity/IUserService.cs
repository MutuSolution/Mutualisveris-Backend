using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using Infrastructure.Models;

namespace Application.Services.Identity;

public interface IUserService
{
    Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request);
    Task<IResponseWrapper> RegisterUserByAdminAsync(UserRegistrationRequest request);
    Task<IResponseWrapper> GetUserByIdAsync(string userId);
    Task<IResponseWrapper<UserResponse>> GetUserByEmailAsync(string email);
    Task<IResponseWrapper<UserResponse>> GetUserByUserNameAsync(string username);
    Task<IResponseWrapper> GetAllUsersAsync();
    Task<PaginationResult<ApplicationUser>> GetPagedUsersAsync(UserParameters parameters);
    Task<IResponseWrapper> DeleteAsync(DeleteUserByUsernameRequest request);
    Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request);
    Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request);
    Task<IResponseWrapper> ChangeUserEmailAsync(ChangeEmailRequest request);
    Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request);
    Task<IResponseWrapper> GetRolesAsync(string userId);
    Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest request);
}
