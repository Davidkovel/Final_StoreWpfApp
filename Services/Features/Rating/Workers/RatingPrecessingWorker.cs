using Core.Repository;
using Data.Abstractions.NoSqlDatabase;
using Data.DTOs;
using Data.Infrastructure.Queue;
using Microsoft.Extensions.Logging;


namespace Services.Features.Rating.Workers;

public class RatingWorker(IRatingRepository repository, ICacheProvider redisProvider)
{
    private readonly IRatingRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ICacheProvider _redisProvider = redisProvider ?? throw new ArgumentNullException(nameof(redisProvider));

    public async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var task = await _redisProvider.DequeueAsync<RatingTask>("rating-queue");
            
            if (task == null)
            {
                // Console.WriteLine("Dequeued task is null.");
                await Task.Delay(5000, ct);
                continue;
            }
            
            try
            {
                Console.WriteLine($"{task.Rating}, {task.ProductId}, {task.UserId}");
                await _repository.AddRatingAsync(task.Rating, task.ProductId, task.UserId);
                Console.WriteLine($"Processed rating for product {task.ProductId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception message: " + ex.Message);
                // Console.WriteLine("StackTrace: " + ex.StackTrace);
            }

            await Task.Delay(5000, ct);
        }
    }
}