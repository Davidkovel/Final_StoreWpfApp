namespace Data.Abstractions.Database;

public interface ICommentSqlCommandProvider
{
    public string GetCommentsByProductId(int productId);
    public string GetCommentsByUserId(int userId, int productId);
    public string AddComment(int userId, string text, int productId, int? rating);
}