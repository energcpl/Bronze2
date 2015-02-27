using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data;
using BronzeLibrary;
using BronzeDBLibrary;

namespace BronzeWebUtils
{
    public partial class NotInBronze : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            List<Jobs> notInBronze = new List<Jobs>();
            DataTable jobvisits = DBStuff.GetAllDistinctJobVisits(Calendar1.SelectedDate);
            foreach (DataRow dr in jobvisits.Rows)
            {
                int jobid = Convert.ToInt32(dr["job_id"]);
                DataTable jobsdt = DBStuff.GetAllJobRecords(jobid);
                
                bool toTablet = false;
                bool cancel = false;
                foreach (DataRow jobsrow in jobsdt.Rows)
                {
                    int statusto = Convert.ToInt32(jobsrow["status_to"]);
                    if (statusto == 11) cancel = true;
                    if (statusto != 11)
                    {
                        cancel = false;
                        bool confirmed = Convert.ToBoolean(jobsrow["confirmed"]);
                        bool customer_informed = Convert.ToBoolean(jobsrow["customer_informed"]);
                        if (confirmed || customer_informed)
                        {
                            toTablet = true;
                        }
                    }
                }
                if (cancel)
                {
                    bool inBronze = DBStuff.IsInBronze(jobid);
                    if (inBronze)
                    {
                        //could delete stuff here
                    }
                }
                else
                {
                    if (toTablet)
                    {
                        bool inBronze = DBStuff.IsInBronze(jobid);
                        if (!inBronze)
                        {
                            DataTable dt = DBStuff.GetNewestJobID(jobid);
                            DataRow drb = dt.Rows[0];
                            Jobs aJob = new Jobs();
                            aJob.jobid = Convert.ToInt32(drb["job_ID"]);
                            aJob.visitid = Convert.ToInt32(drb["Visit_ID"]);
                            aJob.unitid = Convert.ToInt32(drb["Unit_ID"]);
                            aJob.displayid = DBStuff.GetDisplayID(aJob.unitid);
                            aJob.trip_no = DBStuff.GetTripNo(aJob.jobid);
                            DataTable jobtable = DBStuff.GetJob(jobid);
                            aJob.datein = Convert.ToDateTime(drb["Date_Visit"]);
                            aJob.comment = Convert.ToString(jobtable.Rows[0]["comment"]);
                            aJob.engInit = Convert.ToString(drb["Eng_Visit"]);
                            notInBronze.Add(aJob);
                        }
                    }
                }
            }
            gvJobs.DataSource = notInBronze;
            gvJobs.DataBind();

