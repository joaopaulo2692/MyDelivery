
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyDelivery.Application.DTOs;
using MyDelivery.Application.DTOs.User;
using MyDelivery.Domain.Interfaces.Service;
using System.Security.Claims;


namespace CarteiraDigital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        /// <summary>
        /// Realiza o login do usuário e retorna um token JWT.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _authService.AuthenticateAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", details = ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado");
            }
            return idClaim.Value;
        }

        

        /// <summary>
        /// Realiza o registro de usuário.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            try
            {

                //var userId = GetCurrentUserId();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier);
                var result = await _authService.RegisterAsync(request, userId != null ? userId.Value : "");

                if (result.IsSuccess)
                {
                    return CreatedAtAction(nameof(Register), new { id = result.Value.Id }, new ApiResponse<UserResponse>
                    {
                        Success = true,
                        Data = result.Value,
                        Message = "Usuário registrado com sucesso"
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Data = null,
                        Message = string.Join("; ", result.Errors.Select(e => e.Message))
                    });
                }
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Message = $"Erro interno no servidor: {ex.Message}"
                });
            }
        }


        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordUser request)
        {
            try
            {
                //var userId = GetCurrentUserId();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier);
                var result = await _authService.CreateOrResetPassword(request);

                if (result.IsSuccess)
                {
                    return CreatedAtAction(nameof(Register), new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Usuário registrado com sucesso"
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = string.Join("; ", result.Errors.Select(e => e.Message))
                    });
                }
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Message = $"Erro interno no servidor: {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Testes de Autenticação
        /// </summary>
        [AllowAnonymous]
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"];
            return Ok(new { authHeader });
        }


  
    }
}
