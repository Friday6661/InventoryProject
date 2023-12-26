using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InventoryProject.Contracts;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryProject.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly IUserAccount _userAccount;
        public AccountController(IUserAccount userAccount)
        {
            _userAccount = userAccount;
        }

        [HttpGet("Confirm-Email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string token)
        {
            // userId = WebUtility.UrlDecode(userId);
            // token = WebUtility.UrlDecode(token);
            var response = await _userAccount.ConfirmEmailAsync(userId, token);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var response = await _userAccount.CreateAccountAsync(userDTO);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await _userAccount.LoginAccountAsync(loginDTO);
            return Ok(response);
        }

        [HttpGet("Profile")]
        [Authorize]
        public async Task<IActionResult> GetLoggedInUserProfile()
        {
            var userProfileResponse = await _userAccount.GetLoggedInUserProfileAsync(HttpContext);

            if (!userProfileResponse.Flag)
            {
                return BadRequest(new {message = userProfileResponse.Message} );
            }
            return Ok(new {userProfile = userProfileResponse});
        }

        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var response = await _userAccount.ForgotPassword(forgotPasswordDTO.Email);
            return Ok(response);
        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var response = await _userAccount.ResetPasswordAsync(resetPasswordDTO.email, resetPasswordDTO.Token, resetPasswordDTO.newPassword);
            return Ok(response);
        }
    }
}