using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BronzeAddJobs
{
    public class Jobs
    {
        public Jobs()
        {
        }

        public int jobid { get; set; }
        public int visitid { get; set; }
        public int unitid { get; set; }
        public string displayid { get; set; }
        public int trip_no { get; set; }
        public DateTime datein { get; set; }
        public string comment { get; set; }
        public string engInit { get; set; }
        public string added { get; set; }
    }
}