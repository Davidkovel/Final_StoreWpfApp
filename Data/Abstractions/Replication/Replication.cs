namespace Data.Abstractions.Replication;

public interface IReplicationConfigurator
{
    Task ConfigurePublisherAsync();
    Task ConfigureSubscriberAsync(string publisherConnectionString);
    Task AddArticleToPublicationAsync(string tableName);
}

public interface IReplicationMonitor
{
    Task<bool> CheckReplicationStatusAsync();
    Task<TimeSpan> GetReplicationLatencyAsync();
}