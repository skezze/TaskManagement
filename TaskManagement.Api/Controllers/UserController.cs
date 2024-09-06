using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ILogger = Serilog.ILogger;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IConfiguration configuration;
        private readonly ILogger _logger;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            this.userService = userService;
            this.configuration = configuration;
            _logger = Log.ForContext<UserController>(); // Set up Serilog context for this controller
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registerViewModel">User registration data</param>
        /// <returns>200 OK if success, 400 Bad Request if failure</returns>
        [HttpPost]
        [Route("users/register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            _logger.Information("Register endpoint hit with data for user: {UserName}", registerViewModel?.UserName);

            string errorMessage = string.Empty;
            PasswordValidator passwordValidator = new PasswordValidator();

            // Validate password and ensure the registration data is provided
            if (registerViewModel != null &&
                passwordValidator.ValidatePassword(registerViewModel.Password, out errorMessage))
            {
                var registerDTO = new RegisterDTO
                {
                    UserName = registerViewModel.UserName,
                    Password = registerViewModel.Password,
                    Email = registerViewModel.Email
                };

                // Try to register the user
                var user = await userService.RegisterUserAsync(registerDTO);
                if (user != null)
                {
                    _logger.Information("User {UserName} registered successfully", user.Username);
                    return Ok(user);
                }

                _logger.Warning("User {UserName} already exists", registerViewModel.UserName);
                return BadRequest(new ErrorResponse { ErrorMessage = "User already exists" });
            }

            _logger.Warning("User registration failed due to invalid data: {ErrorMessage}", errorMessage);
            return BadRequest(new ErrorResponse { ErrorMessage = errorMessage });
        }

        /// <summary>
        /// Check if the user is authorized (requires valid JWT)
        /// </summary>
        /// <param name="loginViewModel">User login data</param>
        /// <returns>200 OK if authorized, 400 Bad Request if invalid</returns>
        [HttpPost]
        [Route("users/is-user-authorized")]
        [Authorize] // This endpoint requires authorization
        public async Task<IActionResult> IsUserAuthorized([FromBody] LoginViewModel loginViewModel)
        {
            _logger.Information("IsUserAuthorized endpoint hit");

            // Check for null login data
            if (loginViewModel == null)
            {
                _logger.Warning("Authorization failed due to null login data");
                return BadRequest(new ErrorResponse { ErrorMessage = "Invalid client credentials" });
            }

            // Prepare login DTO
            var loginDTO = new LoginDTO
            {
                UserNameOrEmail = loginViewModel.UserNameOrEmail,
                Password = loginViewModel.Password
            };

            // Try to authorize the user
            var user = await userService.LoginUserAsync(loginDTO);
            if (user != null)
            {
                _logger.Information("User {UserName} is authorized", user.Username);
                return Ok(user);
            }

            _logger.Warning("Authorization failed for user {UserNameOrEmail}", loginViewModel.UserNameOrEmail);
            return BadRequest(new ErrorResponse { ErrorMessage = "Invalid client credentials" });
        }

        /// <summary>
        /// Login the user and generate a JWT token
        /// </summary>
        /// <param name="loginViewModel">User login data</param>
        /// <returns>JWT token if successful, Unauthorized if login fails</returns>
        [HttpPost]
        [Route("users/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            _logger.Information("Login endpoint hit for user: {UserNameOrEmail}", loginViewModel?.UserNameOrEmail);

            // Check for null login data
            if (loginViewModel == null)
            {
                _logger.Warning("Login failed due to null data");
                return BadRequest(new ErrorResponse { ErrorMessage = "Invalid client request" });
            }

            // Prepare login DTO
            var loginDTO = new LoginDTO
            {
                UserNameOrEmail = loginViewModel.UserNameOrEmail,
                Password = loginViewModel.Password
            };

            // Try to login the user
            var user = await userService.LoginUserAsync(loginDTO);
            if (user != null)
            {
                _logger.Information("User {UserName} logged in successfully", user.Username);

                // Generate JWT token
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

                // Create token string
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                _logger.Information("JWT generated successfully for user {UserName}", user.Username);
                return Ok(new AuthenticatedResponse { Token = tokenString });
            }

            _logger.Warning("Login failed for user {UserNameOrEmail}", loginViewModel.UserNameOrEmail);
            return Unauthorized(new ErrorResponse { ErrorMessage = "Invalid client credentials" });
        }
    }
}
