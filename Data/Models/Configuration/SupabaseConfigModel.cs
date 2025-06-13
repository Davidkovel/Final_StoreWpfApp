namespace Data.Models;

public class SupabaseConfigsModel
{
    /// <summary>
    /// Gets or sets the URL of the Supabase service.
    /// This property is nullable and defaults to null.
    /// </summary>
    public string? Url { get; set; } = null;

    /// <summary>
    /// Gets or sets the API key for the Supabase service.
    /// This property is nullable and defaults to null.
    /// </summary>
    public string? Key { get; set; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupabaseConfigsModel"/> class with the specified URL and API key.
    /// </summary>
    /// <param name="url">The URL of the Supabase service.</param>
    /// <param name="key">The API key for the Supabase service.</param>
    public SupabaseConfigsModel(string url, string key)
    {
        Url = url;
        Key = key;
    }
}