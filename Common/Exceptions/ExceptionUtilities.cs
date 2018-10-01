using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace RulesService.Exceptions
{
    public static class ExceptionUtilities
    {
        public static string FormatException(
            Exception ex,
            bool includeStackTrace = true,
            bool includeContext = false,
            int maxInnerExceptionDepth = 5)
        {
            if (ex == null) return string.Empty;

            var sb = new StringBuilder();
            try
            {
                // Whether or not to include the application and machine context
                if (includeContext)
                {
                    AppendContext(sb);
                }

                AppendExceptionInfo(sb, ex, 0);
            }
            catch (Exception ex0)
            {
                sb.AppendFormat("Warning; Could not format exception {0}", ex0.ToString());
            }

            return sb.ToString();
        }

        private static void AppendExceptionInfo(
            StringBuilder sb,
            Exception exception, int depth)
        {
            Func<Exception, string> formatter = defaultFormatter;
            if (FormatExceptionMap.ContainsKey(exception.GetType()))
                formatter = FormatExceptionMap[exception.GetType()];

            sb.AppendFormat("\r\n------------------------------\r\n{0}",
                formatter(exception));
        }

        private static void AppendContext(StringBuilder sb)
        {
            var currentAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var lastWritten = File.GetLastWriteTime(currentAssembly.Location);

            sb.AppendFormat("[Context] assembly={0}, version={1}, buildTime={2}, appDomain={3}, basePath={4}",
                currentAssembly.FullName,
                currentAssembly.GetName().Version.ToString(),
                lastWritten,
                AppDomain.CurrentDomain.FriendlyName,
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
        }

        private static readonly IDictionary<Type, Func<Exception, string>> FormatExceptionMap = new Dictionary<Type, Func<Exception, string>>()
        {
            { typeof(SqlException), (ex) => FormatSqlException(ex) }
        };

        private static string FormatSqlException(Exception ex)
        {
            if (!(ex is SqlException sqlEx)) return string.Empty;

            var sb = new StringBuilder();

            sb.AppendLine(ex.ToString());
            for (int i = 0; i < sqlEx.Errors.Count; i++)
            {
                var sqlErr = sqlEx.Errors[i];
                sb.AppendFormat("[#{0}] Message: {0}, LineNumber: {1}, Source: {2}, Procedure: {3}\r\n",
                    sqlErr.Number, sqlErr.Message, sqlErr.LineNumber, sqlErr.Source, sqlErr.Procedure);
            }
            return sb.ToString();
        }

        private static readonly Func<Exception, string> defaultFormatter = (ex) => (ex == null) ? string.Empty : ex.ToString();
    }
}
