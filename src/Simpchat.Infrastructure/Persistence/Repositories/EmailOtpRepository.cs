using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class EmailOtpRepository : IEmailOtpRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public EmailOtpRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(EmailOtp entity)
        {
            await _dbContext.EmailOtps.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(EmailOtp entity)
        {
            _dbContext.EmailOtps.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<EmailOtp>?> GetAllAsync()
        {
            return await _dbContext.EmailOtps.ToListAsync();
        }

        public async Task<EmailOtp> GetLatestByEmailAsync(string email)
        {
            return await _dbContext.EmailOtps
                .Where(eo => eo.Email == email)
                .OrderByDescending(eo => eo.ExpiredAt)
                .FirstOrDefaultAsync();
        }

        public async Task<EmailOtp?> GetByIdAsync(Guid id)
        {
            return await _dbContext.EmailOtps
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(EmailOtp entity)
        {
            _dbContext.EmailOtps.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
