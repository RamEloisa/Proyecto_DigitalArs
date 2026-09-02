using System;
using System.Collections.Generic;
using System.Text;
using DigitalArs.Application.DTOs;
using DigitalArs.Application.Security;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;
using Mapster;
using MapsterMapper;

namespace DigitalArs.Application.Services
{
    public class UserMeService : IUserMeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserMeService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> GetMeAsync(int userId) 
        {
            var user = await GetUserOrThrowAsync(userId);
            return user.Adapt<UserDto>();
        }

        public async Task UpdateMeAsync(int userId, UpdateMeDto request)
        {
            var user = await GetUserOrThrowAsync(userId);
            request.Adapt(user);

            var wantsPasswordChange =
                !string.IsNullOrWhiteSpace(request.CurrentPassword) &&
                !string.IsNullOrWhiteSpace(request.NewPassword);

            if (wantsPasswordChange) 
            {
                if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword)

                ) {
                    throw new InvalidOperationException("Para cambiar la contraseña hay que enviar la contraseña actual y la nueva.");
                }

                var currentPasswordIsValid = _passwordHasher.Verify(request.CurrentPassword, user.Password_Hasheada);

                if (!currentPasswordIsValid) {
                    throw new UnauthorizedAccessException("La contraseña actual no es correcta.");
                }

                user.Password_Hasheada = _passwordHasher.Hash(request.NewPassword);

            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<User> GetUserOrThrowAsync(int userId)
        {
            var users = await _unitOfWork.Repository<User>().FindAsync(u => u.ID_User == userId, default, u => u.Role);

            var user = users.FirstOrDefault();

            if (user is null)
            {
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado.");
            }
            return user;
        }


    }
}
