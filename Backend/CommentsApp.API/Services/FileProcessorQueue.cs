using CommentsApp.API.Models;

using System.Threading.Channels;

namespace CommentsApp.API.Services;

public class FileProcessorQueue(ILogger<FileProcessorQueue> logger) : BackgroundService
{
    private readonly Channel<Comment> _channel = Channel.CreateUnbounded<Comment>();
    private readonly ILogger<FileProcessorQueue> _logger = logger;

    public void Enqueue(Comment comment)
    {
        if (!_channel.Writer.TryWrite(comment))
        {
            _logger.LogWarning("Failed to enqueue comment with ID {Id}", comment.Id);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var comment in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                Process(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing comment {Id}", comment.Id);
            }
        }
    }

    private void Process(Comment comment)
    {
        _logger.LogInformation("Processing comment {Id} from {Email}", comment.Id, comment.User?.Email);

        foreach (var file in comment.Files)
        {
            _logger.LogInformation("File: {Name}, Size: {Size}, Type: {Type}",
                file.FileName, file.Content?.Length ?? 0, file.ContentType);
        }
    }
}
