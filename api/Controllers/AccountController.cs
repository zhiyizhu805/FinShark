using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private ITokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager,ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (createdUser.Succeeded) 
                //createdUser.Succeeded: is a boolean indicating whether the user creation was successful. .CreateAsync() returns an IdentityResult, and Succeeded is true if the operation completed without errors.
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    //UserManager<AppUser> is a service provided by ASP.NET Core Identity, injected via dependency injection（through the AddIdentity method in builder.Services.）
                    if (roleResult.Succeeded)
                    {
                        return Ok(

                            new NewUserDto{
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = _tokenService.CreateToken(appUser), // creates and returns a new token to the client for immediate use without needing to save it in a database.
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }

            }
            catch (Exception e)
            {
                return StatusCode(500,e);
            }

        }
    }
}