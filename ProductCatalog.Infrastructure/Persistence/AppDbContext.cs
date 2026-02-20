using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Idempotency;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Infrastructure.Persistence;

    public sealed class AppDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProcessedRequest> ProcessedRequests => Set<ProcessedRequest>(); // idempotencia (si la usas)

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
