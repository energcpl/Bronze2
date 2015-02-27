using System;
using BronzeLibrary;
using System.Data;
using BronzeDBLibrary;

namespace TomTomCall
{
    class Program
    {
        public static LoginObject lo = new LoginObject();

        static void Main(string[] args)
        {
            Login();
#if DEBUG
            CheckJobs();
#else
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 30000;
            t.Start();
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);

            System.Timers.Timer loginTimer = new System.Timers.Timer();
            loginTimer.Interval = 3600000;
            loginTimer.Start();
            loginTimer.Elapsed += new System.Timers.ElapsedEventHandler(loginTimer_Elapsed);
#endif

            Console.ReadLine();
        }

        static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckJobs();
        }

        private static void CheckJobs()
        {
            Console.WriteLine(String.Format("**************** Processing {0:F} ****************\n", DateTime.Now));

#if DEBUG
            Console.Write("Enter job number ..");
            int jobnum = Convert.ToInt32(Console.ReadLine());
            DataTable dt = DBStuff.GetAllDistinctJobVisits(jobnum);
#else
            DataTable dt = DBStuff.GetAllDistinctJobVisits(DateTime.Today);
#endif            
            Console.WriteLine(String.Format("{0} Jobs to process\n", dt.Rows.Count));

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
                        string info = "";
                        TomTomLibrary.APICalls.DeleteOrder(jobid, ref info);
                        Console.WriteLine(string.Format("Delete {0} from tom tom {0}", jobid, info));

                        if (BronzeDBLibrary.DBStuff.IsInBronze(jobid))
                        {                            
                            string wor = BronzeDBLibrary.DBStuff.GetWorkOrderID(jobid);
                            MoreInfo mi = new MoreInfo();
                            Success ss = BronzeLibrary.APICalls.DeleteWorkOrder(lo, wor, ref mi);
                            Console.WriteLine(string.Format("Delete {0} from Bronze {0}", jobid, mi.bronzeInfo));
                            BronzeDBLibrary.DBStuff.DeleteFromBronzeTable(jobid);
                        }
                    }

                }
                else
                {

                    ListWorkOrderObject lwoo = APICalls.GetAllWorkOrders(lo, "All", StartDate.AddDays(-1), ScheduledDate.AddDays(1), jobid.ToString(), "");
                    if (lwoo.results.Count == 0)    // the dates are the wrong way around 
                    {
                        try
                        {
                            lwoo = APICalls.GetAllWorkOrders(lo, "All", DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(1), jobid.ToString(), "");
                        }
                        catch
                        {
                            Console.WriteLine("GetAllWorkOrders failed\n");
                        }
                    }
                    try
                    {
                        if (lwoo.results != null)
                        {
                            if (lwoo.results.Count > 0)
                            {
                                //Exist in bronze already.
                                string user = lwoo.results[0].userAssignedTo;
                                string name = DBStuff.GetNameFromUserAssignedTo(user);
                                if (name.Length == 0)
                                {
                                    Console.WriteLine(String.Format("Can't find user {0} {1} {2}", Convert.ToString(finaldt.Rows[0]["Name"]), user, jobid));
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
                                        Console.WriteLine(String.Format("Delete {0}", jobid));
                                        TomTomLibrary.APICalls.DeleteOrder(jobid, ref info);
                                        Console.WriteLine(String.Format("\tTOMTOM delete {0}", info));
                                        string wor = DBStuff.GetWorkOrderID(jobid);
                                        MoreInfo mi = new MoreInfo();
                                        BronzeLibrary.APICalls.DeleteWorkOrder(lo, wor, ref mi);
                                        BronzeDBLibrary.DBStuff.DeleteFromBronzeTable(jobid);
                                        Console.WriteLine(String.Format("\tBronze Delete {0}", mi.bronzeInfo));

                                        Console.WriteLine(String.Format("Adding Job {0}", jobid));
                                        AddAJob(jobid);

                                    }
                                    if (inBronzeDate != ScheduledDate)
                                    {
                                        string info = "";
                                        Console.WriteLine(String.Format("Delete {0}", jobid));
                                        TomTomLibrary.APICalls.DeleteOrder(jobid, ref info);
                                        Console.WriteLine(String.Format("\tTOMTOM delete {0}", info));
                                        string wor = DBStuff.GetWorkOrderID(jobid);
                                        MoreInfo mi = new MoreInfo();
                                        BronzeLibrary.APICalls.DeleteWorkOrder(lo, wor, ref mi);
                                        BronzeDBLibrary.DBStuff.DeleteFromBronzeTable(jobid);
                                        Console.WriteLine(String.Format("\tBronze Delete {0}", mi.bronzeInfo));

                                        Console.WriteLine(String.Format("Adding Job {0}", jobid));
                                        AddAJob(jobid);
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
                        else
                        {
                            Console.WriteLine("Loo is null !!!! {0}", jobid);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Still having a problem {0} {1}", ex.Message, jobid);
                    }
                }
            }
            Console.WriteLine("**************** Finished {0:F} ******************\n", DateTime.Now);
        }

        private static void AddAJob(int jobid)
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
            wor.comment = comment;

            //WorkOrderObject woo = new WorkOrderObject();
            MoreInfo mi = new MoreInfo();
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
                        Console.WriteLine(String.Format("{0} - {1}", "add work statement SUCCESS !!!!", jobid, mi.bronzeInfo));
                        DBStuff.AddBronzeRecord(jobid, wor.visitID, formID, "Work Statement", woo.result.workOrderId);
                        int trip_no = DBStuff.GetTripNo(jobid);
                        if (trip_no == 257)  // also need a service form
                        {
                            formID = DBStuff.GetNextID();
                            Success ss = BronzeLibrary.APICalls.addForm(lo, woo, formID, "Service Record MAN Mercedes", ref mi);
                            DBStuff.AddBronzeRecord(jobid, wor.visitID, formID, "Service Record MAN Mercedes", woo.result.workOrderId);
                            Console.WriteLine(String.Format("{0} - {1}", "add Service Record MAN Mercedes statement SUCCESS !!!!", jobid, mi.bronzeInfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Failed to add {0} -{1}", jobid, mi.bronzeInfo));
                    }
                }
                else
                {
                    Console.WriteLine(String.Format("{0}", "Failed to get a formID", jobid));
                }
            }
            else
            {
                Console.WriteLine(String.Format("{0}\n", mi.bronzeInfo));
            }

            //now add to tomtom
            string info = "";
            TomTomLibrary.APICalls.AddOrder(wor.ui, jobid, wor.comment, engineer, ScheduledDate, ref info);
            Console.WriteLine(String.Format("Add to TOMTOM {0}\n", info));
        }

        static void loginTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Login();
        }

        private static void Login()
        {
            MoreInfo mi = new MoreInfo();
            lo = BronzeLibrary.APICalls.Login(ref mi);
            if (lo.success != null)
            {
                Console.WriteLine(String.Format("Bronze login guid = {0}", lo.auth_key));
            }
            else
            {
                Console.WriteLine(String.Format("{0}", "Failed to login"));
            }
        }
    }
}
