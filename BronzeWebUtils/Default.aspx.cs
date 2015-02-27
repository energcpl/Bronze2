using System;
using BronzeLibrary;

namespace BronzeWebUtils
{
    public partial class Default : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            Login();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Login();

        }

        private void Login()
        {
            MoreInfo mi = new MoreInfo();
            LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            if (lo.success != null)
            {
                lblLoggedIn.Text = String.Format("Sucessfull login key {0}", lo.auth_key);
            }
            else
            {
                Response.Redirect(String.Format("ErrorPage.aspx?error={0}", "Failed to login"));
            }
        }

        protected void btnWork_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("WorkOrders.aspx"));
        }

        protected void btnJob_Click(object sender, EventArgs e)
        {
            //if (Global.BRONZELOGIN != null)
            //{
                Response.Redirect(String.Format("SendAJob.aspx"));
            //}
        }

        protected void btnPDF_Click(object sender, EventArgs e)
        {
           
              Response.Redirect(@"P:\CustomerReports\2014\09\582_2014_09_30.xlsx");
          //  Response.Redirect(@"P:\CustomerReports\2014\09\91_2014_09_30.pdf");

            //string path = Server.MapPath(@"P:\CustomerReports\2014\09\91_2014_09_30.pdf");
            //WebClient client = new WebClient();
            //Byte[] buffer = client.DownloadData(path);

            //if (buffer != null)
            //{

            //    Response.ContentType = "application/pdf";
            //    Response.AddHeader("content-length", buffer.Length.ToString());
            //    Response.BinaryWrite(buffer);
            //}
        }

        protected void btnNotInBronze_Click(object sender, EventArgs e)
        {
            Response.Redirect("NotInBronze.aspx");
        }

        protected void btnAddForm_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddForm.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("TryAgain.aspx");
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Response.Redirect("Export.aspx");
        }

        protected void btnAddFrom_Click(object sender, EventArgs e)
        {
            Response.Redirect("CheckForms.aspx");
        }


    }
}