using System;
using System.Drawing;
using System.Windows.Forms;

namespace CafeSystem
{
    public static class LogBox
    {
        /// <summary>
        ///     Тип логгирования, иначе говоря - цвет сообщения в журнале событий
        /// </summary>
        public enum LogType
        {
            Log, // Обычный - без цвета.
            Warning, // Предупреждение - жёлтый.
            Error, // Ошибка - красный.
            Success // "Успешно" - зелёный.
        }

        public static RichTextBox Rtb { get; set; }

        //public LogBox(RichTextBox textBox) => RTB = textBox;

        public static void Log(string message, LogType type = LogType.Log)
        {
            string currTime; // Текущее время
            Color currColor; // Текущий цвет (если вдруг цвет шрифта в richTextBox изменится)
            Rtb.BeginInvoke(new MethodInvoker(() =>
            {
                currTime = DateTime.Now.ToString("dd/MM HH:mm:ss");

                Rtb.SelectionStart = Rtb.TextLength;
                Rtb.SelectionLength = message.Length;
                currColor = Rtb.ForeColor;

                switch (type)
                {
                    case LogType.Log:
                        Rtb.BeginInvoke(new Action(() => { Rtb.AppendText($"[{currTime}] {message}\n"); }));
                        break;

                    case LogType.Warning:
                        Rtb.BeginInvoke(new Action(() =>
                        {
                            Rtb.SelectionColor = Color.DarkOrange;
                            Rtb.AppendText($"[{currTime}] {message}\n");
                            Rtb.SelectionColor = currColor;
                        }));
                        break;

                    case LogType.Error:
                        Rtb.BeginInvoke(new Action(() =>
                        {
                            Rtb.SelectionColor = Color.Red;
                            Rtb.AppendText($"[{currTime}] {message}\n");
                            Rtb.SelectionColor = currColor;
                        }));
                        break;

                    case LogType.Success:
                        Rtb.BeginInvoke(new Action(() =>
                        {
                            Rtb.SelectionColor = Color.Green;
                            Rtb.AppendText($"[{currTime}] {message}\n");
                            Rtb.SelectionColor = currColor;
                        }));
                        break;
                }
            }));
        }
    }
}