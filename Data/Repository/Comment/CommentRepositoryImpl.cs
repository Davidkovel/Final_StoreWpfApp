using Core.Entity;
using Core.Repository;
using Dapper;
using Data.Abstractions.Database;
using Data.Database.Abstractions;
using Data.DBCommands;
using Data.DBProvider;

namespace Data.Repository;

public class CommentRepositoryImpl : CommentRepository
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ICommentSqlCommandProvider _commentCommandProvider;

    public CommentRepositoryImpl(IConnectionFactory connectionFactory, ICommentSqlCommandProvider commandProvider)
    {
        _connectionFactory = connectionFactory;
        _commentCommandProvider = commandProvider;
    }

    public override async Task<IEnumerable<Comment>> GetCommentsByProductIdAsync(int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("ProductId", productId);
        return await connection.QueryAsync<Comment>(_commentCommandProvider.GetCommentsByProductId(), parameters);
    }

    public override async Task<IEnumerable<Comment>> GetCommentByUserIdAsync(string userId, int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);
        parameters.Add("ProductId", productId);
        return await connection.QueryAsync<Comment>(
            _commentCommandProvider.GetCommentsByUserId(), parameters);
    }

    public override async Task AddCommentAsync(Comment comment)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", comment.UserId);
        parameters.Add("Text", comment.Text);
        parameters.Add("ProductId", comment.ProductId);
        await connection.ExecuteAsync(_commentCommandProvider.AddComment(), parameters);
    }
}