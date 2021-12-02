using log4net;
using log4net.Appender;
using System;
using System.Linq;
using System.Reflection;

namespace AspNetZeroRadToolVisualStudioExtension.Helpers
{
  public static class Logger
  {
    private static readonly ILog Instance = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public static void Error(string message, Exception ex = null) => Logger.Instance.Error((object) message, ex);

    public static void Info(string message) => Logger.Instance.Info((object) message);

    public static void Debug(string message) => Logger.Instance.Debug((object) message);

    public static void Warn(string message) => Logger.Instance.Warn((object) message);

    public static ILog InstanceFor(object loggerObject) => loggerObject == null ? Logger.InstanceFor((string) null) : Logger.InstanceFor(loggerObject.GetType());

    public static ILog InstanceFor(Type objectType) => !(objectType != (Type) null) ? LogManager.GetLogger(string.Empty) : LogManager.GetLogger(objectType);

    public static ILog InstanceFor(string loggerName) => LogManager.GetLogger(loggerName);

    public static string GetLogFilePathOrNull() => ((log4net.Repository.Hierarchy.Hierarchy) LogManager.GetRepository()).Root.Appenders.OfType<FileAppender>().FirstOrDefault<FileAppender>()?.File;
  }
}
