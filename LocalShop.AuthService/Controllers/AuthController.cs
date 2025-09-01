using LocalShop.AuthService.Services;
using LocalShop.Shared.Infrastructure.Middleware;
using LocalShop.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalShop.AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Invalid request data", 
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var response = await _authService.AuthenticateAsync(request);
                if (response == null)
                {
                    return Unauthorized(ApiResponse<AuthResponse>.ErrorResult("Invalid username or password"));
                }

                return Ok(ApiResponse<AuthResponse>.SuccessResult(response, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorResult("Internal server error"));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResult("Invalid request data",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var success = await _authService.RegisterAsync(request);
                if (!success)
                {
                    return BadRequest(ApiResponse.ErrorResult("Registration failed. User might already exist."));
                }

                return Ok(ApiResponse.SuccessResult("User registered successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return StatusCode(500, ApiResponse.ErrorResult("Internal server error"));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<ActionResult<ApiResponse>> RegisterAdmin([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.ErrorResult("Invalid request data",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var success = await _authService.RegisterAsync(request, "Admin");
                if (!success)
                {
                    return BadRequest(ApiResponse.ErrorResult("Admin registration failed. User might already exist."));
                }

                return Ok(ApiResponse.SuccessResult("Admin user registered successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during admin registration");
                return StatusCode(500, ApiResponse.ErrorResult("Internal server error"));
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(ApiResponse<UserDto>.ErrorResult("User not authenticated"));
                }

                var user = await _authService.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResult("User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResult(user, "Profile retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting profile");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResult("Internal server error"));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users/{userId}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int userId)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResult("User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResult(user, "User retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user: {UserId}", userId);
                return StatusCode(500, ApiResponse<UserDto>.ErrorResult("Internal server error"));
            }
        }
    }
}
