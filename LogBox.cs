using System;
using System.Drawing;
using System.Windows.Forms;

namespace CafeSystem
{
    public static class LogBox
    {
        public enum LogType
        {
            Log,
            Warning,
            Error,
            Succes
        }

        public static RichTextBox RTB { get; set; }

        //public LogBox(RichTextBox textBox) => RTB = textBox;

        public static void Log(string message, LogType type = LogType.Log)
        {
            RTB.BeginInvoke(new MethodInvoker(() =>
            {
                var currTime = DateTime.Now.ToString("dd/MM HH:mm:ss");

                RTB.SelectionStart = RTB.TextLength;
                RTB.SelectionLength = message.Length;
                var currColor = RTB.ForeColor;

                switch (type)
                {
                    case LogType.Log:
                        RTB.BeginInvoke(new Action(() => { RTB.AppendText($"[{currTime}] {message}\n"); }));
                        break;

                    case LogType.Warning:
                        RTB.BeginInvoke(new Action(() =>
                        {
                            RTB.SelectionColor = Color.DarkOrange;
                            RTB.AppendText($"[{currTime}] {message}\n");
                            RTB.SelectionColor = currColor;
                        }));
                        break;

                    case LogType.Error:
                        RTB.BeginInvoke(new Action(() =>
                        {
                            RTB.SelectionColor = Color.Red;
                            RTB.AppendText($"[{currTime}] {message}\n");
                            RTB.SelectionColor = currColor;
                        }));
                        break;

                    case LogType.Succes:
                        RTB.BeginInvoke(new Action(() =>
                        {
                            RTB.SelectionColor = Color.Green;
                            RTB.AppendText($"[{currTime}] {message}\n");
                            RTB.SelectionColor = currColor;
                        }));
                        break;
                }
            }));
        }
    }
}