namespace Data.Abstractions.Database;

public interface ICommentSqlCommandProvider
{
    public string GetCommentsByProductId();
    public string GetCommentsByUserId();
    public string AddComment();
}