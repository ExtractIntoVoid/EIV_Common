using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace EIV_Common.Logger;

public class MethodEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        StackTrace stackTrace = new StackTrace(true);
        StackFrame[] stackFrames = stackTrace.GetFrames();
        bool hasEmit = false;
        /*
        foreach (var item in stackFrames)
        {
            Console.WriteLine(item.ToString());
        }
        */
        if (stackFrames.Length > 2)
        {
            if (stackFrames[2].GetMethod() != null && stackFrames[2].GetMethod()!.Name.Contains("ILogEventSink"))
                hasEmit = true;
        }
        string methodName = "";
        if (hasEmit)
        {
            if (stackFrames.Length >= 7)
            {
                var method = stackFrames[7].GetMethod();
                if (method == null)
                    goto END;
                if (method.DeclaringType == null)
                    goto END;
                methodName = method.DeclaringType.FullName + "." + method.Name;
            }    
            else
            {
                Console.WriteLine("error x <7");
            }
        }
        else
        {
            if (stackFrames.Length >= 5)
            {
                var method = stackFrames[5].GetMethod();
                if (method == null)
                    goto END;
                if (method.DeclaringType == null)
                    goto END;
                methodName = method.DeclaringType.FullName + "." + method.Name;
            }   
            else
            {
                Console.WriteLine("error <5");
            }
        }
        END:
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Method", methodName));
    }
}