using Microsoft.EntityFrameworkCore;
using RetailAIAssitant.AI;
using RetailAIAssitant.Data;
using RetailAIAssitant.Helpers;
using RetailAIAssitant.Models;

namespace RetailAIAssitant.Services
{
    public class EmbeddingBuilderService
    {
        private readonly AppDbContext _db;
        private readonly IEmbeddingClient _embedding;

        public EmbeddingBuilderService(
            AppDbContext db,
            IEmbeddingClient embedding)
        {
            _db = db;
            _embedding = embedding;
        }

        public async Task BuildAsync()
        {
            if (await _db.Embeddings.AnyAsync())
                return;

            var products =
                await _db.Products.ToListAsync();

            foreach (var product in products)
            {
                var vector =
                    await _embedding.GetEmbeddingAsync(
                        product.Description);

                _db.Embeddings.Add(
                    new EmbeddingMetadata
                    {
                        ProductName = product.Name,
                        Content = product.Description,

                        Vector =
                            VectorHelper.ToVectorString(
                                vector)
                    });
            }

            await _db.SaveChangesAsync();
        }
    }
}