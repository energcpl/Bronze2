using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergLibrary;

namespace BronzeLibrary
{
    public class WorkOrderRef
    {
        public string jdeReference { get; set;}
        public int visitID { get; set; }
        public UnitInfo ui { get; set; }
        public Address add { get; set; }
        public string comment { get; set; }
    }
}
