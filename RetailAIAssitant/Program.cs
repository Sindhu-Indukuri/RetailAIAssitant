using Microsoft.EntityFrameworkCore;
using RetailAIAssitant.AI;
using RetailAIAssitant.Data;
using RetailAIAssitant.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration
            .GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<
    IAIClient,
    GeminiClient>();

builder.Services.AddHttpClient<
    IEmbeddingClient,
    GeminiEmbeddingClient>();

builder.Services.AddScoped<
    RecommendationService>();

builder.Services.AddScoped<
    EmbeddingBuilderService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    db.Database.Migrate();

    await DbSeeder.SeedAsync(db);

    var embeddingBuilder =
        scope.ServiceProvider
            .GetRequiredService<
                EmbeddingBuilderService>();

    await embeddingBuilder.BuildAsync();
}

app.MapControllers();

app.Run();