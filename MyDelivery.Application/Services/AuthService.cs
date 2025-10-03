using FluentResults;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyDelivery.Application.DTOs.User;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Interfaces.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace TrainningApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IApplicationUserRepository _userRepo;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthService> _logger;




        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AuthService> logger, IApplicationUserRepository userRepo)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _userRepo = userRepo;
   

        }


        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            var claims = new[]
            {
        // Claim principal em múltiplos formatos
        new Claim(ClaimTypes.NameIdentifier, user.Id),  // Padrão ASP.NET Core
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),  // Padrão JWT
        new Claim("uid", user.Id),  // Fallback customizado
        
        // Claims adicionais
        new Claim(ClaimTypes.Email, user.Email!),
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }



        

        public static Result ValidatePassword(CreateUserRequest request)
        {
            // Verifica se a senha e confirmação são iguais
            if (request.Password != request.ConfirmPassword)
            {
                return Result.Fail("As senhas não coincidem.");
            }

            // Verifica o comprimento mínimo
            if (request.Password.Length < 8)
            {
                return Result.Fail("A senha deve ter pelo menos 8 caracteres.");
            }

            // Verifica se contém pelo menos uma letra maiúscula
            if (!Regex.IsMatch(request.Password, "[A-Z]"))
            {
                return Result.Fail("A senha deve conter pelo menos uma letra maiúscula.");
            }

            // Verifica se contém pelo menos uma letra minúscula
            if (!Regex.IsMatch(request.Password, "[a-z]"))
            {
                return Result.Fail("A senha deve conter pelo menos uma letra minúscula.");
            }

            // Verifica se contém pelo menos um número
            if (!Regex.IsMatch(request.Password, "[0-9]"))
            {
                return Result.Fail("A senha deve conter pelo menos um número.");
            }

            // Verifica se contém pelo menos um caractere especial
            if (!Regex.IsMatch(request.Password, "[!@#$%^&*(),.?\":{}|<>]"))
            {
                return Result.Fail("A senha deve conter pelo menos um caractere especial.");
            }

            // Verifica senhas comuns (lista básica - você pode expandir)
            var commonPasswords = new List<string> { "12345678", "password", "senha123", "qwerty", "abcdefgh" };
            if (commonPasswords.Contains(request.Password.ToLower()))
            {
                return Result.Fail("A senha é muito comum. Por favor, escolha uma senha mais complexa.");
            }

            return Result.Ok();
        }

        public async Task<Result<UserResponse>> RegisterAsync(CreateUserRequest request, string creatorId)
        {
            try
            {
                _logger.LogInformation("Iniciando registro de usuário {Email} criado por {CreatorId}", request.Email, creatorId);
                var passwordValidation = ValidatePassword(request);
                if (!passwordValidation.IsSuccess)
                {
                    return passwordValidation; // Retorna o erro de validação
                }
     

                // 4. Verificação de e-mail único
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("E-mail {Email} já cadastrado", request.Email);
                    return Result.Fail("E-mail já está em uso");
                }

                // 5. Criação do usuário
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = request.Email,
                    Email = request.Email,
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow
                };
                IdentityResult createResult = new IdentityResult();
                if (request.Password == null || string.IsNullOrWhiteSpace(request.Password))
                {
                    createResult = await _userManager.CreateAsync(user);
                }
                else
                {
                    createResult = await _userManager.CreateAsync(user, request.Password);
                }

                if (!createResult.Succeeded)
                {

                    _logger.LogError("Falha ao criar usuário");
                    return Result.Fail("Falha ao criar usuário");
                }

                try
                {
                    // await _walletService.CreateForUserAsync(user.Id);
                }
                catch (Exception ex)
                {
                    // Rollback parcial
                    await _userManager.DeleteAsync(user);
                    _logger.LogError(ex, "Falha ao criar entidades relacionadas");
                    return Result.Fail("Falha ao configurar usuário");
                }

                _logger.LogInformation("Usuário {UserId} criado com sucesso", user.Id);

                return Result.Ok(new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o registro do usuário");
                return Result.Fail("Erro interno ao registrar usuário");
            }
        }




        public async Task<string> GetTokenAsync(string idUser)
        {
            try
            {
                _logger.LogInformation("Buscando usuário pelo ID: {UserId}", idUser);

                var user = await _userRepo.GetByIdAsync(idUser);

                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado pelo ID: {UserId}", idUser);
                    throw new KeyNotFoundException("Usuário não encontrado");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Result> CreateOrResetPassword(ResetPasswordUser resetPasswordUser)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordUser.Email);
                if (user == null)
                    return Result.Fail("Usuário não encontrado.");

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordUser.Token, resetPasswordUser.Password);
                if (!result.Succeeded) return Result.Fail("Erro ao criar ou alterar senha");
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail("Erro ao criar ou alterar senha");
            }

        }


    }
}