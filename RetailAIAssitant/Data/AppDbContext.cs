using Microsoft.EntityFrameworkCore;
using RetailAIAssitant.Models;

namespace RetailAIAssitant.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionItem> TransactionItems { get; set; }

        public DbSet<EmbeddingMetadata> Embeddings { get; set; }

        public DbSet<RecommendationLog> RecommendationLogs { get; set; }
    }
}