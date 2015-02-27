using System;

namespace BronzeWebUtils
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string error = Request.QueryString["error"];
            txtError.Text = error;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("SendAJob.aspx");
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }
}