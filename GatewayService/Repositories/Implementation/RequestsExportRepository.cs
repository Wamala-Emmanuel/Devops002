using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.Context;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GatewayService.Repositories.Implementation
{
    public class RequestsExportRepository : IRequestsExportRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestsExportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Queries

        public async Task<bool> ExistsAsync<RequestsExports>(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.RequestsExports.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<RequestsExport> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.RequestsExports
                            .SingleOrDefaultAsync(r => r.Id == id && r.IsDeleted == false, cancellationToken);
        }

        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.RequestsExports.CountAsync(cancellationToken);
        }
        
        #endregion

        #region Commands
        
        public async Task<RequestsExport> AddAsync(RequestsExport request, CancellationToken cancellationToken = default)
        {
            await _context.RequestsExports.AddAsync(request);
            await _context.SaveChangesAsync(cancellationToken);
            return request;
        }


        public async Task<RequestsExport> UpdateAsync(RequestsExport request, CancellationToken cancellationToken = default)
        {
            _context.RequestsExports.Update(request);
            await _context.SaveChangesAsync(cancellationToken);
            return request;
        }

        public Task<List<RequestsExport>> GetNotDownloadedRequestsExportListAsync(int days, double offset, CancellationToken cancellationToken = default)
        {
            var backDate = DateTime.UtcNow.AddDays(-days).AddHours(offset);

            IQueryable<RequestsExport> query = _context.RequestsExports;

            query = query.Where(r => r.GenerationStatus == ExportStatus.Complete);

            query = query.Where(r => r.DownloadedOn == null);

            query = query.Where(r => r.CreatedOn <= backDate);

            return query.ToListAsync(cancellationToken);
        }

        #endregion
    }
}
