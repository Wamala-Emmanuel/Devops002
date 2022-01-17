using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GatewayService.Context
{
#nullable disable
    public class ApplicationDbContext : DbContext
    {
        private readonly IEncryptionProvider _provider;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
            IOptions<EncryptionOptions> encryptionOptions) : base(options)
        {
            _provider = new AesProvider(encryptionOptions.Value.KeyBytes, encryptionOptions.Value.InitializationVector);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseEncryption(_provider);
        }

        public DbSet<Request> Requests { get; set; }    
        
        public DbSet<Credential> Credentials { get; set; }
        
        public DbSet<RequestsExport> RequestsExports { get; set; }    
    
        public DbSet<NitaCredential> NitaCredentials { get; set; }
    }
}
