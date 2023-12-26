using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryProject.Contracts;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Repositories
{
    public class AccountRepository : IUserAccount
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }


        public async Task<GeneralResponse> CreateAccountAsync(UserDTO userDTO)
        {
            if (userDTO is null) return new GeneralResponse(false, "Model is Empty");
            var newUser = new ApplicationUser()
            {
                UserName = userDTO.UserName,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                PasswordHash = userDTO.Password
            };

            var user = await _userManager.FindByEmailAsync(newUser.Email);
            if (user is not null) return new GeneralResponse(false, "User Registered already");

            var createUser = await _userManager.CreateAsync(newUser!, userDTO.Password);
            if(!createUser.Succeeded) return new GeneralResponse(false, "Error occured.. Please try again later");

            var checkManager = await _roleManager.FindByNameAsync("Manager");
            if (checkManager is null)
            {
                await _roleManager.CreateAsync(new IdentityRole() {Name = "Manager"});
                await _userManager.AddToRoleAsync(newUser, "Manager");
                return new GeneralResponse(true, "Account Created successfully");
            }
            else
            {
                var checkUser = await _roleManager.FindByNameAsync("User");
                if (checkUser is null) await _roleManager.CreateAsync(new IdentityRole() {Name = "User"});

                await _userManager.AddToRoleAsync(newUser, "User");
                return new GeneralResponse(true, "Account Created successfully");
            }
        }

        public async Task<LoginResponse> LoginAccountAsync(LoginDTO loginDTO)
        {
            if (loginDTO is null) return new LoginResponse(false, null!, "Login Container is empty");

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user is null) return new LoginResponse(false, null!, "User not found");
            
            bool checkUserPassword = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!checkUserPassword) return new LoginResponse(false, null!, "Invalid Email/Password");

            var userRole = await _userManager.GetRolesAsync(user);
            var userSession = new UserSession(user.Id, user.UserName, user.Email, userRole.First());
            string token = GenerateToken(userSession);
            return new LoginResponse(true, token!, "Login Successful");
        }

        public async Task<UserProfileResponse> GetLoggedInUserProfileAsync(HttpContext context)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new UserProfileResponse(false, null!, "User not authenticated");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new UserProfileResponse(false, null!, "User not found");
            }

            var userProfile = new UserProfileDTO
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            return new UserProfileResponse(true, userProfile, "User Profile Retrived Successfully");
        }

        private string GenerateToken(UserSession userSession)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userSession.Id),
                new Claim(ClaimTypes.Name, userSession.UserName),
                new Claim(ClaimTypes.Email, userSession.Email),
                new Claim(ClaimTypes.Role, userSession.Role)
            };
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}