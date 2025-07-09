using Serilog;
using Serilog.Core;

namespace EIV_Common.Logger;

public static class MainLog
{
    public static ILogger? Logger { get; internal set; } = null;
    public static LoggingLevelSwitch LevelSwitch { get; internal set; } = new(Serilog.Events.LogEventLevel.Information);
    public static LoggingLevelSwitch ConsoleLevelSwitch { get; internal set; } = new(Serilog.Events.LogEventLevel.Information);
    public static LoggingLevelSwitch FileLevelSwitch { get; internal set; } = new(Serilog.Events.LogEventLevel.Information);

    /// <summary>
    /// Create new <see cref="Logger"/>
    /// </summary>
    public static void CreateNew()
    {
        Create(GetConfiguration());     
    }

    public static void Create(LoggerConfiguration config)
    {
        Logger = config.CreateLogger();;
        Logger.Information("Application started!");
        Log.Logger = Logger;
    }

    public static LoggerConfiguration GetConfiguration()
    {
        return new()
            .MinimumLevel.ControlledBy(LevelSwitch)
            .Enrich.With(new MethodEnricher())
            .WriteTo.File("logs.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({Method}) {Message:lj}{NewLine}{Exception}", levelSwitch: FileLevelSwitch)
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] ({Method}) {Message:lj}{NewLine}{Exception}", levelSwitch: ConsoleLevelSwitch);
    }


    /// <summary>
    /// Close the <see cref="Logger"/>
    /// </summary>
    public static void Close()
    {
        if (Logger == null)
            return;
        Logger.Information("Application closed!");
        Log.CloseAndFlush();
        Logger = null;
    }
}
