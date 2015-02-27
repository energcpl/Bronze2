using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BronzeLibrary
{
    public class User
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public List<int> roles { get; set; }
        public bool isSignOffEmailRecipient { get; set; }
        public string serverId { get; set; }
        public string idOrBlank { get; set; }
        public bool portalAdmin { get; set; }
        public bool platformAdmin { get; set; }
        public bool portalEngineer { get; set; }
        public bool tabletEngineer { get; set; }
        public string displayName { get; set; }

    }
}
