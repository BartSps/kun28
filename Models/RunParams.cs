using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace kun28.Models
{
    [XmlRoot("RunParams")]
    public class RunParams
    {
        public string TaskDataURL { get; set; } = "D:/test";

        public bool DeBugger { get; set; } = false;

    }
}
