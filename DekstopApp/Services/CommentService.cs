using System.Windows;
using Core.Entity;
using Core.Repository;

namespace DekstopApp.Services;

public class CommentService(CommentRepository commentRepository)
{
    public async Task<IEnumerable<Comment>> GetCommentsByProductId(int productId)
    {
        var comments = await commentRepository.GetCommentsByProductIdAsync(productId);
        return comments;
    }

    public async Task<IEnumerable<Comment>> GetCommentByUserId(int userId, int productId)
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
            MessageBox.Show($"Failed to add comment {ex.Message}");
            return false;
        }
        
    }

    public async Task<bool> CheckIfUserHasComment(int userId, int productId)
    {
        try
        {
            var comments = await commentRepository.GetCommentByUserIdAsync(userId, productId);
            return comments?.Any() ?? false;
        }
        catch
        {
            MessageBox.Show("Failed to check user comments");
            return false;
        }
    }
}