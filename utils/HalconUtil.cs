using System;
using HalconDotNet;
using System.Drawing;
using System.Drawing.Imaging;

namespace kun28.utils
{
    public class HalconUtil
    {
        public static MatchResult FAILD_MATCHRESULT = new MatchResult(-1,-1,-1);
        public static MatchResult matchByGrayTemplate(HImage ho_image, HTuple hv_modelID, HTuple hv_expireScore)
        {
            // Initialize local and output iconic variables

            HOperatorSet.FindShapeModel(ho_image, hv_modelID, -0.39, 0.79, hv_expireScore,
                1, 0.5, "least_squares", 0, 0.9, out HTuple hv_row, out HTuple hv_col, out HTuple hv_Angle, out HTuple hv_score);

            hv_Angle.Dispose();

            MatchResult res;

            if (hv_score.Length > 0 && hv_score.D > 0.9) res = new MatchResult(hv_row, hv_col, hv_score);
            else res = FAILD_MATCHRESULT;

            hv_expireScore.Dispose();
            ho_image.Dispose();
            hv_col.Dispose();
            hv_row.Dispose();
            hv_Angle.Dispose();
            hv_score.Dispose();

            return res;
        }

        public static HImage BitMapToHImage(Bitmap image)
        {
            Bitmap bmp32 = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            //确保bitmap是32位RGB格式
            using (Graphics g = Graphics.FromImage(bmp32))
            {
                g.DrawImage(image, 0, 0);
            }

            //锁定bitmap数据
            BitmapData bmpData = bmp32.LockBits(
                new Rectangle(0, 0, bmp32.Width, bmp32.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                HImage hImage = new HImage();

                hImage.GenImageInterleaved(
                    bmpData.Scan0,
                    "bgrx",
                    bmp32.Width,
                    bmp32.Height,
                    -1,
                    "byte",
                    bmp32.Width,
                    bmp32.Height,
                    0, 0, 8, 0);

                return hImage;
            }catch(Exception e)
            {
                throw new Exception("BitmapToHImageFailed: " + e.Message);
            }
            finally
            {
                bmp32.UnlockBits(bmpData);
                bmp32.Dispose();
            }

        }

        public class MatchResult
        {
            public int row = -1;
            public int col = -1;
            public double score = -1F;
            public OpenCvSharp.Point p;

            public MatchResult()
            {
            }

            public MatchResult(double row, double col, double score)
            {
                this.row = (int)row;
                this.col = (int)col;
                this.score = score;

                if (score > 0) p = new OpenCvSharp.Point(col,row);
            }

            public override string ToString()
            {
                return "row:" + row + " col:" + col + " score:" + score;
            }
        }
    }
}
