#region Imports
using System;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
#endregion

namespace ifcodes.ifconfig.Console
{
    public class CSVFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            output.Write(logEvent.Timestamp.ToString("MM-dd-yyyy h:mm a"));
            output.Write(",");
            output.Write(logEvent.Level);
            output.Write(",");
            output.Write(logEvent.MessageTemplate);
            output.Write(",");

            if (logEvent.Exception != null)
            {
                string stackTrace = logEvent.Exception.StackTrace;

                stackTrace = stackTrace.Replace(Environment.NewLine, string.Empty);

                stackTrace = "\"" + stackTrace + "\""; 

                output.Write(stackTrace);

                output.Write(",");
            }

            output.WriteLine();
        }
    }
}