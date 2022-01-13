using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.Context;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GatewayService.Repositories.Implementation
{
    public class CredentialRepository : ICredentialRepository
    {
        private readonly ApplicationDbContext _context;

        public CredentialRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Credential> AddAsync(Credential credentials, CancellationToken cancellationToken = default)
        {
            await _context.Credentials.AddAsync(credentials, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return credentials;
        }

        public Task<bool> ExistsAsync<T>(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Credential> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Credentials.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<List<Credential>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Credentials.ToListAsync(cancellationToken);
        }

        public Task<Credential> GetLatestAsync(CancellationToken cancellationToken = default)
        {
            return _context.Credentials.OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<bool> AnyActiveCredentials(CancellationToken cancellationToken = default)
        {
            return _context.Credentials.AnyAsync(x => x.ExpiresOn > DateTime.Today || !x.ExpiresOn.HasValue, cancellationToken);
        }

        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Credentials.CountAsync(cancellationToken);
        }

        public async Task<Credential> UpdateAsync(Credential credential, CancellationToken cancellationToken = default)
        {
            _context.Credentials.Update(credential);
            await _context.SaveChangesAsync(cancellationToken);
            return credential;
        }
    }
}
