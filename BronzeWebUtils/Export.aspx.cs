using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BronzeLibrary;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.IO.Compression;
using BronzeDBLibrary;
using System.Diagnostics;

namespace BronzeWebUtils
{
    public partial class Export : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnWO_Click(object sender, EventArgs e)
        {
            MoreInfo mi = new MoreInfo();
            LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            string workorderID = DBStuff.GetWorkOrderID(Convert.ToInt32(txtJobid.Text));

            ExportWorkOrderpdf(lo, workorderID, ref mi);

        }

        public static void ExportWorkOrderpdf(LoginObject lo, string id, ref MoreInfo mi)
        {
            //LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            DateTime fromDate = DateTime.Today;
            string bodyData = "{\"workOrderId\" : \"" + id + "\"}";

            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);
            WebRequest request = WebRequest.Create("https://energ.mobilewkr.com/api/export/pdf");

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("mwkr-username", lo.user.username);
            request.Headers.Add("mwkr-email", lo.user.email);
            request.Headers.Add("mwkr-authkey", lo.auth_key);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(bodyData);
            request.ContentLength = bytes.Length;

            using (Stream os = request.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }

            MemoryStream str = new MemoryStream();

            using (System.Net.WebResponse resp = request.GetResponse())
            {

                var buf = new byte[1024];

                using (var sr = resp.GetResponseStream())
                {
                    int bytesRead;
                    while ((bytesRead = sr.Read(buf, 0, buf.Length)) > 0)
                    {
                        str.Write(buf, 0, bytesRead);
                    }
                }
            }
            str.Seek(0, SeekOrigin.Begin);
            var data = str.GetBuffer();
            File.WriteAllBytes(@"c:\test.zip", data);

            byte[] file = File.ReadAllBytes(@"c:\test.zip");
            //Process.Start(@"c:\test.zip");
        }
    }
}