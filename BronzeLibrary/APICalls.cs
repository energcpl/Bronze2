using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace BronzeLibrary
{
    public class APICalls
    {
        public static LoginObject Login( ref MoreInfo mi)
        {
            String result = "";
            String strPost = "username=ENERG_ADMIN" + "&password=mobileWkr101";
            StreamWriter myWriter = null;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://energ.mobilewkr.com/api/login");
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            System.Net.ServicePointManager.Expect100Continue = false;

            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed Authenticating" + e.Message);
            }
            finally
            {
                myWriter.Close();
            }
            try
            {
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

                using (StreamReader sr =
                new StreamReader(objResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();

                    // Close and clean up the StreamReader
                    sr.Close();
                }
                LoginObject lo = JsonConvert.DeserializeObject<LoginObject>(result);
                mi.bronzeInfo = String.Format("Status %d key %s", lo.success.status, lo.auth_key);
                return lo;

            }
            catch (Exception e)
            {
                mi.bronzeInfo = "Failed Authenticating";
                Console.Out.WriteLine("Failed Authenticating" + e.Message);
                return null;
            }
        }

        public static WorkOrderObject addWorkOrder(LoginObject lo, string engineer, WorkOrderRef wor, ref MoreInfo mi, DateTime ScheduledDate)
        {
            //LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            DateTime fromDate = DateTime.Today;
            string bodyData = "{\"userAssignedTo\" : \"" + engineer + "\", ";
            bodyData = bodyData + "\"dateScheduled\" : \"" + ScheduledDate.ToString("yyyy-MM-dd") + "\", ";
            bodyData = bodyData + "\"dateScheduledEnd\" : \"" + ScheduledDate.ToString("yyyy-MM-dd") + "\", ";
            bodyData = bodyData + "\"idCustomerAlias\" : \"" + wor.jdeReference + "\", ";
            bodyData = bodyData + "\"idCustomerAlias2\" : \"" + wor.visitID + "\", ";
            bodyData = bodyData + "\"customerName\" : \"" + wor.ui.NAME + "\", ";
            bodyData = bodyData + "\"description\" : \"" + wor.comment + "\", ";
            bodyData = bodyData + "\"sitePostCode\" : \"" + wor.add.PostCode + "\", ";
            bodyData = bodyData + "\"siteAddress\" : \"" + wor.add.FullAddress + "\", ";
            bodyData = bodyData + "\"idCustomerAlias3\" : \"" + wor.ui.DISPLAY_ID + "\"}";

            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);
            WebRequest request = WebRequest.Create("https://energ.mobilewkr.com/api/workorders/add");

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

            string retValue;
            using (System.Net.WebResponse resp = request.GetResponse())
            {
                if (resp == null) return null;

                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    retValue = sr.ReadToEnd().Trim();
                }
            }
            WorkOrderObject woo = JsonConvert.DeserializeObject<WorkOrderObject>(retValue);
            mi = new MoreInfo();
            mi.bronzeInfo = retValue;
            return woo;
        }

        public static Success addForm(LoginObject lo, WorkOrderObject woo, int iformID, string formName, ref MoreInfo mi)
        {
            //LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            string formdId = Convert.ToString(iformID); ;                        //FormID  this is a unique incrementing number
            string formTitle = formName;      //from name must match to form name in Bronze
            string workOrderId = woo.result.workOrderId;
            //string workOrderId = "5487116ce4b0c3f1dc1c615f";

            //string bodyData = "{\"formId\" : \"" + formdId + "\", ";
            //bodyData = bodyData + "\"formTitle\" : \"" + formTitle + "\", ";
            //bodyData = bodyData + "\"workOrderId\" : \"" + workOrderId + "\"}";

            string bodyData = "{\"formId\" : \"" + iformID + "\", ";
            bodyData = bodyData + "\"formTitle\" : \"" + formTitle + "\", ";
            bodyData = bodyData + "\"workOrderId\" : \"" + workOrderId + "\"}";

            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);
            WebRequest request = WebRequest.Create("https://energ.mobilewkr.com/api/workorders/form/add");

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

            string retValue;
            using (System.Net.WebResponse resp = request.GetResponse())
            {
                if (resp == null) return null;

                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    retValue = sr.ReadToEnd().Trim();
                }

            }
            Success addFromResult = JsonConvert.DeserializeObject<Success>(retValue);
            mi.bronzeInfo = retValue;
            return addFromResult;
        }

        public static Success DeleteWorkOrder(LoginObject lo, string workOrderID, ref MoreInfo mi)
        {
            //LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            string bodyData = "{\"workOrderId\" : \"" + workOrderID + "\" } ";
            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);
            WebRequest request = WebRequest.Create("https://energ.mobilewkr.com/api/workorders/trash");

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

            string retValue;
            using (System.Net.WebResponse resp = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    retValue = sr.ReadToEnd().Trim();
                }
            }
            mi.bronzeInfo = retValue;
            Success dwo = JsonConvert.DeserializeObject<Success>(retValue);
            return dwo;
        }

        public static ListWorkOrderObject GetAllWorkOrders(LoginObject lo, string engineer, DateTime fromDate, DateTime toDate, string jobID, string displayID)
        {
            MoreInfo mi = new MoreInfo();
            //LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            string bodyData = "{\"dateWindowStart\" : \"" + fromDate.ToString("yyyy-MM-dd") + "\", ";
            if (engineer != "All")
            {
                bodyData = bodyData + "\"userAssignedTo\" : \"" + engineer + "\", ";
            }
            if (jobID.Length > 0)
            {
                bodyData = bodyData + "\"idCustomerAlias2\" : \"" + jobID + "\", ";
            }
            if (displayID.Length > 0)
            {
                bodyData = bodyData + "\"idCustomerAlias3\" : \"" + displayID + "\", ";
            }
            bodyData = bodyData + "\"dateWindowEnd\" : \"" + toDate.ToString("yyyy-MM-dd") + "\"}";
            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);

            WebRequest request = WebRequest.Create("https://energ.mobilewkr.com/api/workorders/list");

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

            string retValue;
            using (System.Net.WebResponse resp = request.GetResponse())
            {
                if (resp == null) return null;


                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    retValue = sr.ReadToEnd().Trim();
                }
            }
            ListWorkOrderObject lwoo = JsonConvert.DeserializeObject<ListWorkOrderObject>(retValue);
            return lwoo;
        }
    }
}
