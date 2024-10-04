using GlobalChat.DTOs;
using GlobalChat.Middlewares;
using GlobalChat.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlobalChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var user = await _authService.Register(dto.Username, dto.Password);

                PreventDuplicateAccountsMiddleware.RegisterUser(HttpContext.Connection.RemoteIpAddress.ToString());

                return Ok(new { username = user.Username });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _authService.Login(dto.Username, dto.Password);
                return Ok(new { username = user.Username });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
