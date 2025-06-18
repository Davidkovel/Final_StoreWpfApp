using System.Threading.Channels;
using Data.DTOs;
    
namespace Data.Infrastructure.Queue;
//
// public class BackgroundTaskQueue : IBackgroundTaskQueue
// {
//     private readonly Channel<RatingTask> _queue;
//
//     public BackgroundTaskQueue(int capacity = 1000)
//     {
//         var options = new BoundedChannelOptions(capacity)
//         {
//             FullMode = BoundedChannelFullMode.Wait
//         };
//         _queue = Channel.CreateBounded<RatingTask>(options);
//     }
//
//     public async ValueTask QueueRatingAsync(RatingTask task)
//         => await _queue.Writer.WriteAsync(task);
//
//     public async ValueTask<RatingTask> DequeueAsync(CancellationToken ct)
//         => await _queue.Reader.ReadAsync(ct);
// }
