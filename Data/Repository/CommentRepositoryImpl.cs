using Core.Entity;
using Core.Repository;
using Dapper;
using Data.DBCommands;
using Data.DBProvider;

namespace Data.Repository;

public class CommentRepositoryImpl : CommentRepository
{
    private readonly IDatabaseProvider _databaseProvider;
    private readonly CommentCommandProvider _commentCommandProvider;

    public override async Task<IEnumerable<Comment>> GetCommentsByProductIdAsync(int productId)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        return await connection.QueryAsync<Comment>(_commentCommandProvider.GetCommentsByProductId(productId));
    }

    public override async Task<IEnumerable<Comment>> GetCommentByUserIdAsync(int userId, int productId)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        return await connection.QueryAsync<Comment>(
            _commentCommandProvider.GetCommentsByUserId(userId: userId, productId: productId));
    }

    public override async Task AddCommentAsync(Comment comment)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        await connection.ExecuteAsync(_commentCommandProvider.AddComment(
            userId: comment.UserId,
            text: comment.Text,
            productId: comment.ProductId,
            rating: comment.Rating));
    }
}