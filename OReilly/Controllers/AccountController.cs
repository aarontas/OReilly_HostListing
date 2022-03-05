using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OReilly.Data;
using OReilly.Models;
using OReilly.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //Part of the identity core. The ApiUser is the same class that we defined in the services (startup.cs)
        private readonly UserManager<ApiUser> _userManager;
        //private readonly SignInManager<ApiUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        public AccountController(UserManager<ApiUser> userManager,
                                 //SignInManager<ApiUser> signInManager,
                                 ILogger<AccountController> logger,
                                 IMapper mapper,
                                 IAuthManager authManager)
        {
            _userManager = userManager;
            //_signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
        }

        //We can send sensitive informacion across the pipe or in plane text
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("register")]//Need a route because both methods are Post and have the same parameter
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration Attemp for {userDTO.Email}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = _mapper.Map<ApiUser>(userDTO);//We get the user field from the post and mapp it
                user.UserName = userDTO.Email;
                var result = await _userManager.CreateAsync(user, userDTO.Password); //Now we have the fields and create a user with our manager

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(error.Code, error.Description);

                    return BadRequest(ModelState);
                }

                await _userManager.AddToRolesAsync(user, userDTO.Roles);//Add the roles when register this user
                return Accepted();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Register)}");
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)//Frombody is to ingore all that comes from URL
        {
            _logger.LogInformation($"Login Attemp for {userDTO.Email}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!await _authManager.ValidateUser(userDTO))
                    return Unauthorized();

                return Accepted(new { Token = await _authManager.CreateToken() });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Login)}");
                return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

    }
}
