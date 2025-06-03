using System;
using System.Collections.Generic;
using System.IO;

namespace DekstopApp;

public class ConfigLoader
{
    // TValue
    public static Dictionary<string, string> LoadConfigEnv()
    {
        var envPath = "C:\\Users\\David\\RiderProjects\\StoreWpfApp\\DekstopApp\\.env";
        Console.WriteLine(envPath);

        var host = DotNetEnv.Env.GetString("HOST");
        var port = DotNetEnv.Env.GetString("PORT");
        var username = DotNetEnv.Env.GetString("USERNAME");
        var password = DotNetEnv.Env.GetString("PASSWORD");
        var database = DotNetEnv.Env.GetString("DATABASE");

        var supabaseApiKey = DotNetEnv.Env.GetString("SUPABASE_API_KEY");
        var supabaseDBEndpoint = DotNetEnv.Env.GetString("SUPABASE_DB_ENDPOINT");

        Console.WriteLine(DotNetEnv.Env.GetString("HOST"));
        var config = new Dictionary<string, string>
        {
            { "Host", host },
            { "Port", port },
            { "Username", username },
            { "Password", password },
            { "Database", database },
            { "SupabaseApiKey", supabaseApiKey },
            { "SupabaseDBEndpoint", supabaseDBEndpoint },
        };

        foreach (var key in config.Keys)
        {
            if (string.IsNullOrEmpty(config[key]))
            {
                throw new Exception($"Environment variable {key} is not set.");
            }
        }

        return config;
    }

    public static Dictionary<string, string> LoadConfig()
    {
        var host = "localhost";
        var port = "1434";
        var username = "sa";
        var password = "123456I!@";
        var database = "Shop";

        var redisHost = "localhost:6379";

        var supabaseApiKey =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRuaWl1b3ZldXhhYmJtZXdvZXdjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDg1MDM3MDUsImV4cCI6MjA2NDA3OTcwNX0.fm4SPKHMfPamTkfMQTriO8UxIOd-0dJ3i4MfzS-wXDk";
        var supabaseDBEndpoint = "https://tniiuoveuxabbmewoewc.supabase.co";

        var config = new Dictionary<string, string>
        {
            { "Host", host },
            { "Port", port },
            { "Username", username },
            { "Password", password },
            { "Database", database },
            { "RedisHost", redisHost },
            { "SupabaseApiKey", supabaseApiKey },
            { "SupabaseDBEndpoint", supabaseDBEndpoint },
        };

        return config;
    }


    public static string GetConnectionDBString()
    {
        var config = LoadConfig();
        return $"Server={config["Host"]},{config["Port"]};" +
               $"Database={config["Database"]};" +
               $"User Id={config["Username"]};" +
               $"Password={config["Password"]};" +
               "TrustServerCertificate=True;";
    }

    public static string GetRedisConnectionString()
    {
        var config = LoadConfig();
        return config["RedisHost"];
    }

    public static List<string> GetSupabaseConnectionStrings()
    {
        var config = LoadConfig();
        return new List<string>
        {
            config["SupabaseApiKey"],
            config["SupabaseDBEndpoint"]
        };
    }
};