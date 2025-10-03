using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using MyDelivery.Application.DTOs.User;

namespace MyDelivery.Domain.Interfaces.Service
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthenticateAsync(LoginRequest request);
        Task<Result<UserResponse>> RegisterAsync(CreateUserRequest request, string creatorId);
        public Task<Result> CreateOrResetPassword(ResetPasswordUser resetPasswordUser);
        public Task<string> GetTokenAsync(string userId);
    }
}
