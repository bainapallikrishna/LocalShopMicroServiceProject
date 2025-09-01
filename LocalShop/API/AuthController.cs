using LocalShop.Models.DTOs;
using LocalShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalShop.API
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _authService.AuthenticateAsync(request);
                if (response == null)
                    return Unauthorized(new { message = "Invalid username or password" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
       
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _authService.RegisterAsync(request);
                if (!success)
                    return BadRequest(new { message = "Registration failed. User might already exist." });

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _authService.RegisterAsync(request, "Admin");
                if (!success)
                    return BadRequest(new { message = "Admin registration failed. User might already exist." });

                return Ok(new { message = "Admin user registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during admin registration");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
} 