using Prometheus;

namespace DekstopApp.Utils;

public static class AppMetrics
{
    // Счетчики
    public static readonly Counter LoginAttempts = Metrics
        .CreateCounter("login_attempts_total", "Number of login attempts");

    public static readonly Counter FailedLogins = Metrics
        .CreateCounter("failed_logins_total", "Number of failed login attempts");

    // Гейджи
    public static readonly Gauge ActiveSessions = Metrics
        .CreateGauge("active_sessions", "Number of active sessions");

    public static readonly Gauge MemoryUsage = Metrics
        .CreateGauge("memory_usage_mb", "Current memory usage in MB");

    // Гистограммы
    public static readonly Histogram LoginDuration = Metrics
        .CreateHistogram("login_duration_seconds", "Time spent processing login");
    
    public static readonly Histogram LoadProductsDuration = Metrics
        .CreateHistogram("load_products_duration_seconds", "Time spent loading products");

    
    // public static void Initialize()
    // {
    //     Task.Run(async () =>
    //     {
    //         while (true)
    //         {
    //             ActiveSessions.Set(GetCurrentSessions());
    //             await Task.Delay(1000);
    //         }
    //     });
    // }
}