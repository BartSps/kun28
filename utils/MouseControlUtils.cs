
using OpenCvSharp;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace kun28.utils
{
    class MouseControlUtils
    {
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        private static Random rand = new Random();

        // 鼠标事件常量
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        // 获取当前鼠标位置
        public static Point GetCurrentPosition()
        {
            GetCursorPos(out POINT point);
            return new Point(point.X, point.Y);
        }

        // 移动鼠标到指定位置
        public static void MoveTo(int x, int y)
        {
            SetCursorPos(x, y);
        }

        // 移动鼠标相对当前位置
        public static void MoveRelative(int deltaX, int deltaY)
        {
            var current = GetCurrentPosition();
            SetCursorPos(current.X + deltaX, current.Y + deltaY);
        }

        // 平滑移动到目标位置
        public static async Task SmoothMoveTo(Point p, int durationMs = 100,int offsetX = 0,int offsetY = 0)
        {
            p.X += offsetX;
            p.Y += offsetY;
            p = randPoint(p);
            var start = GetCurrentPosition();
            int steps = durationMs / 16; // 约60fps
            for (int i = 0; i <= steps; i++)
            {
                double progress = (double)i / steps;
                int x = (int)(start.X + (p.X - start.X) * progress);
                int y = (int)(start.Y + (p.Y - start.Y) * progress);
                SetCursorPos(x, y);
                await Task.Delay(16);
            }
        }

        //click
        public static async Task Click(uint times = 1 ,int delay_ms = 100)
        {
            if (delay_ms > 0) await Task.Delay(delay_ms);

            do
            {
                
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                await Task.Delay(delay_ms);
                times--;
            } while (times > 0);
        }

        //move and click
        public static async Task moveAndClick(Point p, int delay_ms = 100)
        {
            await SmoothMoveTo(p);
            await Task.Delay(delay_ms);
            await Click();
        }

        public static Point randPoint(Point p)
        {
            //p.X += rand.Next() % 10 - 5;
            //p.Y += rand.Next() % 10 - 5;
            return p;
        }
    }
}
