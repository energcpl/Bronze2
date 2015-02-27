using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using EnergLibrary;
using System.Data.SqlClient;
using BronzeLibrary;

namespace Bronze2
{
    class Program
    {
        static void Main(string[] args)
        {
            UnitInfo ui = new UnitInfo(301117);

            String apiUser = "ENERG_ADMIN";
            String apiPassword = "mobileWkr101";

            string loginResponse = Login("http://energ.mobilewkr.com/api/login", apiUser, apiPassword);
            LoginObject lo = JsonConvert.DeserializeObject<LoginObject>(loginResponse);

            string engineer = "ENERG_ENGINEER";
            //Success delSuccess = DeleteWorkOrder(lo, "5315b19ae4b0b6419c7963e2");

            WorkOrderRef wor = new WorkOrderRef();
            wor.jdeReference = "JDE112AA";
            wor.visitID = 115;
            wor.ui = new UnitInfo(301117);
            wor.add = GetAddress(wor.ui);

            WorkOrderObject woo = addWorkOrder(lo, engineer, wor);
            Success s1 = addForm(lo, woo, 115, "Work Statement");

            //Success s2 = addForm(lo, woo, 116, "Transfer Note");
            //ListWorkOrderObject lwoo = GetAllWorkOrders(lo, "KIM.HOLYHEAD" , new DateTime(2014,2,12));
        }

        private static Address GetAddress(UnitInfo unitInfo)
        {
            Address theAddress = new Address();
            theAddress.FullAddress = "None";
            theAddress.ContactName = "None";
            theAddress.Phone = "None";
            theAddress.PostCode = "None";

            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT     Unit_ID, Address_Type, ContactName, Line_1, Line_2, Line_3, Line_4, Line_5, Line_6, Line_7, Postcode, PhoneNumber, FaxNumber
FROM         Addresses
WHERE      unit_id = @UID
  AND      address_type = 2
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@UID", unitInfo.UNIT_ID);
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    string contactname = Convert.ToString(sdr["ContactName"]);
                    if (contactname.Length > 0) theAddress.ContactName = Convert.ToString(sdr["ContactName"]);

                    string phone = Convert.ToString(sdr["PhoneNumber"]);
                    if (phone.Length > 0) theAddress.Phone = phone;

                    string postCode = Convert.ToString(sdr["Postcode"]);
                    if (postCode.Length > 0) theAddress.PostCode = postCode;

                    string fullAddress = Convert.ToString(sdr["Line_1"]) + "," + Convert.ToString(sdr["Line_2"]) + Convert.ToString(sdr["Line_3"]) + "," + Convert.ToString(sdr["Line_4"]) + Convert.ToString(sdr["Line_5"]) + "," + Convert.ToString(sdr["Line_6"]) + "," + Convert.ToString(sdr["Line_7"]);
                    theAddress.FullAddress = fullAddress;
                }

            }
            return theAddress;
        }

        private static Success DeleteWorkOrder(LoginObject lo, string workOrderID)
        {
            string bodyData = "{\"workOrderId\" : \"" + workOrderID + "\" } ";
            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);
            WebRequest request = WebRequest.Create("http://energ.mobilewkr.com/api/workorders/trash");

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
            Success dwo = JsonConvert.DeserializeObject<Success>(retValue);
            return dwo;
        }

        private static ListWorkOrderObject GetAllWorkOrders(LoginObject lo, string userAssignedTo, DateTime fromDate)
        {
            string bodyData = "{\"userAssignedTo\" : \"" + userAssignedTo + "\", ";
            bodyData = bodyData + "\"dateWindowStart\" : \"" + fromDate.ToString("yyyy-MM-dd") + "\", ";
            bodyData = bodyData + "\"dateWindowEnd\" : \"" + fromDate.ToString("yyyy-MM-dd") + "\"}";
            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);

            WebRequest request = WebRequest.Create("http://energ.mobilewkr.com/api/workorders/list");

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

        private static WorkOrderObject addWorkOrder(LoginObject lo, string engineer, WorkOrderRef wor)
        {
            DateTime fromDate = DateTime.Today;
            string bodyData = "{\"userAssignedTo\" : \"" + engineer + "\", ";
            bodyData = bodyData + "\"dateScheduled\" : \"" + fromDate.ToString("yyyy-MM-dd") + "\", ";
            bodyData = bodyData + "\"dateScheduledEnd\" : \"" + fromDate.ToString("yyyy-MM-dd") + "\", ";
            bodyData = bodyData + "\"idCustomerAlias\" : \"" + wor.jdeReference + "\", ";
            bodyData = bodyData + "\"idCustomerAlias2\" : \"" + wor.visitID + "\", ";
            bodyData = bodyData + "\"customerName\" : \"" + wor.ui.NAME + "\", ";
            bodyData = bodyData + "\"sitePostCode\" : \"" + wor.add.PostCode + "\", ";
            bodyData = bodyData + "\"siteAddress\" : \"" + wor.add.FullAddress + "\", ";
            bodyData = bodyData + "\"idCustomerAlias3\" : \"" + wor.ui.DISPLAY_ID + "\"}";

            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);
            WebRequest request = WebRequest.Create("http://energ.mobilewkr.com/api/workorders/add");

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
            return woo;
        }

        private static Success addForm(LoginObject lo, WorkOrderObject woo, int iformID, string formName )
        {
            string formdId = Convert.ToString(iformID); ;                        //FormID  this is a unique incrementing number
            string formTitle = formName;      //from name must match to form name in Bronze
            string workOrderId = woo.result.workOrderId;  //

            string bodyData = "{\"formId\" : \"" + formdId + "\", ";
            bodyData = bodyData + "\"formTitle\" : \"" + formTitle + "\", ";
            bodyData = bodyData + "\"workOrderId\" : \"" + workOrderId + "\"}";

            byte[] bodyByteArray = Encoding.UTF8.GetBytes(bodyData);
            WebRequest request = WebRequest.Create("http://energ.mobilewkr.com/api/workorders/form/add");

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
            return addFromResult;
        }

        public static string Login(String loginUrl, String username, String password)
        {
            String result = "";
            String strPost = "username=" + username + "&password=" + password;
            StreamWriter myWriter = null;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
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
                return result;

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed Authenticating" + e.Message);
                return null;
            }
        }
    }
}
