using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DropQuery
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!File.Exists("SQLite.Interop.dll"))
            {
                byte[] data = Properties.Resources.SQLite_Interop;
                FileStream fileStream = new FileStream("SQLite.Interop.dll", FileMode.Create);
                fileStream.Write(data, 0, (int)(data.Length));
                fileStream.Close();
                FileInfo info = new FileInfo("SQLite.Interop.dll");
                info.Attributes = FileAttributes.Hidden;
            }
            Application.Run(new Form1());
        }
    }
}
