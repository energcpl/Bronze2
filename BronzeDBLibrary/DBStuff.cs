using System;
using System.Data.SqlClient;
using EnergLibrary;
using System.Data;
using BronzeLibrary;

namespace BronzeDBLibrary
{
    public static class DBStuff
    {
        public static string GetDisplayID(int unit_id)
        {
            string did = "";
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select display_id 
  from units
 where unit_id = @UID
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@UID", unit_id);
                conn.Open();
                did = Convert.ToString(cmd.ExecuteScalar());
            }
            return did;
        }

        public static int GetTripNo(int jobID)
        {
            //1st try jobs then try history
            int tripno = -1;
            DataTable dt = GetFromJobs(jobID);
            if (dt.Rows.Count > 0)
            {
                tripno = Convert.ToInt32(dt.Rows[0]["trip_no"]);
            }
            else
            {
                dt = GetFromJobHistory(jobID);
                if (dt.Rows.Count > 0)
                {
                    tripno = Convert.ToInt32(dt.Rows[0]["trip_no"]);
                }
            }
            return tripno;
        }

        public static DataTable GetJob(int jobid)
        {
            DataTable dt = DBStuff.GetFromJobs(jobid);
            if (dt.Rows.Count > 0) return dt;

            DataTable dth = DBStuff.GetFromJobHistory(jobid);
            return dth;
        }

