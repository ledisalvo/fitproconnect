namespace UserService.Infrastructure.Common;

public static class RetryHelper
{
    public static async Task TryAsync(Func<Task> action, int maxRetries = 3, int initialDelayMs = 1000)
    {
        int retry = 0;
        while (true)
        {
            try
            {
                await action();
                break;
            }
            catch (Exception ex)
            {
                retry++;
                Console.WriteLine($"[Retry] Error: {ex.Message}");

                if (retry >= maxRetries)
                {
                    Console.WriteLine("[Retry] Max retries reached. Giving up.");
                    break;
                }

                int delay = initialDelayMs * (int)Math.Pow(2, retry - 1);
                Console.WriteLine($"[Retry] Retrying in {delay} ms...");
                await Task.Delay(delay);
            }
        }
    }
}
