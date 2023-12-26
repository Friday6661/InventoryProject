using InventoryProject.Models.DTOs;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccountAsync(UserDTO userDTO);
        Task<LoginResponse> LoginAccountAsync(LoginDTO loginDTO);
        Task<UserProfileResponse> GetLoggedInUserProfileAsync(HttpContext context);
        Task<GeneralResponse> ConfirmEmailAsync(string userId, string token);
        Task<GeneralResponse> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<GeneralResponse> ForgotPassword(string email);
    }
}