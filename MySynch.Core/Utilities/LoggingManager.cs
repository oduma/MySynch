﻿using System;
using System.Diagnostics;
using System.Reflection;
using log4net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace MySynch.Core.Utilities
{
    public class Logger
    {
        internal Action<string, object[]> _implementation;

        public void Log(string format, params object[] args)
        {
            _implementation(format, args);
        }
    }

    public class LoggingManager : IDisposable
    {

        private static readonly ILog ACInvestPerformanceLog = LogManager.GetLogger("MySynch.PerformanceLog");

        private static readonly ILog ACInvestSystemErrorLog = LogManager.GetLogger("MySynch.SystemError");

        public static Logger Get(string name)
        {
            switch (name)
            {
                case "MySynch.PerformanceLog":
                    return new Logger() { _implementation = ACInvestPerformanceLog.InfoFormat };
                case "MySynch.SystemError":
                    return new Logger() { _implementation = ACInvestSystemErrorLog.ErrorFormat };
                default:
                    return default(Logger);
            }
        }

        private static readonly ILog ACInvestDebugLog = LogManager.GetLogger("MySynch.Debug");

        public ILog Log { get; internal set; }

        private readonly DateTime _StartTime;

        private DateTime _EndTime;

        private TimeSpan _ProcessTimeSpan;

        private string _CallerDetails;

        public object _performanceExtraInfo;

        #region Constructor and destructor

        private LoggingManager()
        {
            _StartTime = DateTime.Now;
        }

        public void Dispose()
        {
            _EndTime = DateTime.Now;
            _ProcessTimeSpan = _EndTime.Subtract(_StartTime);
            ACInvestPerformanceLog.Info(() => string.Format("{0} ({1}) - Finished: {2}", _CallerDetails, _performanceExtraInfo, _ProcessTimeSpan));
        }

        #endregion

        #region Methods

        #region Log ACInvest Process

        public static LoggingManager LogACInvestPerformance()
        {
            LoggingManager result;

            result = new LoggingManager()
            {
                _CallerDetails = GetCallerDetails().ToString(),
                Log = ACInvestPerformanceLog
            };

            ACInvestPerformanceLog.Info(() => string.Format("{0} - Started", result._CallerDetails));

            return result;
        }

        public static LoggingManager LogACInvestPerformance(object info)
        {
            LoggingManager result;

            result = new LoggingManager()
            {
                _CallerDetails = GetCallerDetails().ToString(),
                _performanceExtraInfo = info,
                Log = ACInvestPerformanceLog
            };

            ACInvestPerformanceLog.Info(() => string.Format("{0} ({1})- Started", result._CallerDetails, result._performanceExtraInfo));

            return result;
        }

       #endregion

        public static void LogACInvestSystemError(Exception exception)
        {
            LogACInvestSystemError(null, exception);
        }

        public static void LogACInvestSystemError(object message, Exception exception)
        {
            ACInvestSystemErrorLog.Error(message, exception);
        }
        public static void Debug(object message)
        {
            using (PushContext("ACInvestDebug", GetCallerDetails().ToString()))
            {
                ACInvestDebugLog.Debug(message);
            }
        }

        public static IDisposable PushContext(string context, string value)
        {
            return ThreadContext.Stacks[context].Push(value);
        }

        private struct ClassAndMethod
        {
            public string ClassName;
            public string MethodName;

            public new string ToString()
            {
                return ClassName + "." + MethodName;
            }
        }

        private static ClassAndMethod GetCallerDetails()
        {
            var result = new ClassAndMethod { ClassName = "Unknown", MethodName = "Unknown" };

            var trace = new StackTrace(false);

            for (var index = 0; index <= trace.FrameCount - 1; index++)
            {
                StackFrame frame = trace.GetFrame(index);
                MethodBase method = frame.GetMethod();

                if (!method.DeclaringType.Equals(typeof(LoggingManager))
                    && !method.DeclaringType.Equals(typeof(Log4NetExtensionMethods)))
                {
                    result.ClassName = method.ReflectedType.FullName;
                    result.MethodName = method.Name;

                    // I would like to get the values that created the exception, but could not find a simple way to get them

                    break;
                }
            }

            return result;
        }

        #endregion
    }
}
