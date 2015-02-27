using System;
using System.Web.UI.WebControls;
using BronzeLibrary;

namespace BronzeWebUtils
{
    public partial class WorkOrders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillEngineers();
            }
        }

        private void BindGrid()
        {
            MoreInfo mi = new MoreInfo();
            LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            ListWorkOrderObject lwoo = APICalls.GetAllWorkOrders(lo, ddlEng.SelectedValue, calStart.SelectedDate, calEnd.SelectedDate, txtJobID.Text, txtDisplayID.Text);
            gvOrders.DataSource = lwoo.results;
            gvOrders.DataBind();
        }

        private void FillEngineers()
        {
            ddlEng.DataSource = BronzeDBLibrary.DBStuff.GetBronzeEngineers();
            ddlEng.DataBind();
        }

        protected void gvOrders_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int rowIndex = e.RowIndex;
            string guidToDelete = gvOrders.Rows[rowIndex].Cells[1].Text;

            MoreInfo mi = new MoreInfo();
            LoginObject lo = BronzeLibrary.APICalls.Login(ref mi);
            Success deleted = APICalls.DeleteWorkOrder(lo, guidToDelete, ref mi);
            

            gvOrders.EditIndex = -1;

            string engineer = ddlEng.SelectedValue;
            BindGrid();
        }


        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }
}