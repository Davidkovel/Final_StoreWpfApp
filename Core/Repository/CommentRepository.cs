using Core.Entity;

namespace Core.Repository;

public abstract class CommentRepository
{
    public abstract Task <IEnumerable<Comment>> GetCommentsByProductIdAsync(int productId);
    public abstract Task<IEnumerable<Comment>> GetCommentByUserIdAsync(int userId, int productId);
    public abstract Task AddCommentAsync(Comment comment);
}