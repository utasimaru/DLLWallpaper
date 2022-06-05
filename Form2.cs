using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgrammableWallpaper
{
    public partial class Form2 : Form
    {
        IntPtr handle_workerw;
        IntPtr dc_workerw;
        IntPtr handle_form;
        IntPtr dc_form;
        dynamic drawclass;
        public Form2()
        {
            //フォームの基本設定
            InitializeComponent();
        }
        public void DLLLoadPlay(string dllFileName)
        {
            try
            {
                var asm = Assembly.LoadFrom(dllFileName);
                var module = asm.GetModule(Path.GetFileName(dllFileName));
                var DrawClass = module.GetType(Path.GetFileNameWithoutExtension(dllFileName) + ".DrawClass");
                drawclass = Activator.CreateInstance(DrawClass);
                DrawLoop();
            }
            catch
            {
                MessageBox.Show("DLLの読み込みでエラーが起きました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DrawLoop()
        {
            if (Program.drawing) return;
            Program.drawing = true;
            Task.Run(() =>
            {
                //ポインタの設定
                handle_workerw = IntPtr.Zero;
                User32.EnumWindows((hwnd, lParam) =>
                {
                    IntPtr shell = User32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                    if (shell != IntPtr.Zero) handle_workerw = User32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                    return true;
                }, IntPtr.Zero);
                dc_workerw = User32.GetDCEx(handle_workerw, IntPtr.Zero, 0x403);

                try
                {
                    drawclass.DrawStart();

                    int tick, timeToNext = 0;
                    int w = Screen.PrimaryScreen.Bounds.Width;
                    int h = Screen.PrimaryScreen.Bounds.Height;
                    handle_form = User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Form2");
                    dc_form = User32.GetDC(handle_form);
                    while (Program.drawing)
                    {
                        tick = Environment.TickCount;

                        pictureBox1.Image = drawclass.GetDrawBitmap();
                        GDI32.BitBlt(dc_workerw, 0, 0, w, h, dc_form, 0, 0, 0x00CC0020);
                        timeToNext = Environment.TickCount + Program.framerate - tick;
                        if (timeToNext > 0) Thread.Sleep(timeToNext);
                    }
                }
                catch
                {
                    MessageBox.Show("描写中にエラーが起きました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                User32.ReleaseDC(handle_form, dc_form);
                User32.ReleaseDC(handle_workerw, dc_workerw);
                Close();
            });
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.drawing = false;
        }
    }
}