            gvResult.DataSource = null;
            gvResult.DataBind();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnSendAll_Click(object sender, EventArgs e)
        {
            List<Jobs> notInBronze = new List<Jobs>();
            DataTable jobvisits = DBStuff.GetAllDistinctJobVisits(Calendar1.SelectedDate);
            foreach (DataRow dr in jobvisits.Rows)
            {
                int jobid = Convert.ToInt32(dr["job_id"]);
                DataTable jobsdt = DBStuff.GetAllJobRecords(jobid);

                bool toTablet = false;
                bool cancel = false;
                foreach (DataRow jobsrow in jobsdt.Rows)
                {
                    int statusto = Convert.ToInt32(jobsrow["status_to"]);
                    if (statusto == 11) cancel = true;
                    if (statusto != 11)
                    {
                        cancel = false;
                        bool confirmed = Convert.ToBoolean(jobsrow["confirmed"]);
                        bool customer_informed = Convert.ToBoolean(jobsrow["customer_informed"]);
                        if (confirmed || customer_informed)
                        {
                            toTablet = true;
                        }
                    }
                }
                if (cancel)
                {
                    bool inBronze = DBStuff.IsInBronze(jobid);
                    if (inBronze)
                    {
                        //could delete stuff here
                    }
                }
                else
                {
                    if (toTablet)
                    {
                        bool inBronze = DBStuff.IsInBronze(jobid);
                        if (!inBronze)
                        {
                            DataTable dt = DBStuff.GetNewestJobID(jobid);
                            DataRow drb = dt.Rows[0];
                            Jobs aJob = new Jobs();
                            aJob.jobid = Convert.ToInt32(drb["job_ID"]);
                            aJob.visitid = Convert.ToInt32(drb["Visit_ID"]);
                            aJob.unitid = Convert.ToInt32(drb["Unit_ID"]);
                            aJob.displayid = DBStuff.GetDisplayID(aJob.unitid);
                            aJob.trip_no = DBStuff.GetTripNo(aJob.jobid);
                            DataTable jobtable = DBStuff.GetJob(jobid);
                            aJob.datein = Convert.ToDateTime(drb["Date_Visit"]);
                            aJob.comment = Convert.ToString(jobtable.Rows[0]["comment"]);
                            aJob.engInit = Convert.ToString(drb["Eng_Visit"]);
                            notInBronze.Add(aJob);
                        }
                    }
                }
            }

            foreach (Jobs j in notInBronze)
            {
                int jobid = j.jobid;
                int visitid = j.visitid;
                string engineerInitials = j.engInit;
                int display_ID = Convert.ToInt32(j.displayid);
                string engineer = DBStuff.GetBronzeLogin(engineerInitials);
                int trip_no = j.trip_no;

                WorkOrderRef wor = new WorkOrderRef();
                wor.jdeReference = String.Format("JDE{0}", jobid);
                wor.ui = new EnergLibrary.UnitInfo(display_ID);
                wor.add = DBStuff.GetAddress(wor.ui);
                wor.visitID = jobid;

                MoreInfo mi = new MoreInfo();
                LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
                WorkOrderObject woo = BronzeLibrary.APICalls.addWorkOrder(lo,  engineer, wor, ref mi, j.datein);
                if (woo.success != null)
                {
                    mi = new MoreInfo();
                    int formID = DBStuff.GetNextID();
                    if (formID > 0)
                    {
                        try
                        {
                            Success s1 = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Work Statement", ref mi);
                            string test = mi.bronzeInfo.ToLower();
                            if (test.Contains("success"))
                            {
                                j.added = mi.bronzeInfo;
                                DBStuff.AddBronzeRecord(jobid, visitid, formID, "Work Statement", woo.result.workOrderId);
                                if (trip_no == 257)  // also need a service form
                                {
                                    formID = DBStuff.GetNextID();
                                    Success ss = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Service Record MAN Mercedes", ref mi);
                                    DBStuff.AddBronzeRecord(jobid, visitid, formID, "Work Statement", woo.result.workOrderId);
                                }
                            }
                            else if (test.Contains("form already in use"))
                            {
                                formID = DBStuff.GetNextID() + 1;  //just try the next form one more time
                                Success s2 = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Work Statement", ref mi);
                                string test2 = mi.bronzeInfo.ToLower();
                                if (test2.Contains("success"))
                                {
                                    j.added = mi.bronzeInfo;
                                    DBStuff.AddBronzeRecord(jobid, visitid, formID, "Work Statement", woo.result.workOrderId);
                                    if (trip_no == 257)  // also need a service form
                                    {
                                        formID = DBStuff.GetNextID();
                                        Success ss = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Service Record MAN Mercedes", ref mi);
                                        DBStuff.AddBronzeRecord(jobid, visitid, formID, "Work Statement", woo.result.workOrderId);
                                    }
                                }
                            }
                            else
                            {
                                j.added = mi.bronzeInfo;
                            }
                        }
                        catch (Exception ex)
                        {
                            Response.Redirect(String.Format("ErrorPage.aspx?error={0}", ex.Message));
                        }
                    }
                    else
                    {
                        Response.Redirect(String.Format("ErrorPage.aspx?error={0}", "Failed to get a formID"));
                    }
                }
                else
                {
                    j.added = mi.bronzeInfo;
                }
            }

            gvResult.DataSource = notInBronze;
            gvResult.DataBind();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);

            int jobid = Convert.ToInt32(gvJobs.Rows[index].Cells[1].Text);
            int visitid = Convert.ToInt32(gvJobs.Rows[index].Cells[2].Text);
            string engineerInitials = Convert.ToString(gvJobs.Rows[index].Cells[8].Text);
            int display_ID = Convert.ToInt32(gvJobs.Rows[index].Cells[4].Text);
            string engineer = DBStuff.GetBronzeLogin(engineerInitials);
            int trip_no = Convert.ToInt32(gvJobs.Rows[index].Cells[5].Text);
            DateTime ScheduledDate = Convert.ToDateTime(gvJobs.Rows[index].Cells[6].Text);

            WorkOrderRef wor = new WorkOrderRef();
            wor.jdeReference = String.Format("JDE{0}", jobid);
            wor.ui = new EnergLibrary.UnitInfo(display_ID);
            wor.add = DBStuff.GetAddress(wor.ui);
            wor.visitID = jobid;

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
                            DBStuff.AddBronzeRecord(jobid, visitid, formID, "Work Statement", woo.result.workOrderId);
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
            }
            else
            {
                Response.Redirect(String.Format("ErrorPage.aspx?error={0}", mi.bronzeInfo));
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            gvResult.DataSource = null;
            gvResult.DataBind();
        }
    }
}