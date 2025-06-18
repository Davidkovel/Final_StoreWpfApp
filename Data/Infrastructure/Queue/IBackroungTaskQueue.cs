using Data.DTOs;

namespace Data.Infrastructure.Queue;

public interface IBackgroundTaskQueue
{
    public ValueTask QueueRatingAsync(RatingTask task);
    public ValueTask<RatingTask> DequeueAsync(CancellationToken ct);
}