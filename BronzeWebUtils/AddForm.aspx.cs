using System;
using BronzeLibrary;
using BronzeDBLibrary;

namespace BronzeWebUtils
{
    public partial class AddForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            string bronzeGuid = txtID.Text;
            WorkOrderObject woo = new WorkOrderObject();
            woo.result = new WorkOrderID();

            woo.result.workOrderId = bronzeGuid;
            int formID = DBStuff.GetNextID();

            MoreInfo mi = new MoreInfo();

            LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            Success s1 = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Work Statement", ref mi);
            string test = mi.bronzeInfo.ToLower();
            if (test.Contains("success"))
            {
                DBStuff.AddBronzeRecord(Convert.ToInt32(txt_jobid.Text), Convert.ToInt32(txt_visitid.Text), formID, "Work Statement", woo.result.workOrderId);
                Response.Redirect(String.Format("ErrorPage.aspx?error={0}", "SUCCESS !!!! " + mi.bronzeInfo));
            }
            else
            {
                Response.Redirect(String.Format("ErrorPage.aspx?error={0}", mi.bronzeInfo));
            }
        }
    }
}