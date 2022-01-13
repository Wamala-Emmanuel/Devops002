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
    public class NitaCredentialRepository : INitaCredentialRepository
    {
        private readonly ApplicationDbContext _context;

        public NitaCredentialRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NitaCredential> AddAsync(NitaCredential credential, CancellationToken cancellationToken = default)
        {
            await _context.NitaCredentials.AddAsync(credential, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return credential;
        }

        public Task<bool> ExistsAsync<T>(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<NitaCredential> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.NitaCredentials.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

        }

        public async Task<List<NitaCredential>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.NitaCredentials.ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.NitaCredentials.CountAsync(cancellationToken);
        }

        public Task<NitaCredential> GetLatestAsync(CancellationToken cancellationToken = default)
        {
            return _context.NitaCredentials.OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<NitaCredential> UpdateAsync(NitaCredential credential, CancellationToken cancellationToken = default)
        {
            _context.NitaCredentials.Update(credential);
            await _context.SaveChangesAsync(cancellationToken);
            return credential;
        }
    }
}
