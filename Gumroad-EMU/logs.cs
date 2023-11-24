using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gumroad_EMU
{
    public static class Logs
    {
        // A reference to the RichTextBox control
        private static RichTextBox txtConsole;

        // A method to set the RichTextBox control
        public static void SetConsole(RichTextBox console)
        {
            txtConsole = console;
        }

        // A method to write a log message with a specified level and color
        private static void WriteLog(string level, string message, Color color)
        {
            // Get the current time in HH:mm:ss format
            string time = DateTime.Now.ToString("HH:mm:ss");

            // Append the time, level and message to the RichTextBox with the specified color
            txtConsole.SelectionStart = txtConsole.TextLength;
            txtConsole.SelectionLength = 0;
            txtConsole.SelectionColor = Color.DarkMagenta;
            txtConsole.AppendText($"[{time}] ");
            txtConsole.SelectionColor = Color.Cyan;
            txtConsole.AppendText(level);
            txtConsole.SelectionColor = color;
            txtConsole.AppendText($" {message}\n");
            txtConsole.SelectionColor = txtConsole.ForeColor;
        }

        // A method to write an info log message with light green color
        public static void Info(string message)
        {
            WriteLog("[Info]", message, Color.White);
        }

        // A method to write a warn log message with yellow color
        public static void Warn(string message)
        {
            WriteLog("[Warn]", message, Color.Yellow);
        }

        // A method to write an error log message with red color
        public static void Error(string message)
        {
            WriteLog("[Error]", message, Color.Red);
        }
    }
}
