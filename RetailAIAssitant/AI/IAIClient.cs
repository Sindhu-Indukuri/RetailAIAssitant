namespace RetailAIAssitant.AI
{
    public interface IAIClient
    {
        Task<string> GenerateAsync(string prompt);
    }
}
