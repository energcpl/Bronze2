using System;
using System.Web.UI.WebControls;
using System.Data;
using BronzeLibrary;
using BronzeDBLibrary;
using EnergLibrary;

namespace BronzeWebUtils
{
    public partial class SendAJob : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            int jobid = -1;
            try
            {
                jobid = Convert.ToInt32(txtJobId.Text);
                DataTable dt = DBStuff.GetJob(jobid);
                if (dt.Rows.Count > 0)
                {
                    BindGrid(dt);
                }
                else
                {
                    lblError.Text = String.Format("Cant find jobid {0}", jobid);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private void BindGrid(DataTable dt)
        {
            gvJobs.DataSource = dt;
            gvJobs.DataBind();
        }

        protected void gvJobs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            
            int jobid = Convert.ToInt32(gvJobs.Rows[index].Cells[1].Text);
            int visitid = Convert.ToInt32(gvJobs.Rows[index].Cells[2].Text);
            string engineerInitials = Convert.ToString(gvJobs.Rows[index].Cells[8].Text);
            int display_ID = Convert.ToInt32(gvJobs.Rows[index].Cells[4].Text);
            string engineer = DBStuff.GetBronzeLogin(engineerInitials);
            int trip_no = Convert.ToInt32(gvJobs.Rows[index].Cells[5].Text);
            DateTime ScheduledDate = Convert.ToDateTime(gvJobs.Rows[index].Cells[9].Text);

            WorkOrderRef wor = new WorkOrderRef();
            wor.jdeReference = String.Format("JDE{0}", jobid);
            wor.ui = new EnergLibrary.UnitInfo(display_ID);
            wor.add = DBStuff.GetAddress(wor.ui);
            DataTable jobs = DBStuff.GetJob(jobid);
            wor.comment = Convert.ToString(jobs.Rows[0]["comment"]);
            wor.visitID = jobid;

            bool alreadyIn = DBStuff.IsInBronze(jobid);
            if (alreadyIn)
            {
                Response.Redirect(String.Format("ErrorPage.aspx?error={0}", "This job already exists in the table"));
            }
            else
            {
                //WorkOrderObject woo = new WorkOrderObject();
                MoreInfo mi = new MoreInfo();
                LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
                WorkOrderObject woo = BronzeLibrary.APICalls.addWorkOrder(lo, engineer, wor, ref mi, ScheduledDate);
                if (woo.success != null)
                {
                    mi = new MoreInfo();
                    int formID = DBStuff.GetNextID();
                    if (formID > 0)
                    {
                        Success s1 = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Work Statement", ref mi);
                        string test = mi.bronzeInfo.ToLower();
                        if (test.Contains("success"))
                        {
                            DBStuff.AddBronzeRecord(jobid, visitid, formID, "Work Statement", woo.result.workOrderId);
                            if (trip_no == 257)  // also need a service form
                            {
                                formID = DBStuff.GetNextID();
                                Success ss = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Service Record MAN Mercedes", ref mi);
                                DBStuff.AddBronzeRecord(jobid, visitid, formID, "Service Record MAN Mercedes", woo.result.workOrderId);
                            }
                            Response.Redirect(String.Format("ErrorPage.aspx?error={0}", "SUCCESS !!!! " + mi.bronzeInfo));
                        }
                        else
                        {
                            Response.Redirect(String.Format("ErrorPage.aspx?error={0}", mi.bronzeInfo));
                        }
                    }
                    else
                    {
                        Response.Redirect(String.Format("ErrorPage.aspx?error={0}", "Failed to get a formID"));
                    }
                    //Success s2 = addForm(Global.BRONZELOGIN, woo, 156, "Transfer Note");
                    //Success s3 = addForm(Global.BRONZELOGIN, woo, 156, "Service Record MAN Mercedes");
                }
                else
                {
                    Response.Redirect(String.Format("ErrorPage.aspx?error={0}", mi.bronzeInfo));
                }
            }

            //TOM TOM Stuff
            string info = "";
            TomTomLibrary.APICalls.DeleteOrder(jobid, ref info);
        }     

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnDelTomTom_Click(object sender, EventArgs e)
        {
            int jobid = Convert.ToInt32(txtJobId.Text);
            string info = "";
            TomTomLibrary.APICalls.DeleteOrder(jobid, ref info);
            Response.Redirect(String.Format("ErrorPage.aspx?error={0}", info));
        }

        protected void btnSendTomTom_Click(object sender, EventArgs e)
        {
            int jobid = Convert.ToInt32(txtJobId.Text);
            DataTable dtjobinfo = DBStuff.GetNewestJobID(jobid);

            if (dtjobinfo.Rows.Count == 1)
            {
                DataRow dr = dtjobinfo.Rows[0];
                int display_id = Convert.ToInt32(dr["display_id"]);
                string name = Convert.ToString(dr["name"]);
                DateTime visitDate = Convert.ToDateTime(dr["Date_Visit"]);

                DataTable jobcomment = DBStuff.GetJob(jobid);
                string comment = "";
                if (jobcomment.Rows.Count > 0)
                {
                    DataRow jobrow = jobcomment.Rows[0];
                    comment = Convert.ToString(jobrow["comment"]);
                }

                UnitInfo ui = new UnitInfo(display_id);

                string info = "";
                TomTomLibrary.APICalls.AddOrder(ui, jobid, comment, name, visitDate, ref info);
                Response.Redirect(String.Format("ErrorPage.aspx?error={0}", info));
            }
        }

    }
}