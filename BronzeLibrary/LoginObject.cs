using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BronzeLibrary
{
    public class LoginObject
    {
        public Success success { get; set; }
        public User user { get; set; }
        public string auth_key { get; set; }
    }
}
