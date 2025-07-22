namespace CommentsApp.API.Services;

public class FileProcessorQueue : BackgroundService
{
    private readonly ILogger<FileProcessorQueue> _logger;

    public FileProcessorQueue(ILogger<FileProcessorQueue> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Queue worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(5000, stoppingToken);
        }
    }
}
