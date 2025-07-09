using System.Collections.Generic;
using System.Xml.Serialization;

namespace kun28.Models
{
    [XmlRoot("Data")]
    public class LinkTaskCollection
    {
        [XmlElement("ImageURL")]
        public string ImageURL { get; set; } = "D:/test/";

        [XmlElement("LinkTasksList")]
        public List<TaskParams> TaskParamsList { get; set; } = new List<TaskParams>();
    }
}
