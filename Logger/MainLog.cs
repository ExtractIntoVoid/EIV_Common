using Serilog;

namespace EIV_Common.Logger;

public static class MainLog
{
    public static ILogger? logger = null;

    /// <summary>
    /// Create new <see cref="logger"/>
    /// </summary>
    public static void CreateNew()
    {
        logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Information()
#endif
            .Enrich.With(new MethodEnricher())
            //.MinimumLevel.Override("EIV_JsonLib", Serilog.Events.LogEventLevel.Information)
            .WriteTo.File("logs.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({Method}) {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] ({Method}) {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        logger.Information("Application started!");
    }

    /// <summary>
    /// Close the <see cref="logger"/>
    /// </summary>
    public static void Close()
    {
        if (logger != null) 
        { 
            logger.Information("Application closed!");
            Log.CloseAndFlush();
            logger = null;
        }
    }
}
