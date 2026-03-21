using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using AutoTelemetryAPI.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoTelemetry.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TelemetryEvent> TelemetryEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TelemetryEvent>().HasKey(e => e.Id);

            modelBuilder.Entity<TelemetryEvent>().HasIndex(x => x.ChasisId);
        }
    }
}
