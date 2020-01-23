using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InterMescyt.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Header> Headers { get; set; }
        public DbSet<TransLine> TransLines { get; set; }
        public DbSet<Execution> Executions { get; set; }
        public DbSet<ExecutionLine> ExecutionLines { get; set; }
        public DbSet<HeaderBank> HeaderBanks { get; set; }
        public DbSet<TransLineBank> TransLineBanks { get; set; }
    }
}
