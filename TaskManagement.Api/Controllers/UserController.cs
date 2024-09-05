using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Methods;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Models;
using TaskManagement.Domain.ViewModels;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    public class UserController:ControllerBase
    {
        private readonly IUserService userService;
        private readonly IConfiguration configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            this.userService = userService;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("users/register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel registerViewModel)
        {
            string errorMessage = string.Empty;
            PasswordValidator passwordValidator = new PasswordValidator();

            if (registerViewModel != null &&
                passwordValidator.ValidatePassword(registerViewModel.Password, out errorMessage))
            {
                var registerDTO = new RegisterDTO
                {
                    UserName = registerViewModel.UserName,
                    Password = registerViewModel.Password,
                    Email = registerViewModel.Email
                };
                var user = await userService.RegisterUserAsync(registerDTO);
                if (user != null)
                {
                    return Ok(user);
                }
                return BadRequest(new ErrorResponse() { ErrorMessage = "user is exist" });

            }
            return BadRequest(new ErrorResponse() { ErrorMessage = errorMessage});
        }

        [HttpPost]
        [Route("users/is-user-authorized")]
        [Authorize]
        public async Task<IActionResult> IsUserAuthorized([FromBody] LoginViewModel loginViewModel)
        {
            if (loginViewModel == null)
            {
                return BadRequest(new ErrorResponse() { ErrorMessage = "Invalid client credentials" });
            }

            var loginDTO = new LoginDTO 
            { 
                UserNameOrEmail = loginViewModel.UserNameOrEmail,
                Password = loginViewModel.Password
            };
            var user = await userService.LoginUserAsync(loginDTO);
            if (user != null)
            {
                return Ok(user);
            }
            return BadRequest(new ErrorResponse() { ErrorMessage = "Invalid client credentials" });
        }

        [HttpPost]
        [Route("users/login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel loginViewModel)
        {
            if (loginViewModel == null)
            {
                return BadRequest(new ErrorResponse() { ErrorMessage = "Invalid client request" });
            }

            var loginDTO = new LoginDTO
            {
                UserNameOrEmail = loginViewModel.UserNameOrEmail,
                Password = loginViewModel.Password
            };
            var user = await userService.LoginUserAsync(loginDTO);
            if (user != null)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration
                    .GetSection("AppSettings")["Secret"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: configuration.GetSection("AppSettings")["LocalhostUrl"],
                    audience: configuration.GetSection("AppSettings")["LocalhostUrl"],
                    claims: new[]
                    {
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    },
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return Ok(new AuthenticatedResponse { Token = tokenString });
            }

            return Unauthorized(new ErrorResponse() { ErrorMessage = "Invalid client credentials" });
        } 


    }
}
