using System;
using BronzeDBLibrary;
using System.Data;
using BronzeLibrary;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

namespace BronzeWebUtils
{
    public partial class TryAgain : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private void Login()
        {
            MoreInfo mi = new MoreInfo();
            LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            if (lo.success != null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Bronze login guid = {0}", lo.auth_key));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format("ErrorPage.aspx?error={0}", "Failed to login"));
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            go();
        }

        private void go()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("**************** Processing {0:f} ****************\n", DateTime.Now));
            
            DataTable dt = DBStuff.GetAllDistinctJobVisits(DateTime.Today);
            //DataTable dt = DBStuff.GetAllDistinctJobVisits(494920);
            
            foreach (DataRow dr in dt.Rows)
            {
                
                int jobid = Convert.ToInt32(dr["job_id"]);
                DataTable finaldt = DBStuff.GetNewestJobID(jobid);
                int status_to = Convert.ToInt32(finaldt.Rows[0]["status_to"]);
                DateTime ScheduledDate = Convert.ToDateTime(finaldt.Rows[0]["Date_Visit"]);

                DataTable earlydt = DBStuff.GetOldestJobID(jobid);
                DateTime StartDate = Convert.ToDateTime(earlydt.Rows[0]["Date_Visit"]);
                if (status_to == 11)
                {
                    if (DBStuff.IsInBronze(jobid))
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Am I supposed to delete"));
                    }

                }
                else
                {
                    MoreInfo mi = new MoreInfo();
                    LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
                    ListWorkOrderObject lwoo = APICalls.GetAllWorkOrders(lo, "All", StartDate, ScheduledDate, jobid.ToString(), "");
                    if (lwoo.success == null)    // the dates are the wrong way around 
                    {
                        lwoo = APICalls.GetAllWorkOrders(lo, "All", DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(1), jobid.ToString(), "");
                    }
                    if (lwoo.results.Count > 0)
                    {
                        //Exist in bronze already.
                        string user = lwoo.results[0].userAssignedTo;
                        string name = DBStuff.GetNameFromUserAssignedTo(user);
                        if (name.Length == 0)
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("Can't find user {0}", name));
                        }
                        else
                        {
                            string init = DBStuff.GetInitialsFromName(name);
                            long bDate = Convert.ToInt64(lwoo.results[0].dateScheduled);
                            DateTime inBronzeDate = new DateTime(1970, 01, 01).AddMilliseconds(bDate);

                            string engShouldbeName = Convert.ToString(finaldt.Rows[0]["Name"]);
                            if (engShouldbeName == name)    // the record in bronze has the same engineer as planner
                            {
                                // no need to do anything
                            }
                            else
                            {
                                //ok engineer has been moved
                                //delete then add
                                string info = "";
                                System.Diagnostics.Debug.WriteLine(String.Format("Delete {0}", jobid));
                                TomTomLibrary.APICalls.DeleteOrder(jobid, ref info);
                                System.Diagnostics.Debug.WriteLine(String.Format("\tTOMTOM delete {0}", info));
                                string wor = DBStuff.GetWorkOrderID(jobid);
                                MoreInfo mii = new MoreInfo();
                                BronzeLibrary.APICalls.DeleteWorkOrder(lo, wor, ref mii);
                                BronzeDBLibrary.DBStuff.DeleteFromBronzeTable(jobid);
                                System.Diagnostics.Debug.WriteLine(String.Format("\tBronze Delete {0}", mi.bronzeInfo));

                                System.Diagnostics.Debug.WriteLine(String.Format("Adding Job {0}", jobid));
                                AddAJob(jobid);
                                
                            }
                            if (inBronzeDate != ScheduledDate)
                            {
                                //job has beed moved 
                                //throw new ApplicationException("TODO");
                            }
                        }
                    }
                    else
                    {
                        //record is not in bronze so add
                        //only add the job if one record has confirmed or informed customer set
                        bool requiresAdding = DBStuff.RequiredAdding(jobid);
                        if (requiresAdding)
                        {
                            AddAJob(jobid);
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("**************** Finished {0:f} ******************\n", DateTime.Now);
        }

        private void AddAJob(int jobid)
        {
            DataTable last = DBStuff.GetNewestJobID(jobid);
            int display_ID = Convert.ToInt32(last.Rows[0]["display_id"]);
            string engineer = Convert.ToString(last.Rows[0]["Name"]);
            DateTime ScheduledDate = Convert.ToDateTime(last.Rows[0]["Date_Visit"]);

            WorkOrderRef wor = new WorkOrderRef();
            wor.jdeReference = String.Format("JDE{0}", jobid);
            wor.ui = new EnergLibrary.UnitInfo(display_ID);
            wor.add = DBStuff.GetAddress(wor.ui);
            wor.visitID = jobid;
            DataTable dtjc = DBStuff.GetJob(jobid);
            string comment = "";
            if (dtjc.Rows.Count > 0)
            {
                comment = Convert.ToString(dtjc.Rows[0]["comment"]);
            }

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
                        System.Diagnostics.Debug.WriteLine(String.Format("{0}", "add work statement SUCCESS !!!! \n" + mi.bronzeInfo));
                        DBStuff.AddBronzeRecord(jobid, wor.visitID, formID, "Work Statement", woo.result.workOrderId);
                        int trip_no = DBStuff.GetTripNo(jobid);
                        if (trip_no == 257)  // also need a service form
                        {
                            formID = DBStuff.GetNextID();
                            Success ss = BronzeLibrary.APICalls.addForm(lo,  woo, formID, "Service Record MAN Mercedes", ref mi);
                            DBStuff.AddBronzeRecord(jobid, wor.visitID, formID, "Service Record MAN Mercedes", woo.result.workOrderId);
                            System.Diagnostics.Debug.WriteLine(String.Format("{0}", "add Service Record MAN Mercedes statement SUCCESS !!!! \n" + mi.bronzeInfo));
                        }                      
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Failed tp add {0}", mi.bronzeInfo));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("{0}", "Failed to get a formID"));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format("{0}\n", mi.bronzeInfo));
            }

            //now add to tomtom
            string info = "";
            TomTomLibrary.APICalls.AddOrder(wor.ui, jobid, wor.comment, engineer, ScheduledDate, ref info);
            System.Diagnostics.Debug.WriteLine(String.Format("Add to TOMTOM {0}\n", info));
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnTimer_Click(object sender, EventArgs e)
        {
            Login();

            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 30000;
            t.Start();

            t.Elapsed += new ElapsedEventHandler(t_Elapsed);

            System.Timers.Timer loginTimer = new System.Timers.Timer();
            loginTimer.Interval = 300000;
            loginTimer.Start();
            loginTimer.Elapsed += new ElapsedEventHandler(loginTimer_Elapsed);
        }

        void loginTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Login();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            go();
        }
    }
}