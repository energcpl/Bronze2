using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BronzeLibrary
{
    public class FormInfo
    {
        public Id id { get; set; }
        public string fileName { get; set; }
        public string title { get; set; }
        public int templateVersion { get; set; }
        public int sharedJsVersion { get; set; }
        public int jsVersion { get; set; }
        public int cssVersion { get; set; }
        public int isPortrait { get; set; }
        public int isMain { get; set; }
        public object validFrom { get; set; }
        public object expires { get; set; }
        public object imageFields { get; set; }
        public string templateFileName { get; set; }
        public string jsFileName { get; set; }
        public bool portrait { get; set; }
        public bool validToday { get; set; }
        public string cssfileName { get; set; }
        public bool main { get; set; }
    }
}
