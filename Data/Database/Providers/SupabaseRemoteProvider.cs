using System;
using Supabase;

namespace Data.DBProvider.SupabaseRemote;

public interface ISupabaseRemoteProvider
{
    Client SupabaseClient { get; }
    Task InitializeAsync();
}

public class SupabaseRemoteProvider : ISupabaseRemoteProvider
{
    public Client SupabaseClient { get; }

    public SupabaseRemoteProvider( string key, string url)
    {
        var options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        SupabaseClient = new Client(url, key, options);
    }

    public async Task InitializeAsync()
    {
        await SupabaseClient.InitializeAsync();
    }
    //
    // public void Dispose()
    // {
    //     SupabaseClient.
    //     SupabaseClient?.Dispose();
    // }
}