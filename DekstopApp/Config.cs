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

        Console.WriteLine(DotNetEnv.Env.GetString("HOST"));
        var config = new Dictionary<string, string>
        {
            { "Host", host },
            { "Port", port },
            { "Username", username },
            { "Password", password },
            { "Database", database },
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

        var config = new Dictionary<string, string>
        {
            { "Host", host },
            { "Port", port },
            { "Username", username },
            { "Password", password },
            { "Database", database },
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
};