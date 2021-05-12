using DotNet_RPG.Data;
using DotNet_RPG.Dtos.User;
using DotNet_RPG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_RPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepos;

        public AuthController(IAuthRepository auth)
        {
            _authRepos = auth;
        }



        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            ServiceResponse<int> response = await _authRepos.Register(
                new User
                {
                    Username = request.Username
                }, 
                request.Password
                
                );

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }







        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            ServiceResponse<string> response = await _authRepos.Login(
                request.Username,
                request.Password
                );

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


    }
}
