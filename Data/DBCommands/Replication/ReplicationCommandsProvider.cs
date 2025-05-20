namespace Data.DBCommands.Replication;

public class ReplicationCommandsProvider
{
    public static string AddTableToPublication(string tableName) => $@"
        EXEC sp_addarticle 
            @publication = N'MainPublication',
            @article = N'{tableName}',
            @source_table = N'{tableName}';";

    public static string GetReplicationStatus => @"
        SELECT name, status FROM sys.databases 
        WHERE is_published = 1 OR is_subscribed = 1;";
}