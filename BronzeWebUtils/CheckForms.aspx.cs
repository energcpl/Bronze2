using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BronzeLibrary;
using BronzeDBLibrary;

namespace BronzeWebUtils
{
    public partial class CheckForms : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            MoreInfo mi = new MoreInfo();
            LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            ListWorkOrderObject lwoo = APICalls.GetAllWorkOrders(lo, "All", new DateTime(2015, 2, 16), new DateTime(2015, 2, 16), txtJobID.Text, "");

            List<ListWorkOrder> listwoo = new List<ListWorkOrder>();
            listwoo = lwoo.results;

            foreach (ListWorkOrder wo in listwoo)
            {

                int jobid = Convert.ToInt32(wo.idCustomerAlias2);
                if (DBStuff.IsInBronze(jobid))
                {
                }
                else
                {
                    WorkOrderObject woo = new WorkOrderObject();
                    woo.result = new WorkOrderID();
                    woo.result.workOrderId = wo.id;
                    int formID = DBStuff.GetNextID();

                    Success s1 = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Work Statement", ref mi);
                    string test = mi.bronzeInfo.ToLower();
                    if (test.Contains("success"))
                    {
                        DBStuff.AddBronzeRecord(jobid, jobid, formID, "Work Statement", woo.result.workOrderId);
                    }

                    int trip_no = DBStuff.GetTripNo(jobid);
                    if (trip_no == 257)  // also need a service form
                    {
                        formID = DBStuff.GetNextID();
                        Success ss = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Service Record MAN Mercedes", ref mi);
                        DBStuff.AddBronzeRecord(jobid, jobid, formID, "Service Record MAN Mercedes", woo.result.workOrderId);
                        Console.WriteLine(String.Format("{0} - {1}", "add Service Record MAN Mercedes statement SUCCESS !!!!", jobid, mi.bronzeInfo));
                    }
                }

            }
        }
    }
}