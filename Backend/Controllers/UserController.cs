using AutoMapper;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.DataTransferObjects;
using Backend.Extensions;
using Backend.JwtFeatures;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserService _userService;
        private readonly JwtHandler _jwtHandler;
        private readonly IMapper _mapper;

        private ApplicationUser _currentUser;
        public UserController(
            IHttpContextAccessor contextAccessor,
            IMapper mapper,
            UserService accService,
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager, 
            SignInManager<ApplicationUser> signInManager, 
            JwtHandler jwtHandler)
        {
            _mapper = mapper;
            _userService = accService;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtHandler = jwtHandler;

            if(contextAccessor.IsUserAuthenticated())
                _currentUser = _userService.Get(contextAccessor.GetUserId());
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] UserRegistrationDto newUser)
        {
            if (newUser == null || !ModelState.IsValid)
                return BadRequest();

            ApplicationUser appUser = new ApplicationUser
            {
                UserName = newUser.Email,
                Email = newUser.Email
            };

            IdentityResult result = await _userManager.CreateAsync(appUser, newUser.Password);

            if (!result.Succeeded)
                return IdentityError(result);

            if(!await _roleManager.RoleExistsAsync("Admin"))
            {
                result = await _roleManager.CreateAsync(new ApplicationRole() { Name = "Admin" });
                if(!result.Succeeded)
                    return IdentityError(result);

            }

            result = await _userManager.AddToRoleAsync(appUser, "Admin");
            
            if (!result.Succeeded)
                return IdentityError(result);

            return StatusCode(201);
            
        }

        private BadRequestObjectResult IdentityError(IdentityResult result)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new StatusResponseDto { Errors = errors });
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Login falsch" });
            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = await _jwtHandler.GetClaimsAsync(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }

        [HttpGet("Claims")]
        [Authorize(Roles ="Admin")]
        public IActionResult Claims()
        {
            var claims = User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();
            return Ok(claims);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserSettings(UserSettingsDto updatedSettings)
        {
            
            if (_currentUser is null)
            {
                return BadRequest();
            }     

            var settingsToUpdate = _mapper.Map(updatedSettings, _currentUser.UserSettings);
            _currentUser.UserSettings = settingsToUpdate;

            await _userService.UpdateAsync(_currentUser);

            return NoContent();
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserSettings()
        {

            if (_currentUser.UserSettings == null)
            {
                _currentUser.UserSettings =
                    new UserSettings
                    {                        
                    };

               await _userService.UpdateAsync(_currentUser);
            }

            var dto = _mapper.Map<UserSettingsDto>(_currentUser.UserSettings);

            return Ok(dto);
        }


    }
}
