using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public UserOtpRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(UserOtp entity)
        {
            await _dbContext.UserOtps.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(UserOtp entity)
        {
            _dbContext.UserOtps.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<UserOtp>?> GetAllAsync()
        {
            return await _dbContext.UserOtps.ToListAsync();
        }

        public async Task<UserOtp?> GetByIdAsync(Guid id)
        {
            return await _dbContext.UserOtps.FindAsync(id);
        }

        public async Task<UserOtp> GetUserLatestAsync(Guid userId)
        {
            var userOtps = await _dbContext.UserOtps
                .Where(uo => uo.UserId == userId)
                .OrderByDescending(uo => uo.ExpiredAt)
                .ToListAsync();

            return userOtps.FirstOrDefault();
        }

        public async Task UpdateAsync(UserOtp entity)
        {
            _dbContext.UserOtps.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
