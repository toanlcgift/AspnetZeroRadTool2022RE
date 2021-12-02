using System;
using System.Threading;

namespace AspNetZeroRadToolVisualStudioExtension.Helpers
{
  public static class ExceptionHandler
  {
    public static void HandleThreadException(object sender, ThreadExceptionEventArgs e) => ExceptionHandler.Log(e.Exception);

    public static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e) => ExceptionHandler.Log(e.ExceptionObject as Exception);

    public static void Log(Exception ex, string message = null)
    {
      if (ex == null)
        return;
      Logger.Error(message ?? ex.Message, ex);
    }
  }
}
