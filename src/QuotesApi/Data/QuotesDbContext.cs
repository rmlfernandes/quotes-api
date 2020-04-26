namespace QuotesApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using QuotesApi.Models;

    public class QuotesDbContext : DbContext
    {
        public QuotesDbContext(DbContextOptions<QuotesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Quote> Quotes { get; set; }
    }
}
