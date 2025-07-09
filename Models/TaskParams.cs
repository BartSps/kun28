using OpenCvSharp;


namespace kun28.Models
{
    public class TaskParams
    {
        private static string NULLSTRING = "null";

        public string taskName { get; set; } = NULLSTRING;
        public string fileName { get; set; } = NULLSTRING;
        public int delayBefore { get; set; } = 1000;
        public int times { get; set; } = 1;
        public int offsetX { get; set; } = 0;
        public int offsetY { get; set; } = 0;
        public bool click { get; set; } = true;
        public TemplateMatchModes matchMode { get; set; } = TemplateMatchModes.CCoeffNormed;
        public string TN { get; set; } = NULLSTRING;
        public string FN { get; set; } = NULLSTRING;

        public override string ToString()
        {
            return taskName + " { fileName : " + fileName + ", delayBefore : " + delayBefore
                + ", times " + times + ", offsetX " + offsetX + ", offsetY : " + offsetY + ", click : " + click + ", TN :  " + TN + ", FN : " + FN + " }";
        }
    }
}
