using FishingTrainer;
using StardewModdingAPI;

public static class Log
{
    public static void Trace(string message)
    {
        ModEntry.Instance!.Monitor.Log(message, LogLevel.Trace);
    }
    public static void Debug(string message)
    {
        ModEntry.Instance!.Monitor.Log(message, LogLevel.Debug);
    }
    public static void Info(string message)
    {
        ModEntry.Instance!.Monitor.Log(message, LogLevel.Info);
    }
    public static void WarnOnce(string message)
    {
        ModEntry.Instance!.Monitor.LogOnce(message, LogLevel.Warn);
    }
    public static void Warn(string message)
    {
        ModEntry.Instance!.Monitor.Log(message, LogLevel.Warn);
    }
    public static void Error(string message)
    {
        ModEntry.Instance!.Monitor.Log(message, LogLevel.Error);
    }
    public static void Alert(string message)
    {
        ModEntry.Instance!.Monitor.Log(message, LogLevel.Alert);
    }
}