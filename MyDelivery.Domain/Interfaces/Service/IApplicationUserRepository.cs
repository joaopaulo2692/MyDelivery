using FluentResults;
using MyDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDelivery.Domain.Interfaces.Service
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<Result> DeleteByIdAsync(string id);
        Task<Result> UpdateAsync(ApplicationUser user);
        Task<Result> AddAsync(ApplicationUser user);
        Task SaveChangesAsync();
    }
}
