﻿
using Microsoft.EntityFrameworkCore;
using PoliceMaps.Entities;
using System.Reflection.Emit;

namespace PoliceMaps.Contexts
{
    public class MapsDbContext : DbContext
    {
        public DbSet<Hotspot> Hotspots { get; set; }
        public DbSet<SurveyArea> SurveyAreas { get; set; }

        public MapsDbContext()
        {
        }

        public MapsDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Hotspot>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=sql,1434;Initial Catalog=PM-Data;Persist Security Info=False;User ID=sa;Password=Pass123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            }
        }
    }
}
