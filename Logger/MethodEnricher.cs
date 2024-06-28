using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace EIV_Common.Logger
{
    public class MethodEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            StackTrace stackTrace = new StackTrace(true);
            StackFrame[] stackFrames = stackTrace.GetFrames();
            bool HasEmit = false;
            /*
            foreach (var item in stackFrames)
            {
                Console.WriteLine(item.ToString());
            }
            */
            if (stackFrames.Length > 2)
            {
                if (stackFrames[2].GetMethod() != null && stackFrames[2].GetMethod()!.Name.Contains("ILogEventSink"))
                    HasEmit = true;
            }
            string methodName = "";
            if (HasEmit)
            {
                if (stackFrames.Length >= 7)
                {
                    methodName = stackFrames[7].GetMethod().DeclaringType.FullName + "." + stackFrames[7].GetMethod().Name;
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
                    methodName = stackFrames[5].GetMethod().DeclaringType.FullName + "." + stackFrames[5].GetMethod().Name;
                }   
                else
                {
                    Console.WriteLine("error <5");
                }
            }
            
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Method", methodName));
        }
    }
}
