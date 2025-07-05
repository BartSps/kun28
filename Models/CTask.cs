using kun28.utils;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kun28.Models
{
    public class CTask
    {
        string name;

        string url;

        public Func<Task<bool>> run;

        Mat templateImage;

        public bool isLoop;

        public int delayBefore;

        public bool resultWhenFail;

        public bool continueFlag;

        public static CustomTaskRunner ctr;




        public CTask(string name, string url, Func<Task<bool>> run = null, bool isLoop = false, int delayBefore = 1000, bool resultWhenFail = true,bool continueFlag = false)
        {
            this.name = name;
            this.url = url;
            this.run = run;
            templateImage = new Mat(url);
            this.isLoop = isLoop;
            this.delayBefore = delayBefore;
            this.resultWhenFail = resultWhenFail;
            this.continueFlag = continueFlag;
            this.run = run == null ? Run : run;
        }

        private async Task<bool> Run()
        {
            if (continueFlag) return true;
            Point? point;
            do
            {
                await Task.Delay(1000);
                point = CVTools.FindGrayImageOnScreen(templateImage);
                if (ctr != null && ctr.checkCancel()) return false;
                await ctr?.checkPause();
            } while (isLoop && point == null);
            if (point == null) return resultWhenFail;
            await ctr?.checkPause();
            await MouseControlUtils.SmoothMoveTo((Point)point);
            await ctr?.checkPause();
            await MouseControlUtils.Click();
            return true;
        }
    }
}
