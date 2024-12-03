using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace NamuDarbas.Models
{
    public class EncryptionContext : DbContext
    {
        public DbSet<KeyInfo> Keys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "encryptionkeys.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
                Console.WriteLine($"[Info] Configuring database at: {dbPath}");
            }
        }
    }
}