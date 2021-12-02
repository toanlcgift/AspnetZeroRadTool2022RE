using System.Windows.Forms;

namespace AspNetZeroRadToolVisualStudioExtension.Helpers
{
  public class MsgBox
  {
    public static string Caption = AppSettings.AppName;

    public static void Warn(string text) => MsgBox.Warn(text, MsgBox.Caption);

    public static void Warn(string text, string caption)
    {
      Logger.Warn(text);
      int num = (int) MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    public static void Info(string text) => MsgBox.Info(text, MsgBox.Caption);

    public static void Info(string text, string caption)
    {
      Logger.Info(text);
      int num = (int) MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    public static void Exception(string text) => MsgBox.Exception(text, MsgBox.Caption);

    public static void Exception(string text, string caption) => MsgBox.Exception(text, (System.Exception) null, caption);

    public static void Exception(string text, System.Exception ex) => MsgBox.Exception(text, ex, MsgBox.Caption);

    public static void Exception(string text, System.Exception ex, string caption)
    {
      if (ex != null)
        text = text + "\n\rDetails: " + ex.Message;
      if (ex == null)
      {
        if (!string.IsNullOrWhiteSpace(text))
          Logger.Error(text);
      }
      else
        ExceptionHandler.Log(ex, text);
      int num = (int) MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }

    public static DialogResult Question(string text, params object[] args) => MsgBox.Question(string.Format(text, args));

    public static DialogResult Question(string text) => MsgBox.Question(text, MessageBoxButtons.YesNo);

    public static DialogResult Question(string text, MessageBoxButtons buttons) => MsgBox.Question(text, buttons, MsgBox.Caption);

    public static DialogResult Question(
      string text,
      MessageBoxButtons buttons,
      string caption)
    {
      return MessageBox.Show(text, caption, buttons, MessageBoxIcon.Question);
    }
  }
}
