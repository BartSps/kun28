using OpenCvSharp;
using System.Drawing;
using Point = OpenCvSharp.Point;

namespace kun28.utils
{
    public class CVTools
    {
        static int x = 0, y = 1035;
        static string name = "0";

        // 模板匹配
        public static Point? FindGrayImageOnScreen(Mat templateImage)
        {
            if (templateImage.Empty())
            {
                Cv2.ImShow("end", templateImage);
                return null;
            }
            Bitmap screenImage;
            if (false)
            {
                name = x + "," + y;
                Cv2.ImShow(name, templateImage);
                Cv2.MoveWindow(name, x, y);
                x += 50;
                if (x > 2260)
                {
                    x = 0;
                    y += 30;
                }
                screenImage = ScreenImageUtils.CaptureRegion(new Rectangle(0, 0, 2000, 980));
            }
            else screenImage = ScreenImageUtils.CaptureScreen();
            // 捕获屏幕
            using (var screen = screenImage)
            using (var screenMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(screen))
            {
                // 转换为灰度图像
                using (var screenGray = screenMat.CvtColor(ColorConversionCodes.BGR2GRAY))
                using (var templateGray = templateImage.CvtColor(ColorConversionCodes.BGR2GRAY))
                {
                    // 模板匹配
                    using (var result = new Mat())
                    {
                        Cv2.MatchTemplate(screenGray, templateGray, result, TemplateMatchModes.CCoeffNormed);
                        //Cv2.Threshold(result, result, 0.8, 1.0, ThresholdTypes.Binary);
                        Cv2.MinMaxLoc(result, out _, out double maxVal, out _ ,out Point maxLoc);

                        if (maxVal > 0.8)
                        {
                            int offsetX = templateImage.Width / 2;
                            int offsetY = templateImage.Height / 2;
                            maxLoc.X += offsetX;
                            maxLoc.Y += offsetY;

                            return maxLoc;
                        }
                    }
                }
            }

            return null;
        }

        // 平方差匹配
        public static Point? FindBySqDiffNormed(Mat templateImage)
        {
            if (templateImage.Empty())
            {
                Cv2.ImShow("end", templateImage);
                return null;
            }
            Bitmap screenImage;
            if (false)
            {
                name = x + "," + y;
                Cv2.ImShow(name, templateImage);
                Cv2.MoveWindow(name, x, y);
                x += 50;
                if (x > 2260)
                {
                    x = 0;
                    y += 30;
                }
                screenImage = ScreenImageUtils.CaptureRegion(new Rectangle(0, 0, 2000, 980));
            }else screenImage = ScreenImageUtils.CaptureScreen();
            // 捕获屏幕
            using (var screen = screenImage)
            using (var screenMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(screen))
            {
                // 转换为灰度图像
                using (var screenGray = screenMat.CvtColor(ColorConversionCodes.BGR2GRAY))
                using (var templateGray = templateImage.CvtColor(ColorConversionCodes.BGR2GRAY))
                {
                    // 模板匹配
                    using (var result = new Mat())
                    {
                        Cv2.MatchTemplate(screenGray, templateGray, result, TemplateMatchModes.SqDiffNormed);
                        Cv2.MinMaxLoc(result, out double minVal, out _, out Point minLoc, out _);

                        if (minVal < 0.05)
                        {
                            int offsetX = templateImage.Width / 2;
                            int offsetY = templateImage.Height / 2;
                            minLoc.X += offsetX;
                            minLoc.Y += offsetY;

                            return minLoc;
                        }
                    }
                }
            }

            return null;
        }

        public static void init()
        {
            x = 0;
            y = 1035;
            name = "0";
        }
    }
}
