using System.Transactions;
using Core.Repository;
using Dapper;
using Data.Abstractions.Database;
using Core.Entity;
using Data.Database.Abstractions;
using Data.Database.Commands.Rating;

namespace Data.Repository.Rating;

public class RatingRepositoryImpl : IRatingRepository
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IRatingSqlCommandProvider _commandProvider;

    public RatingRepositoryImpl(IConnectionFactory connectionFactory,
        IRatingSqlCommandProvider commandProvider)
    {
        _connectionFactory = connectionFactory;
        _commandProvider = commandProvider;
    }

    public async Task<IEnumerable<Core.Entity.Rating>> GetRatingsByProductIdAsync(int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Core.Entity.Rating>(
            _commandProvider.GetRatingsByProductIdAsync(productId),
            new { ProductId = productId });
    }

    public async Task AddRatingAsync(Core.Entity.Rating rating)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.DefaultTimeout
            },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            // 1. Проверяем, не оценивал ли уже пользователь
            var hasRated = await connection.ExecuteScalarAsync<bool>(
                _commandProvider.CheckIfUserHasRatedAsync(rating.UserId, rating.ProductId),
                new { rating.UserId, rating.ProductId });

            if (hasRated)
                throw new InvalidOperationException("User has already rated this product");

            // 2. Добавляем новый рейтинг
            await connection.ExecuteAsync(
                _commandProvider.AddRatingAsync(rating),
                new
                {
                    rating.ProductId,
                    rating.UserId,
                    rating.RatingValue,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

            scope.Complete();
        }
        catch
        {
            // Транзакция автоматически откатится при выходе из scope без Complete()
            throw;
        }
    }

    public async Task<IEnumerable<Core.Entity.Rating>> GetRatingsByUserIdAsync(string userId, int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Core.Entity.Rating>(
            _commandProvider.GetRatingsByUserIdAsync(userId, productId),
            new { UserId = userId, ProductId = productId });
    }

    public async Task<bool> CheckIfUserHasRatedAsync(string userId, int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(
            _commandProvider.CheckIfUserHasRatedAsync(userId, productId),
            new { UserId = userId, ProductId = productId });
    }
}