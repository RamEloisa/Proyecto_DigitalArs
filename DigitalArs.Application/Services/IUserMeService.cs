using System;
using System.Collections.Generic;
using System.Text;
using DigitalArs.Application.DTOs;

namespace DigitalArs.Application.Services
{
    public interface IUserMeService
    {
        Task<UserDto> GetMeAsync(int userId);
        Task UpdateMeAsync(int userId, UpdateMeDto updateMeDto);
    }
}
