using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Data;
using BronzeLibrary;
using Newtonsoft.Json;
using BronzeDBLibrary;

namespace BronzeAddJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            LoginObject lo = BronzeLibrary.APICalls.Login("https://energ.mobilewkr.com/api/login", "ENERG_ADMIN", "mobileWkr101");
            if (lo.success != null)
            {
                Global.BRONZELOGIN = lo;
            }
            else
            {
                throw new ApplicationException("Could not login");
            }

            Timer t = new Timer(60000);
            t.Start();
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            Console.ReadLine();
        }

        static void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("*************** ADD JOBS ******************");
            List<Jobs> notInBronze = new List<Jobs>();
            DataTable jobvisits = DBStuff.GetVisits(DateTime.Today);
            foreach (DataRow dr in jobvisits.Rows)
            {
                Jobs aJob = new Jobs();
                aJob.jobid = Convert.ToInt32(dr["job_ID"]);
                aJob.visitid = Convert.ToInt32(dr["Visit_ID"]);
                aJob.unitid = Convert.ToInt32(dr["Unit_ID"]);
                aJob.displayid = DBStuff.GetDisplayID(aJob.unitid);
                aJob.trip_no = DBStuff.GetTripNo(aJob.jobid);
                aJob.datein = Convert.ToDateTime(dr["Date_Visit"]);
                aJob.comment = Convert.ToString(dr["comment"]);
                aJob.engInit = Convert.ToString(dr["Eng_Visit"]);
                bool isInBronze = DBStuff.IsInBronze(aJob.jobid);
                if (isInBronze)
                {
                }
                else
                {
                    int cid = DBStuff.GetCompanyGroup(aJob.engInit);
                    if (cid == 1)    //Only do ECPL employees for tablet jobs
                    {
                        notInBronze.Add(aJob);
                    }
                }
            }

            Console.WriteLine("{0} Jobs to add", notInBronze.Count);
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
                WorkOrderObject woo = BronzeLibrary.APICalls.addWorkOrder(Global.BRONZELOGIN, engineer, wor, ref mi, j.datein);
                if (woo.success != null)
                {
                    mi = new MoreInfo();
                    int formID = DBStuff.GetNextID();
                    if (formID > 0)
                    {
                        try
                        {
                            Success s1 = BronzeLibrary.APICalls.addForm(Global.BRONZELOGIN, woo, formID, "Work Statement", ref mi);
                            string test = mi.bronzeInfo.ToLower();
                            if (test.Contains("success"))
                            {
                                j.added = mi.bronzeInfo;
                                DBStuff.AddBronzeRecord(jobid, visitid, formID, "Work Statement", woo.result.workOrderId);
                                Console.WriteLine("Added Work Statement to JOB: {0}  VISIT: {1}  FORM {2} INFO {3} DATE {4}", jobid, visitid, formID, mi.bronzeInfo, j.datein);
                                if (trip_no == 257)  // also need a service form
                                {
                                    formID = DBStuff.GetNextID();
                                    Success ss = BronzeLibrary.APICalls.addForm(Global.BRONZELOGIN, woo, formID, "Service Record MAN Mercedes", ref mi);
                                    DBStuff.AddBronzeRecord(jobid, visitid, formID, "Service Record MAN Mercedes", woo.result.workOrderId);
                                    Console.WriteLine("Added Service Record MAN Mercedes to JOB: {0}  VISIT: {1}  FORM {2} INFO {3} DATE {4}", jobid, visitid, formID, mi.bronzeInfo, j.datein);
                                }
                            }
                            else
                            {
                                j.added = mi.bronzeInfo;
                                Console.WriteLine("ERROR *** JOB: {0}  VISIT: {1}  FORM {2} INFO {3}", jobid, visitid, formID, mi.bronzeInfo);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR *** JOB: {0}  VISIT: {1}  FORM {2} INFO {3}", jobid, visitid, formID, ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("******* Failed to get formID  ********* DB could be down");
                    }
                }
                else
                {
                    j.added = mi.bronzeInfo;
                    Console.WriteLine("ERROR *** JOB: {0}  VISIT: {1} INFO {2}", jobid, visitid, mi.bronzeInfo);
                }
            }
            Console.WriteLine("*************** END JOBS ******************");
        }
    }
}
