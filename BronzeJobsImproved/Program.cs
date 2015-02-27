using System.Data;
using BronzeDBLibrary;
using System;
using BronzeLibrary;
using System.Timers;
using EnergLibrary;

namespace BronzeJobsImproved
{
    class Program
    {
        public static LoginObject lo = new LoginObject();

        static void Main(string[] args)
        {
            MoreInfo mi = new MoreInfo();
            lo = BronzeLibrary.APICalls.Login(ref mi);
            Console.WriteLine(" *** Bronze Login *** guid {0}", lo.auth_key);

            Timer t = new Timer(30000);
            t.Start();
            t.Elapsed +=new ElapsedEventHandler(t_Elapsed);

            Console.ReadLine();
            t.Stop();
        }

        static void loginTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MoreInfo mi = new MoreInfo();
            LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            Console.WriteLine(" *** Bronze Login *** guid {0} {1}", lo.auth_key, mi.bronzeInfo);
            go();
        }

        private static void go()
        {
            Console.WriteLine(" ***** PROCESSING *******}");
            // Get all jobs today and tomorrow using date_visit
            DataTable dtVisits = DBStuff.GetAllDistinctJobVisits();
            //DataTable dtVisits = DBStuff.GetAllDistinctJobVisits(491237);
            // Get the lastest record for that id using date_changed

            if (dtVisits.Rows.Count > 0)
            {
                Console.WriteLine("{0} Jobs to check", dtVisits.Rows.Count);
                foreach (DataRow dr in dtVisits.Rows)
                {
                    int jobid = Convert.ToInt32(dr["job_ID"]);                    
                    DataTable dtjobRecord = DBStuff.GetNewestJobID(jobid);
                    int visit_id = Convert.ToInt32(dtjobRecord.Rows[0]["visit_ID"]);
                    if (dtjobRecord.Rows.Count == 1)
                    {
                        int status_to = Convert.ToInt32(dtjobRecord.Rows[0]["status_to"]);
                        if (status_to == 11)   // this is a cancelled job
                        {                        
                            bool exists = DBStuff.IsInBronze(jobid);
                            if (exists)
                            {
                                Console.WriteLine("\tNeed to cancel job {0}", jobid);
                                string workorderID = DBStuff.GetWorkOrderID(jobid);
                                MoreInfo mi = new MoreInfo();
                                Success delete = APICalls.DeleteWorkOrder(lo, workorderID, ref mi);

                                DBStuff.DeleteFromBronzeTable(jobid);
                                Console.WriteLine("\tJob cancelled {0} {1} {2}", jobid, workorderID, mi.bronzeInfo);
                                
                            }

                            // if status = 11 cancelled
                            // call delete bronze api
                            // if success
                            // delete from bronze table
                            // Now what do we do if it did not delete in theory should not get here   ********************** Mail Info on why etc   ** perhaps make a log
                        }
                        else
                        {
                            //to make this quicked we can look here to see if it exist already , if it does just ignore it 
                            // if status <> 11  
                            // if informed or customer informed
                            // add to bronze
                            // if success
                            // add to bronze table
                            // it did not add        *********************************** Mail Info on why etc   ** perhaps make a log
                            bool confirmed = Convert.ToBoolean(dtjobRecord.Rows[0]["confirmed"]);
                            bool customer_informed = Convert.ToBoolean(dtjobRecord.Rows[0]["customer_informed"]);

                            if ((confirmed) || (customer_informed))
                            {
                                DataTable job = DBStuff.GetJob(jobid);
                                string comment = "";
                                if (job.Rows.Count > 0)
                                {
                                    comment = Convert.ToString(job.Rows[0]["comment"]);
                                }
                                else
                                {
                                    comment = String.Format("Can't find a comment for job id {0} ****", jobid);
                                    Console.WriteLine(String.Format("Can't find a comment for job id {0} ****", jobid));
                                }

                                bool exists = DBStuff.IsInBronze(jobid);
                                if (!exists)
                                {
                                    Console.WriteLine("\tNeed to ADD JOB {0}", jobid);
                                    DateTime scheduled_date = Convert.ToDateTime(dtjobRecord.Rows[0]["date_visit"]);
                                    MoreInfo mi = new MoreInfo();
                                    WorkOrderRef wor = new WorkOrderRef();
                                    wor.ui = new UnitInfo(Convert.ToInt32(dtjobRecord.Rows[0]["display_id"]));
                                    wor.visitID = jobid;
                                    wor.add = DBStuff.GetAddress(wor.ui);
                                    wor.jdeReference = String.Format("JDE{0}", jobid);
                                    wor.comment = comment;

                                    string engineer = Convert.ToString(dtjobRecord.Rows[0]["Name"]);

                                    try
                                    {

                                        WorkOrderObject woo = APICalls.addWorkOrder(lo, engineer, wor, ref mi, scheduled_date);
                                        if (woo.result != null)   // added to bronze
                                        {
                                            Console.WriteLine("\tAdded Work order {0}", mi.bronzeInfo);
                                            mi = new MoreInfo();
                                            int formID = DBStuff.GetNextID();
                                            Success s1 = APICalls.addForm(lo, woo, formID, "Work Statement", ref mi);

                                            if (s1 != null)
                                            {
                                                Console.WriteLine("\t\tAdded form Work Statement ID {0} {1}", formID, mi.bronzeInfo);
                                                DBStuff.AddBronzeRecord(jobid, visit_id, formID, "Work Statement", woo.result.workOrderId);
                                                int tripno = DBStuff.GetTripNo(jobid);
                                                if (tripno == 257)
                                                {
                                                    formID = DBStuff.GetNextID();
                                                    Success ss = APICalls.addForm(lo, woo, formID, "Service Record MAN Mercedes", ref mi);
                                                    if (ss != null)
                                                    {
                                                        Console.WriteLine("\t\tAdded form Service Record MAN Mercedes ID {0} {1}", formID, mi.bronzeInfo);
                                                        DBStuff.AddBronzeRecord(jobid, visit_id, formID, "Service Record MAN Mercedes", woo.result.workOrderId);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Failed to add Work order {0}", mi.bronzeInfo);
                                            //relogin
                                            Console.WriteLine(" *** Bronze Login ***");
                                            MoreInfo mii = new MoreInfo();
                                            lo = BronzeLibrary.APICalls.Login(ref mii);
                                            Console.WriteLine(" *** Bronze Login *** guid {0}", lo.auth_key);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error {0} ", ex.Message);
                                    }
                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        //I'm unpset we have retrieved unique but it isn't
                    }
                }
            }
            Console.WriteLine("**** END Processing ***");
        }

        static void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            go();
        }
    }
}
