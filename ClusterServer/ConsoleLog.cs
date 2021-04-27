using System;
using log4net;
using log4net.Core;

namespace Cluster
{
    public class ConsoleLog : ILog
    {
        public ILogger Logger { get; }
        public void Debug(object message)
        {
            Console.WriteLine(message);
        }

        public void Debug(object message, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public void DebugFormat(string format, object arg0)
        {
            Console.WriteLine(arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
            Console.WriteLine(arg2);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public void Info(object message)
        {
            Console.WriteLine(message);
        }

        public void Info(object message, Exception exception)
        {
            Console.WriteLine(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public void InfoFormat(string format, object arg0)
        {
            Console.WriteLine(arg0);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
            Console.WriteLine(arg2);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public void Warn(object message)
        {
            Console.WriteLine(message);
        }

        public void Warn(object message, Exception exception)
        {
            Console.WriteLine(message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public void WarnFormat(string format, object arg0)
        {
            Console.WriteLine(arg0);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
            Console.WriteLine(arg2);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public void Error(object message)
        {
            Console.WriteLine(message);
        }

        public void Error(object message, Exception exception)
        {
            Console.WriteLine(message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Console.WriteLine(args);

        }

        public void ErrorFormat(string format, object arg0)
        {
            Console.WriteLine(arg0);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
            Console.WriteLine(arg2);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public void Fatal(object message)
        {
            Console.WriteLine(message);
        }

        public void Fatal(object message, Exception exception)
        {
            Console.WriteLine(message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public void FatalFormat(string format, object arg0)
        {
            Console.WriteLine(arg0);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.WriteLine(arg0);
            Console.WriteLine(arg1);
            Console.WriteLine(arg2);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.WriteLine(args);
        }

        public bool IsDebugEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsWarnEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
    }
}