using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergLibrary;
using System.Data.SqlClient;

namespace TomTomLibrary
{
    public class APICalls
    {
        public static void AddOrder(UnitInfo ui, int jid, string comment, string eng_name, DateTime visitDate, ref string info)
        {
            TomTomOrdersService.AuthenticationParameters ap = new TomTomOrdersService.AuthenticationParameters();
            ap.accountName = "combined";
            ap.password = "C0mb1ned!";
            ap.userName = "connect";

            TomTomOrdersService.GeneralParameters gp = new TomTomOrdersService.GeneralParameters();
            gp.locale = TomTomOrdersService.KnownLocales.UK;
            gp.timeZone = TomTomOrdersService.KnownTimeZones.Europe_London;

            TomTomOrdersService.ObjectIdentityParameter oip = new TomTomOrdersService.ObjectIdentityParameter();
            oip.objectNo = GetObjectNo(eng_name);
            oip.objectUid = GetObjectUID(eng_name);

            TomTomOrdersService.ordersClient oc = new TomTomOrdersService.ordersClient();

            TomTomOrdersService.DestinationOrderParameter dop = new TomTomOrdersService.DestinationOrderParameter();
            dop.addrNoToUseAsDestination = ui.DISPLAY_ID.ToString();
            dop.orderNo = jid.ToString();
            dop.orderText = string.Format("Unit {0} {1} {2}", ui.DISPLAY_ID, ui.NAME, comment);
            dop.scheduledCompletionDateAndTime = visitDate;
            dop.scheduledCompletionDateAndTimeSpecified = true;
            dop.notificationEnabled = true;

            TomTomOrdersService.GenericServiceWriteOpResult gswor = oc.sendDestinationOrder(ap, gp, dop, oip, null);
            info = gswor.statusMessage;
        }

        public static void ShowOrderReport()
        {
            TomTomOrdersService.AuthenticationParameters ap = new TomTomOrdersService.AuthenticationParameters();
            ap.accountName = "combined";
            ap.password = "C0mb1ned!";
            ap.userName = "connect";

            TomTomOrdersService.GeneralParameters gp = new TomTomOrdersService.GeneralParameters();
            gp.locale = TomTomOrdersService.KnownLocales.UK;
            gp.timeZone = TomTomOrdersService.KnownTimeZones.Europe_London;

            TomTomOrdersService.ordersClient oc = new TomTomOrdersService.ordersClient();
            TomTomOrdersService.OrderReportParameters orp = new TomTomOrdersService.OrderReportParameters();
            orp.orderNo = "494123";

            TomTomOrdersService.GenericServiceQueryOpResult gsqor = oc.showOrderReport(ap, gp, orp);
            TomTomOrdersService.TransferObjectBase[] tob = gsqor.results;

            List<TomTomOrdersService.ReportedOrderData> lrod = new List<TomTomOrdersService.ReportedOrderData>();
            foreach (TomTomOrdersService.TransferObjectBase TOB in tob)
            {
                TomTomOrdersService.ReportedOrderData rod = (TomTomOrdersService.ReportedOrderData)TOB;
                lrod.Add(rod);
            }
        }

        public static void DeleteOrder(int visitid, ref string info)
        {
            TomTomOrdersService.AuthenticationParameters ap = new TomTomOrdersService.AuthenticationParameters();
            ap.accountName = "combined";
            ap.password = "C0mb1ned!";
            ap.userName = "connect";

            TomTomOrdersService.GeneralParameters gp = new TomTomOrdersService.GeneralParameters();
            gp.locale = TomTomOrdersService.KnownLocales.UK;
            gp.timeZone = TomTomOrdersService.KnownTimeZones.Europe_London;


            TomTomOrdersService.DeleteOrderParameter dop = new TomTomOrdersService.DeleteOrderParameter();
            dop.markDeleted = true;
            dop.markDeletedSpecified = true;
            dop.orderNo = Convert.ToString(visitid);

            TomTomOrdersService.ordersClient oc = new TomTomOrdersService.ordersClient();
            TomTomOrdersService.GenericServiceWriteOpResult gswor = oc.deleteOrder(ap, gp, dop);

            info = gswor.statusMessage;
        }

        private static string GetObjectNo(string eng_name)
        {
            string name = "";
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select objectNo
  from tomtom
 where name = @NAME
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@NAME", eng_name);
                conn.Open();
                try
                {
                    name = Convert.ToString(cmd.ExecuteScalar());
                }
                catch
                {
                }
            }
            return name;
        }

        private static string GetObjectUID(string eng_name)
        {
            string name = "";
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select objectUID
  from tomtom
 where name = @NAME
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@NAME", eng_name);
                conn.Open();
                try
                {
                    name = Convert.ToString(cmd.ExecuteScalar());
                }
                catch
                {
                }
            }
            return name;
        }
    }
}
