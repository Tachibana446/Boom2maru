using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Boom2maru
{
    class WindowCapture
    {
        //クライアント領域キャプチャ用のメソッドと、戻り値用の構造体
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        private static extern int GetClientRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hwnd, out POINT lpPoint);

        /// <summary>
        /// 画面をキャプチャしてBitmapを返す。
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>クライアント領域のBitmap。キャプチャに失敗した場合null。</returns>
        public static Bitmap GetBitmap(IntPtr handle)
        {
            var rect = new RECT();
            if (GetClientRect(handle, out rect) == 0)
            {
                //キャプチャ失敗
                return null;
            }
            var size = new Size(rect.right - rect.left, rect.bottom - rect.top);
            if (size.Width <= 0 || size.Height <= 0)
            {
                //キャプチャ失敗
                return null;
            }
            var img = new Bitmap(size.Width, size.Height);
            var pt = new POINT { x = rect.left, y = rect.top };
            ClientToScreen(handle, out pt);
            using (var g = Graphics.FromImage(img))
            {
                g.CopyFromScreen(pt.x, pt.y, 0, 0, img.Size);
            }
            return img;
        }

        /// <summary>
        /// 画面をキャプチャしてBitmapImageを返す。
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>クライアント領域のBitmapImage。キャプチャに失敗した場合null。</returns>
        public static BitmapImage GetBitmapImage(IntPtr handle)
        {
            var bitmap = GetBitmap(handle);
            if (bitmap == null)
            {
                //キャプチャ失敗
                return null;
            }
            //変換処理
            //see http://stackoverflow.com/a/1069509
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}