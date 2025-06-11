using Microsoft.AspNetCore.Mvc;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services;

namespace AppAPIEmpacadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica un usuario y genera un token JWT
        /// </summary>
        /// <param name="loginDto">Credenciales del usuario</param>
        /// <returns>Token JWT y datos del usuario</returns>
        /// <response code="200">Retorna el token JWT y datos del usuario</response>
        /// <response code="401">Credenciales inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDto)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
} 