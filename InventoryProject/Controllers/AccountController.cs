using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}