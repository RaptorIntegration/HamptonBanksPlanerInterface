using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class SawMileage : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Saw Mileage", User.Identity.Name);
            
            Label LabelScreenStatus = (Label)Master.FindControl("LabelScreenStatus");
            if (CurrentUser.Access == 0)
                LabelScreenStatus.Text = "READ ONLY";
            else if (CurrentUser.Access == 1)
                LabelScreenStatus.Text = "";
            else if (CurrentUser.Access == 2)
            {
                LabelScreenStatus.Text = "ACCESS DENIED";
                Response.Redirect("websort.aspx");
            }
            if (CurrentUser.Access != 1)
            {
                GridView1.Enabled = false;
            }
            else
            {
                GridView1.Enabled = true;
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ResetMileage")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                // Retrieve the row that contains the button clicked
                // by the user from the Rows collection.
                GridViewRow row = GridView1.Rows[index];

                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd = new SqlCommand("update sawmileage set mileage = 0 where id=" + (index).ToString(), connection);
                cmd.ExecuteNonQuery();
                SqlCommand cmd1 = new SqlCommand("execute UpdateStatusDataWEBSort 482,0", connection);
                cmd1.ExecuteNonQuery();
                connection.Close();
                GridView1.DataBind();
                UpdatePanel3.Update();
            }
            if (e.CommandName == "ResetStrokes")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                // Retrieve the row that contains the button clicked
                // by the user from the Rows collection.
                GridViewRow row = GridView1.Rows[index];

                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd = new SqlCommand("update sawmileage set strokes = 0 where id=" + (index).ToString(), connection);
                cmd.ExecuteNonQuery();
                SqlCommand cmd1 = new SqlCommand("execute UpdateStatusDataWEBSort 483,0", connection);
                cmd1.ExecuteNonQuery();
                connection.Close();
                GridView1.DataBind();
                UpdatePanel3.Update();
            }
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate))
            {
                if (Convert.ToInt32(e.Row.Cells[3].Text.ToString()) >= Convert.ToInt32(e.Row.Cells[4].Text))
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
                if (Convert.ToInt32(e.Row.Cells[5].Text) >= Convert.ToInt32(e.Row.Cells[6].Text))
                    e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //see if we should reset any alarms based on whether theshold is now greater than actual
            //saw mileage
            if (Convert.ToInt32(e.NewValues[0]) > Convert.ToInt32(GridView1.Rows[e.RowIndex].Cells[3].Text))
            {
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd1 = new SqlCommand("execute UpdateStatusDataWEBSort 482,0", connection);
                cmd1.ExecuteNonQuery();
                connection.Close();
            }
            //strokes
            if (Convert.ToInt32(e.NewValues[1]) > Convert.ToInt32(GridView1.Rows[e.RowIndex].Cells[5].Text))
            {
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd1 = new SqlCommand("execute UpdateStatusDataWEBSort 483,0", connection);
                cmd1.ExecuteNonQuery();
                connection.Close();
            }
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            Timer2.Enabled = true;
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = true;
        }

        protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            Timer2.Enabled = true;
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = true;
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Timer2.Enabled = false;
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Timer2.Enabled = false;
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = false;
        }
    }
}