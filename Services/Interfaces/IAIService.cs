namespace NearzoAPI.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> GetChatResponseAsync(string message);
    }
}
