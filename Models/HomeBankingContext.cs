using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HomeBankingMinHub.Models
{
    public class HomeBankingContext : DbContext
    {
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options)
            : base(options)
        {
        }
        public DbSet<Client> Clients { get; set; }
    }
}
