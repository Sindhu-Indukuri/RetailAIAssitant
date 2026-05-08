using Microsoft.EntityFrameworkCore;
using RetailAIAssitant.AI;
using RetailAIAssitant.Data;
using RetailAIAssitant.Helpers;
using RetailAIAssitant.Models;

namespace RetailAIAssitant.Services
{
    public class RecommendationService
    {
        private readonly AppDbContext _db;
        private readonly IEmbeddingClient _embedding;
        private readonly IAIClient _ai;

        public RecommendationService(
            AppDbContext db,
            IEmbeddingClient embedding,
            IAIClient ai)
        {
            _db = db;
            _embedding = embedding;
            _ai = ai;
        }

        public async Task<List<RecommendationResult>>
     RecommendAsync(string productName)
        {
            var product = await _db.Products
                .FirstOrDefaultAsync(p =>
                    p.Name.ToLower() ==
                    productName.ToLower());

            if (product == null)
            {
                return new List<RecommendationResult>();
            }

            // QUERY EMBEDDING
            var queryEmbedding =
                await _embedding.GetEmbeddingAsync(
                    product.Description);

            // LOAD TRANSACTIONS
            var transactions = await _db.Transactions
                .Include(t => t.Items)
                .ToListAsync();

            var productFreq = new Dictionary<string, int>();
            var pairFreq = new Dictionary<string, int>();

            foreach (var transaction in transactions)
            {
                var items = transaction.Items
                    .Select(i => i.ProductName)
                    .Distinct()
                    .ToList();

                foreach (var item in items)
                {
                    productFreq[item] =
                        productFreq.GetValueOrDefault(item) + 1;
                }

                bool contains =
                    items.Any(x =>
                        x.Equals(productName,
                            StringComparison.OrdinalIgnoreCase));

                if (!contains)
                    continue;

                foreach (var item in items)
                {
                    if (item.Equals(productName,
                        StringComparison.OrdinalIgnoreCase))
                        continue;

                    pairFreq[item] =
                        pairFreq.GetValueOrDefault(item) + 1;
                }
            }

            int baseFreq =
                productFreq.GetValueOrDefault(productName, 1);

            var embeddings =
                await _db.Embeddings.ToListAsync();

            var results = new List<RecommendationResult>();

            // HYBRID RECOMMENDATIONS
            foreach (var embedding in embeddings)
            {
                if (embedding.ProductName.Equals(
                    productName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                float[] vector =
                    VectorHelper.ParseVector(embedding.Vector);

                double similarity =
                    VectorHelper.CosineSimilarity(
                        queryEmbedding,
                        vector);

                double confidence = 0;

                if (pairFreq.ContainsKey(embedding.ProductName))
                {
                    confidence =
                        (double)pairFreq[embedding.ProductName]
                        / baseFreq;
                }

                double finalScore =
                    (0.6 * confidence) +
                    (0.4 * similarity);

                results.Add(new RecommendationResult
                {
                    Product = embedding.ProductName,
                    Confidence = Math.Round(confidence, 2),
                    Similarity = Math.Round(similarity, 2),
                    FinalScore = Math.Round(finalScore, 2)
                });
            }

            var top = results
                .OrderByDescending(x => x.FinalScore)
                .Take(3)
                .ToList();

            // AI REASONING
            foreach (var item in top)
            {
                var prompt = $@"
Explain why product
{item.Product}
is recommended for
{productName}.
";

                item.Reason =
                    await _ai.GenerateAsync(prompt);
            }

            // ✅ SAVE TO RECOMMENDATION LOGS (ADDED PART)
            foreach (var item in top)
            {
                _db.RecommendationLogs.Add(new RecommendationLog
                {
                    RequestedProduct = productName,
                    RecommendedProduct = item.Product,
                    Confidence = item.Confidence,
                    Similarity = item.Similarity,
                    FinalScore = item.FinalScore,
                    Reason = item.Reason,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync();

            return top;
        }
    }
    }