using Serilog;
using Serilog.Core;

namespace EIV_Common.Logger;

public static class MainLog
{
    public static ILogger? logger { get; internal set; } = null;
    public static LoggingLevelSwitch LevelSwitch { get; internal set; } = new()
    {
        MinimumLevel = Serilog.Events.LogEventLevel.Information,
    };

    public static LoggingLevelSwitch ConsoleLevelSwitch { get; internal set; } = new()
    {
        MinimumLevel = Serilog.Events.LogEventLevel.Information,
    };
    public static LoggingLevelSwitch FileLevelSwitch { get; internal set; } = new()
    {
        MinimumLevel = Serilog.Events.LogEventLevel.Information,
    };

    /// <summary>
    /// Create new <see cref="logger"/>
    /// </summary>
    public static void CreateNew()
    {
        logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LevelSwitch)
            .Enrich.With(new MethodEnricher())
            .WriteTo.File("logs.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({Method}) {Message:lj}{NewLine}{Exception}", levelSwitch: FileLevelSwitch)
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] ({Method}) {Message:lj}{NewLine}{Exception}", levelSwitch: ConsoleLevelSwitch)
            .CreateLogger();
        logger.Information("Application started!");
        Log.Logger = logger;
    }

    /// <summary>
    /// Close the <see cref="logger"/>
    /// </summary>
    public static void Close()
    {
        if (logger == null)
            return;
        logger.Information("Application closed!");
        Log.CloseAndFlush();
        logger = null;
    }
}
