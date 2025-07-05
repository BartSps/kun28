using System.Drawing;
using System.Windows.Forms;

namespace kun28.utils
{
    class ScreenImageUtils
    {
        // 捕获整个屏幕
        public static Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            return bitmap;
        }

        // 捕获特定区域
        public static Bitmap CaptureRegion(Rectangle region)
        {
            Bitmap bitmap = new Bitmap(region.Width, region.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(region.Location, Point.Empty, region.Size);
            }

            return bitmap;
        }
    }
}
