using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Interfaces.Service;
using MyDelivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDelivery.Infrastructure.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly MyDbContext _context;
        private readonly ILogger<ApplicationUserRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public ApplicationUserRepository(
            MyDbContext context,
            ILogger<ApplicationUserRepository> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Buscando usuário pelo ID: {UserId}", id);
                return await _context.Users.Where(u => u.DisabledAt == null).FirstOrDefaultAsync(x => x.Id == id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar usuário pelo ID: {UserId}", id);
                throw new Exception($"Não foi possível encontrar o usuário com ID {id}", ex);
            }
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Buscando usuário pelo email: {UserEmail}", email);
                return await _context.Users
                    .Where(u => u.DisabledAt == null)
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar usuário pelo email: {UserEmail}", email);
                throw new Exception($"Não foi possível encontrar o usuário com email {email}", ex);
            }
        }

        public async Task<Result> AddAsync(ApplicationUser user)
        {
            try
            {
                _logger.LogInformation("Adicionando novo usuário com email: {UserEmail}", user.Email);

                user.CreatedAt = DateTime.UtcNow;
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário {UserEmail} adicionado com sucesso", user.Email);
                return Result.Ok().WithSuccess(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao adicionar usuário com email: {UserEmail}", user.Email);
                return Result.Fail($"Não foi possível adicionar o usuário: {ex.Message}");
            }
        }

        public async Task<Result> UpdateAsync(ApplicationUser user)
        {
            try
            {
                _logger.LogInformation("Atualizando usuário ID: {UserId}", user.Id);

                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("Usuário {UserId} não encontrado para atualização", user.Id);
                    return Result.Fail("Usuário não encontrado");
                }

                // Update each property individually
                existingUser.Name = user.Name;


                // Update timestamp
                existingUser.UpdatedAt = DateTime.UtcNow;

                // Mark as modified (optional - EF Core usually detects changes automatically)
                //_context.Entry(existingUser).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário {UserId} atualizado com sucesso", user.Id);
                return Result.Ok().WithSuccess("Usuário atualizado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao atualizar usuário ID: {UserId}", user.Id);
                return Result.Fail($"Não foi possível atualizar o usuário");
            }
        }

        public async Task<Result> DeleteByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Removendo usuário ID: {UserId}", id);

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuário {UserId} não encontrado para remoção", id);
                    return Result.Fail("Usuário não encontrado");
                }
                user.DisabledAt = DateTime.UtcNow;
                user.CreatedAt = DateTime.UtcNow;
                //_context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário {UserId} removido com sucesso", id);
                return Result.Ok().WithSuccess("Usuário removido com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao remover usuário ID: {UserId}", id);
                return Result.Fail($"Não foi possível remover o usuário: {ex.Message}");
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                _logger.LogInformation("Salvando alterações no banco de dados");
                await _context.SaveChangesAsync();
                _logger.LogInformation("Alterações salvas com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao salvar alterações no banco de dados");
                throw new Exception("Não foi possível salvar as alterações", ex);
            }
        }



        

        
    }
}