        private static DataTable GetFromJobHistory(int jobid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT     Job_History.Job_ID, Job_Visits.Visit_ID, Job_History.Unit_Id, Units.Display_ID, Job_History.Trip_No, Job_History.Date_In, Job_History.Comment, 
                      job_history.engineer, job_visits.Date_visit
FROM         Job_History INNER JOIN
                      Job_Visits ON Job_History.Job_ID = Job_Visits.Job_ID INNER JOIN
                      Units ON Job_History.Unit_Id = Units.Unit_ID
where Job_History.Job_ID = @JID
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@JID", jobid);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                return dt;
            }
        }

        private static DataTable GetFromJobs(int jobid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT     Jobs.Job_ID, Job_Visits.Visit_ID, Jobs.Unit_Id, Units.Display_ID, Jobs.Trip_No, Jobs.Date_In, Jobs.Comment, Job_Visits.Eng_Visit, job_visits.Date_visit
FROM         Jobs INNER JOIN
                      Units ON Jobs.Unit_Id = Units.Unit_ID INNER JOIN
                      Job_Visits ON Jobs.Job_ID = Job_Visits.Job_ID
where Jobs.Job_ID = @JID
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@JID", jobid);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                return dt;
            }
        }

        public static bool IsInBronze(int jobID)
        {
            bool FOUND = false;
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                conn.Open();
                string dbquery = @"
select 1 from bronze
 where job_id = @JID
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@JID", jobID);
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read()) FOUND = true;
            }
            return FOUND;
        }

        public static int GetNextID()
        {
            int id = -1;
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select max(bronze_form_ID)
  from bronze
";
                conn.Open();
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                id = Convert.ToInt32(cmd.ExecuteScalar());
                if (id > 0)    // returen the next ID
                {
                    id++;
                }
            }
            return id;
        }

        public static string GetBronzeLogin(string engineerInitials)
        {
            string name = "";
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select Name
  from engineers
 where engineer = @ENG 
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@ENG", engineerInitials);
                conn.Open();
                name = Convert.ToString(cmd.ExecuteScalar());
            }
            return name;
        }

        public static Address GetAddress(UnitInfo unitInfo)
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

        public static void AddBronzeRecord(int jobid, int visitid, int formID, string formName, string workOrderID)
        {
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
insert into bronze
       (visit_id, job_id, bronze_form_id, bronze_from_name, workorderID)
values (@VID    , @JID  ,      @BFID    ,     @BFN        , @WOID)
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@VID", visitid);
                cmd.Parameters.AddWithValue("@JID", jobid);
                cmd.Parameters.AddWithValue("@BFID", formID);
                cmd.Parameters.AddWithValue("@BFN", formName);
                cmd.Parameters.AddWithValue("@WOID", workOrderID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static DataTable GetVisits(DateTime dateTime)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT     Visit_ID, Job_ID, Unit_ID, Status_From, Status_To, Date_Changed, Date_Visit, Eng_Visit, Visit_Scheduled, Comment, CommentID, LoginName, Confirmed, PoNumber, 
                      Customer_Informed, CurrentStatus, JVT_ID
FROM         Job_Visits
WHERE     ((Date_Visit >= @DV) AND (Status_to <> 11) and (Confirmed = 1) OR
            (Date_Visit >= @DV) AND (Status_to <> 11) and (Customer_informed = 1))
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@DV", dateTime);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            return dt;
        }

        public static int GetCompanyGroup(string enginit)
        {
            int companyGroup = -1;
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
Select group_id
  from engineers
 where engineer = @ENG
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@ENG", enginit);
                conn.Open();
                companyGroup = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return companyGroup;
        }

        public static DataTable GetAllDistinctJobVisits()
        {
            //Only do ECPL employees.
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT     Job_Visits.Job_ID
FROM         Job_Visits INNER JOIN
                      Engineers ON Job_Visits.Eng_Visit = Engineers.Engineer
WHERE     (Date_Visit >= @TODAY) AND (Job_ID > 0)
  AND       (Engineers.Group_ID = 1)
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@TODAY", DateTime.Today);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetAllDistinctJobVisits(string engineer)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT   DISTINCT  Job_Visits.Job_ID
FROM         Job_Visits INNER JOIN
                      Engineers ON Job_Visits.Eng_Visit = Engineers.Engineer
WHERE     (Date_Visit >= @TODAY) AND (Job_ID > 0)
  AND     (Eng_Visit = @ENG)
  AND       (Engineers.Group_ID = 1)
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@TODAY", DateTime.Today);
                cmd.Parameters.AddWithValue("@ENG", engineer);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetAllDistinctJobVisits(int jobid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT DISTINCT Job_ID
FROM         Job_Visits
WHERE      (Job_ID = @JID)
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@JID", jobid);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetAllDistinctJobVisits(DateTime visitDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT DISTINCT Job_ID
FROM         Job_Visits INNER JOIN
                      Engineers ON Job_Visits.Eng_Visit = Engineers.Engineer
WHERE      (date_visit = @DVISIT)
  AND       (Engineers.Group_ID = 1)
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@DVISIT", visitDate);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetNewestJobID(int jobid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT top 1 Job_Visits.Visit_ID, Job_Visits.Job_ID, Job_Visits.Unit_ID, Job_Visits.Status_To, Job_Visits.Date_Visit, Job_Visits.Eng_Visit, Job_Visits.Confirmed, 
                      Job_Visits.Customer_Informed, Engineers.Name, Units.Display_ID
FROM         Job_Visits INNER JOIN
                      Engineers ON Job_Visits.Eng_Visit = Engineers.Engineer INNER JOIN
                      Units ON Job_Visits.Unit_ID = Units.Unit_ID
 where job_id = @JID
 order by Date_Changed DESC
";
                conn.Open();
                SqlCommand cmd = new SqlCommand(dbquery,conn);
                cmd.Parameters.AddWithValue("@JID", jobid);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetOldestJobID(int jobid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT top 1 Job_Visits.Visit_ID, Job_Visits.Job_ID, Job_Visits.Unit_ID, Job_Visits.Status_To, Job_Visits.Date_Visit, Job_Visits.Eng_Visit, Job_Visits.Confirmed, 
                      Job_Visits.Customer_Informed, Engineers.Name, Units.Display_ID
FROM         Job_Visits INNER JOIN
                      Engineers ON Job_Visits.Eng_Visit = Engineers.Engineer INNER JOIN
                      Units ON Job_Visits.Unit_ID = Units.Unit_ID
 where job_id = @JID
 order by Date_Changed ASC
";
                conn.Open();
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@JID", jobid);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            return dt;
        }

        public static string GetWorkOrderID(int jobID)
        {
            string workOrderID = null;
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select workorderID
  from bronze
 where  job_id = @JID
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@JID", jobID);
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    workOrderID = Convert.ToString(sdr[0]);
                }
                conn.Close();
            }
            return workOrderID;
        }

        public static void DeleteFromBronzeTable(int jobid)
        {
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
update bronze
    set job_id = -1, visit_id = -1
where job_id = @JID
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@JID", jobid);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static DataTable GetAllJobRecords(int jobid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
SELECT Job_Visits.Visit_ID, Job_Visits.Job_ID, Job_Visits.Unit_ID, Job_Visits.Status_To, Job_Visits.Date_Visit, Job_Visits.Eng_Visit, Job_Visits.Confirmed, 
                      Job_Visits.Customer_Informed, Engineers.Name, Units.Display_ID
FROM         Job_Visits INNER JOIN
                      Engineers ON Job_Visits.Eng_Visit = Engineers.Engineer INNER JOIN
                      Units ON Job_Visits.Unit_ID = Units.Unit_ID
 where job_id = @JID
 order by Date_Changed
";
                conn.Open();
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@JID", jobid);
                sda.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetBronzeEngineers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select Name
  from engineers
 where group_id = 1
   and rate > 0
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            DataRow dr = dt.NewRow();
            dr["Name"] = "All";
            dt.Rows.InsertAt(dr,0);
            return dt;
        }

        public static string GetInitialsFromName(string name)
        {
            string Initials = "";
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select engineer
  from engineers
 where name = @NAME
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@NAME", name);
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    Initials = Convert.ToString(sdr[0]);
                }
            }
            return Initials;
        }

        public static string GetNameFromUserAssignedTo(string user)
        {
            string Name = "";
            using (SqlConnection conn = new SqlConnection(DBConnections.EnergDB))
            {
                string dbquery = @"
select name
  from tomtom
 where userAssignedTo = @USER
";
                SqlCommand cmd = new SqlCommand(dbquery, conn);
                cmd.Parameters.AddWithValue("@USER", user);
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    Name = Convert.ToString(sdr[0]);
                }
            }
            return Name;
        }

        public static bool RequiredAdding(int jobid)
        {
            bool required = false;
            DataTable dt = GetAllJobRecords(jobid);
            foreach (DataRow dr in dt.Rows)
            {
                bool confirmed = Convert.ToBoolean(dr["confirmed"]);
                bool inform = Convert.ToBoolean(dr["customer_informed"]);
                if ((confirmed) || (inform)) required = true;
            }
            return required;
        }
    }        
}
