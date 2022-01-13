using System.Text;
using GatewayService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;

namespace GatewayService.Context
{
#nullable disable
    public class ApplicationDbContext : DbContext
    {
        private readonly IEncryptionProvider _provider;
        public string Key { get; set; } = "6fcb6050650a435780f3420b158b7001";
        public byte[] _initializationVector => new byte[] { 26, 19, 18, 90, 117, 17, 87, 43, 24, 103, 11, 44, 18, 113, 93, 14 };
        public byte[] _keyBytes => Encoding.Default.GetBytes(Key);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _provider = new AesProvider(_keyBytes, _initializationVector);
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
