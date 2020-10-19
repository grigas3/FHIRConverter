using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIRConverter.Logging
{
    public class Logger
    {

        private static string SRExceptionThrown = "ExceptionThrown";

        private static ILog _log = LogManager.GetLogger(typeof(Logger));



        public static void Init(string file)
        {


            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

            Logging.RollingFileAppender fileAppender = new Logging.RollingFileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new Logging.FileAppender.MutexLock();
            fileAppender.File = file;

            fileAppender.RollingStyle = Logging.RollingFileAppender.RollingMode.Composite;
            PatternLayout pl = new PatternLayout();
            pl.ConversionPattern = "%d [%t] %-5p - %m%n";
            pl.ActivateOptions();
            fileAppender.Layout = pl;
            fileAppender.ActivateOptions();
            fileAppender.MaxFileSize = 20 * 1024 * 1024;
            fileAppender.MaxSizeRollBackups = 20;
            // fileAppender.DatePattern="yyyy-MM-dd.log";
            fileAppender.StaticLogFileName = true;
            fileAppender.PreserveLogFileNameExtension = false;
            log4net.Config.BasicConfigurator.Configure(fileAppender);

            //Test logger
            _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);




        }

        /// <summary>
        /// Determines if the specified <see cref="LogLevel"/> is enabled.
        /// </summary>
        /// <param name="category">The logging level to check.</param>
        /// <returns>true if the <see cref="LogLevel"/> is enabled, or else false.</returns>
        public static bool IsLogLevelEnabled(LogLevel category)
        {
            switch (category)
            {
                case LogLevel.Debug:
                    return _log.IsDebugEnabled;
                case LogLevel.Info:
                    return _log.IsInfoEnabled;
                case LogLevel.Warn:
                    return _log.IsWarnEnabled;
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Fatal:
                    return _log.IsFatalEnabled;
            }
            return false;
        }

        /// <summary>
        /// Logs the specified message at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="category">The logging level.</param>
        /// <param name="message">The message to be logged.</param>
        public static void Log(LogLevel category, object message)
        {
            // Just return without formatting if the log level isn't enabled
            if (!IsLogLevelEnabled(category)) return;

            Exception ex = message as Exception;
            if (ex != null)
            {
                switch (category)
                {
                    case LogLevel.Debug:
                        _log.Debug(SRExceptionThrown, ex);
                        break;
                    case LogLevel.Info:
                        _log.Info(SRExceptionThrown, ex);
                        break;
                    case LogLevel.Warn:
                        _log.Warn(SRExceptionThrown, ex);
                        break;
                    case LogLevel.Error:
                        _log.Error(SRExceptionThrown, ex);
                        break;
                    case LogLevel.Fatal:
                        _log.Fatal(SRExceptionThrown, ex);
                        break;
                }
            }
            else
            {
                switch (category)
                {
                    case LogLevel.Debug:
                        _log.Debug(message);
                        break;
                    case LogLevel.Info:
                        _log.Info(message);
                        break;
                    case LogLevel.Warn:
                        _log.Warn(message);
                        break;
                    case LogLevel.Error:
                        _log.Error(message);
                        break;
                    case LogLevel.Fatal:
                        _log.Fatal(message);
                        break;
                }
            }
        }

        /// <summary>
        /// Logs the specified message at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="category">The log level.</param>
        /// <param name="message">Format message, as used with <see cref="System.Text.StringBuilder"/>.</param>
        /// <param name="args">Optional arguments used with <paramref name="message"/>.</param>
        public static void Log(LogLevel category, String message, params object[] args)
        {
            // Just return without formatting if the log level isn't enabled
            if (!IsLogLevelEnabled(category)) return;

            StringBuilder sb = new StringBuilder();

            if (args == null || args.Length == 0)
                sb.Append(message);
            else
                sb.AppendFormat(message, args);

            switch (category)
            {
                case LogLevel.Debug:
                    _log.Debug(sb.ToString());
                    break;
                case LogLevel.Info:
                    _log.Info(sb.ToString());
                    break;
                case LogLevel.Warn:
                    _log.Warn(sb.ToString());
                    break;
                case LogLevel.Error:
                    _log.Error(sb.ToString());
                    break;
                case LogLevel.Fatal:
                    _log.Fatal(sb.ToString());
                    break;
            }
        }


        /// <summary>
        /// Logs the specified exception at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="ex">The exception to log.</param>
        /// <param name="category">The log level.</param>
        /// <param name="message">Format message, as used with <see cref="System.Text.StringBuilder"/>.</param>
        /// <param name="args">Optional arguments used with <paramref name="message"/>.</param>
        public static void Log(LogLevel category, Exception ex, String message, params object[] args)
        {
            // Just return without formatting if the log level isn't enabled
            if (!IsLogLevelEnabled(category)) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("EXCEPTION ");
            sb.AppendLine();

            if (args == null || args.Length == 0)
                sb.Append(message);
            else
                sb.AppendFormat(message, args);

            switch (category)
            {
                case LogLevel.Debug:
                    _log.Debug(sb.ToString(), ex);
                    break;
                case LogLevel.Info:
                    _log.Info(sb.ToString(), ex);
                    break;
                case LogLevel.Warn:
                    _log.Warn(sb.ToString(), ex);
                    break;
                case LogLevel.Error:
                    _log.Error(sb.ToString(), ex);
                    break;
                case LogLevel.Fatal:
                    _log.Fatal(sb.ToString(), ex);
                    break;
            }

        }


        public static void LogError(Exception ex, string source)
        {

            Log(LogLevel.Error, ex, source);
            //MessageBox.Show(String.Format("{0} throw exception {1}", source, ex.Message));
        }
    }
}
