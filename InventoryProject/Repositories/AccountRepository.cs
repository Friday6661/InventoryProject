using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        private readonly IEmailService _emailService;

        public AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _emailService = emailService;
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

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            // var confirmationLink = $"{_config["AppBaseUrl"]}/Confirm-Email?userId={WebUtility.UrlEncode(newUser.Id)}&token={WebUtility.UrlEncode(emailConfirmationToken)}";
            var confirmationLink = $"{_config["AppBaseUrl"]}/Confirm-Email?userId={Uri.EscapeDataString(newUser.Id)}&token={Uri.EscapeDataString(emailConfirmationToken)}";

            var emailConfirmationTemplate = _emailService.LoadEmailTemplate("Models/EmailTemplate/ActivationAccount.txt");
            var emailBody = string.Format(emailConfirmationTemplate, userDTO.UserName, confirmationLink);
            var emailDTO = new SendEmailDTO
            {
                ToEmail = userDTO.Email,
                Subject = "Account Activation",
                Body = emailBody
            };

            await _emailService.SendEmailAsync(emailDTO);

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

        public async Task<GeneralResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return new GeneralResponse(false, "User not found");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return new GeneralResponse(true, "Email Confirmed Successfully");
            }
            else
            {
                return new GeneralResponse(false, "Email confirmation failed");
            }
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

        public async Task<GeneralResponse> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return new GeneralResponse(false, "User not found");
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                return new GeneralResponse(true, "Password Reset Successfully");
            }
            else
            {
                return new GeneralResponse(false, "Password reset failed");
            }
        }

        public async Task<GeneralResponse> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return new GeneralResponse(false, "User not found");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            // var resetLink = $"{_config["AppBaseUrl"]}/Reset-Password?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(resetToken)}";
            var emailResetTemplate = _emailService.LoadEmailTemplate("Models/EmailTemplate/ResetPassword.txt"); 
            var emailBody = string.Format(emailResetTemplate, user.UserName, resetToken);
            var emailDTO = new SendEmailDTO
            {
                ToEmail = user.Email,
                Subject = "Reset Password Request",
                Body = emailBody
            };

            await _emailService.SendEmailAsync(emailDTO);
            return new GeneralResponse(true, "Password reset link sent to your email");
        }
    }
}