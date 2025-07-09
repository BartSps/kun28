using System;
using kun28.utils;
using OpenCvSharp;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace kun28.Models
{
    public class LinkTask
    {
        [JsonIgnore]
        public Mat templateMat { get; set; }
        [JsonIgnore]
        public LinkTask TN { get; set; }
        [JsonIgnore]
        public LinkTask FN { get; set; }

        public int delayBefore { get; set; }
        [JsonIgnore]
        public Func<Task<LinkTask>> run { get; set; }
        [JsonIgnore]
        public static CustomTaskRunner ctr { get; set; }

        public static string path  = "D:/kun28image/";

        public string fileName { get; set; }

        public int times { get; set; }

        public int offsetX { get; set; }

        public int offsetY { get; set; }

        public bool click { get; set; }

        public TemplateMatchModes matchMode { get; set; }

        public LinkTask(string fileName,
            int delayBefore = 1000,int times = 1, LinkTask tN = null,
            LinkTask fN = null, Func<Task<LinkTask>> run = null,
            int offsetX = 0,int offsetY = 0,
            bool click = true,
            TemplateMatchModes matchMode = TemplateMatchModes.CCoeffNormed)
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
            int delay = delayBefore;
            while(delay > 0)
            {
                await ctr.checkPause();
                if (ctr.checkCancel()) break;
                await Task.Delay(100);
                delay -= 100;
            }
            int t = times;
            while (t > 0)
            {
                --t;
                await ctr.checkPause();
                if (ctr.checkCancel()) break;
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
                await Task.Delay(1000);
            }
            return res ? TN : FN;
        }
        
        public static LinkTask ConverterByParams(TaskParams taskParams)
        {
            return new LinkTask(taskParams.fileName, taskParams.delayBefore, taskParams.times,
                offsetX: taskParams.offsetX, offsetY: taskParams.offsetY, click: taskParams.click, matchMode: taskParams.matchMode);
        }
    }
}
