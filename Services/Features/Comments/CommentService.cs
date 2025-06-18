using System.Windows;
using Core.Entity;
using Core.Repository;

namespace DekstopApp.Services;

public class CommentService(CommentRepository commentRepository, IRatingRepository ratingRepository)
{
    public async Task<IEnumerable<Comment>> GetCommentsByProductId(int productId)
    {
        var comments = await commentRepository.GetCommentsByProductIdAsync(productId);
        return comments;
    }

    public async Task<IEnumerable<Comment>> GetCommentByUserId(string userId, int productId)
    {
        var comments = await commentRepository.GetCommentByUserIdAsync(userId, productId: productId);
        return comments;
    }

    public async Task<bool> AddComment(Comment comment)
    {
        try
        {
            await commentRepository.AddCommentAsync(comment);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> CheckIfUserHasComment(string userId, int productId)
    {
        try
        {
            return await ratingRepository.CheckIfUserHasRatedAsync(userId, productId);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CheckIfUserHasRating(string userId, int productId)
    {
        try
        {
            var ratings = await ratingRepository.GetRatingsByUserIdAsync(userId, productId);
            return ratings?.Any() ?? false;
        }
        catch
        {
            return false;
        }
    }
}