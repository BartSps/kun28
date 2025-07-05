using System;
using kun28.utils;
using OpenCvSharp;
using System.Threading.Tasks;

namespace kun28.Models
{
    public class LinkTask
    {
        public Mat templateMat;

        public LinkTask TN;

        public LinkTask FN;

        public int delayBefore;

        public Func<Task<LinkTask>> run;

        public static CustomTaskRunner ctr;

        public static readonly string path = "D:/kun28image/";

        public string fileName;

        public int times;

        public int offsetX;

        public int offsetY;

        public bool click;

        public TemplateMatchModes matchMode;

        public LinkTask(string fileName, int delayBefore = 1000,int times = 1, LinkTask tN = null, LinkTask fN = null, Func<Task<LinkTask>> run = null,int offsetX = 0,int offsetY = 0,bool click = true,TemplateMatchModes matchMode = TemplateMatchModes.CCoeffNormed)
        {
            this.fileName = fileName;
            if (!fileName.Contains(".")) fileName += ".png";
            templateMat = new Mat(path + fileName);
            TN = tN;
            FN = fN;
            this.delayBefore = delayBefore;
            this.run = run == null ? Run : run;
            this.times = times > 0 ? times : 1;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.click = click;
            this.matchMode = matchMode;
        }

        private async Task<LinkTask> Run()
        {
            bool res = false;
            await Task.Delay(delayBefore);
            int t = times;
            while (t > 0)
            {
                --t;
                await Task.Delay(1000);
                Point? p;
                if (!matchMode.Equals(TemplateMatchModes.SqDiffNormed)) p = CVTools.FindGrayImageOnScreen(templateMat);
                else p = CVTools.FindBySqDiffNormed(templateMat);
                await Task.Delay(100);
                await ctr.checkPause();
                if (ctr.checkCancel()) break;
                if (p != null)
                {
                    if (click)
                    {
                        await ctr.checkPause();
                        if (ctr.checkCancel()) break;
                        await MouseControlUtils.SmoothMoveTo((Point)p,offsetX:offsetX,offsetY:offsetY);
                        await ctr.checkPause();
                        if (ctr.checkCancel()) break;
                        await MouseControlUtils.Click();
                    }
                    res = true;
                    break;
                }
            }
            return res ? TN : FN;
        }
        
    }
}
