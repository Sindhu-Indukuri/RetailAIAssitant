using RetailAIAssitant.Models;

namespace RetailAIAssitant.Repositories
{
    public interface IRecommendationLogRepository
    {
        Task SaveLogsAsync(
            List<RecommendationLog> logs);
    }
}