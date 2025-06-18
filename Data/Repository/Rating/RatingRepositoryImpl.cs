using System.Reflection.Metadata;
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

    public async Task<IEnumerable<Core.Entity.RatingProduct>> GetRatingsByProductIdAsync(int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("ProductId", productId);
        return await connection.QueryAsync<Core.Entity.RatingProduct>(
            _commandProvider.GetRatingsByProductIdAsync(productId),
            parameters);
    }

    public async Task AddRatingAsync(int selectedRating, int productId, string userId)
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

            // 2. Добавляем новый рейтинг
            await connection.ExecuteAsync(
                _commandProvider.AddRatingAsync(selectedRating, productId, userId),
                new
                {
                    Rating = selectedRating,
                    ProductId = productId,
                    UserId = userId
                });

            scope.Complete();
        }
        catch
        {
            // Транзакция автоматически откатится при выходе из scope без Complete()
            throw;
        }
    }

    public async Task<bool> HasUserRatedAsync(string userId, int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        // 1. Проверяем, не оценивал ли уже пользователь
        var hasRated = await connection.ExecuteScalarAsync<bool>(
            _commandProvider.CheckIfUserHasRatedAsync(userId, productId),
            new { UserId = userId, ProductId = productId });

        return hasRated;
    }

    public async Task<List<int>> GetRatingsByUserIdAsync(string userId, int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return (List<int>)await connection.QueryAsync(
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