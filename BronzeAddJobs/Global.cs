using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BronzeLibrary;

namespace BronzeAddJobs
{
    public static class Global
    {
        static LoginObject _lo;

        public static LoginObject BRONZELOGIN
        {
            get
            {
                return _lo;
            }
            set
            {
                _lo = value;
            }
        }
    }
}