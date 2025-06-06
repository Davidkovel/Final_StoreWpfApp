using Data.Abstractions.Database;

namespace Data.DBCommands;

public class CommentCommandProvider : ICommentSqlCommandProvider
{
    private ICommentSqlCommandProvider _commentSqlCommandProviderImplementation;

    public string GetCommentsByProductId(int productId) => @"
        SELECT C.Id, C.UserId, C.ProductId, C.Text, C.Rating 
        FROM Comments C 
        WHERE C.ProductId = @ProductId";

    public string GetCommentsByUserId(int userId, int productId) => @"
        SELECT C.Id, C.UserId, C.ProductId, C.Text, C.Rating
        FROM Comments C
        WHERE C.UserId = @UserId AND C.ProductId = @ProductId
    ";

    public string AddComment(int userId, string text, int productId, int? rating) => @"
        INSERT INTO Comments (UserId, Text, ProductId, Rating)
        VALUES (@UserId, @Text, @ProductId, @Rating)
    ";
}