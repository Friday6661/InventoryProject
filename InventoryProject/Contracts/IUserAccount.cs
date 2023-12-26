using InventoryProject.Models.DTOs;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccountAsync(UserDTO userDTO);
        Task<LoginResponse> LoginAccountAsync(LoginDTO loginDTO);
        Task<UserProfileResponse> GetLoggedInUserProfileAsync(HttpContext context);
    }
}