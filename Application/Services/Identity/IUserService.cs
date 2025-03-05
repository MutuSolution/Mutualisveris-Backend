using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Identity
{
    public interface IUserService
    {
        Task<IResponseWrapper<string>> RegisterUserAsync(UserRegistrationRequest request);
        Task<IResponseWrapper<string>> RegisterUserByAdminAsync(UserRegistrationRequest request);
        Task<IResponseWrapper<UserResponse>> GetUserByIdAsync(string userId);
        Task<IResponseWrapper<UserResponse>> GetUserByEmailAsync(string email);
        Task<IResponseWrapper<UserResponse>> GetUserByUserNameAsync(string username);
        Task<IResponseWrapper<List<UserResponse>>> GetAllUsersAsync();
        Task<IResponseWrapper<PaginationResult<UserResponse>>> GetPagedUsersAsync(UserParameters parameters);
        Task<IResponseWrapper<string>> DeleteAsync(DeleteUserByUsernameRequest request);
        Task<IResponseWrapper<string>> UpdateUserAsync(UpdateUserRequest request);
        Task<IResponseWrapper<string>> ChangeUserPasswordAsync(ChangePasswordRequest request);
        Task<IResponseWrapper<string>> ChangeUserEmailAsync(ChangeEmailRequest request);
        Task<IResponseWrapper<string>> ChangeUserStatusAsync(ChangeUserStatusRequest request);
        Task<IResponseWrapper<List<UserRoleViewModel>>> GetRolesAsync(string userId);
        Task<IResponseWrapper<string>> UpdateUserRolesAsync(UpdateUserRolesRequest request);
    }
}
