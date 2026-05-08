using RetailAIAssitant.Data;
using RetailAIAssitant.Models;

namespace RetailAIAssitant.Repositories
{
    public class RecommendationLogRepository
        : IRecommendationLogRepository
    {
        private readonly AppDbContext _db;

        public RecommendationLogRepository(
            AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveLogsAsync(
            List<RecommendationLog> logs)
        {
            _db.RecommendationLogs.AddRange(logs);

            await _db.SaveChangesAsync();
        }
    }
}