
using Microsoft.EntityFrameworkCore;
using ModelContext.Models;
using System;

namespace ModelContext
{
    public class WebContext : GrContext
    {
        public string _connectionString { get; private set; }

        public WebContext(DbContextOptions<WebContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DbContextOptionsBuilder newOptionsBuilder;

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                newOptionsBuilder = optionsBuilder;
            }
            else
            {
                newOptionsBuilder = optionsBuilder.UseNpgsql(_connectionString);
            }

            base.OnConfiguring(newOptionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            Type[] types =
            {
                typeof(Store),
                typeof(Sector),
                typeof(Product),
            };

            foreach (Type t in types)
            {
                ModelBuilderGenerate(modelBuilder, t);
            }

        }
    }
}
