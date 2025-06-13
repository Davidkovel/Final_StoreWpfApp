using Data.Abstractions.Database;

namespace Data.DBCommands;

public class CommentCommandProvider : ICommentSqlCommandProvider
{
    public string GetCommentsByProductId() => @"
        SELECT C.Id, C.UserId, C.ProductId, C.Text, C.Rating 
        FROM Comments C 
        WHERE C.ProductId = @ProductId";

    public string GetCommentsByUserId() => @"
        SELECT C.Id, C.UserId, C.ProductId, C.Text, C.Rating
        FROM Comments C
        WHERE C.UserId = @UserId AND C.ProductId = @ProductId
    ";

    public string AddComment() => @"
        INSERT INTO Comments (UserId, Text, ProductId, Rating)
        VALUES (@UserId, @Text, @ProductId, @Rating)
    ";
}