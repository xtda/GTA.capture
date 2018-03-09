using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GTA.capture {
    public partial class MainWindow : Form {
        private readonly string _currentPath = Path.GetDirectoryName(Application.ExecutablePath);

        public MainWindow() {
            InitializeComponent();

            Directory.CreateDirectory(_currentPath + "\\screenshots");

            var screenShotHotKeyId = 0;
            RegisterHotKey(Handle, screenShotHotKeyId, 0, Keys.F11.GetHashCode());
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            UnregisterHotKey(Handle, 0);
            Application.Exit();
        }

        private void screenshotsToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start(_currentPath + @"\screenshots");
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
                CaptureGtaScreen();
        }

        public void CaptureGtaScreen() {
            var proc = Process.GetProcessesByName("notepad")[0];
            var rect = new Rect();
            GetWindowRect(proc.MainWindowHandle, ref rect);

            var width = rect.right - rect.left;
            var height = rect.bottom - rect.top;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            var date = DateTime.Now;
            var month = date.ToString("MMMM");
            var filename = date.ToString("dd-MM-yyyy (HH-mm-ss)");
            Directory.CreateDirectory(_currentPath + "\\screenshots\\" + month);
            bmp.Save("screenshots\\" + month + "\\" + filename + ".png", ImageFormat.Png);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}