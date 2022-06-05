using System;
using System.Windows.Forms;

namespace ProgrammableWallpaper
{
    internal static class Program
    {
        public static bool drawing;
        public static int framerate=100;
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
