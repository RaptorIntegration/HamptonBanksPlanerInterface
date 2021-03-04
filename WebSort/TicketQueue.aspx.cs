using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace WebSort
{
    public partial class ticketqueue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
            SqlConnection connection = new SqlConnection(Global.ConnectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from RaptorTicketSettings", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            CheckBoxAutoIncrement.Checked = Convert.ToBoolean(reader["Auto"]);

            reader.Close();
            connection.Close();
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            Timer2.Interval = 30000;
            GridViewPreviousData.DataBind();
        }

        protected void GridViewPreviousData_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Timer2.Enabled = false;
        }

        protected void GridViewPreviousData_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            //Timer2.Interval = 1;
            Timer2.Enabled = true;
        }

        protected void GridViewPreviousData_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int index = e.RowIndex;
            GridViewRow row = GridViewPreviousData.Rows[index];

            SqlConnection connection = new SqlConnection(Global.ConnectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd0 = new SqlCommand("UPDATE ProductionPackages SET TicketPrinted = 0 WHERE PackageNumber = " + row.Cells[1].Text + " UPDATE ProductionPackagesPrevious SET TicketPrinted = 0 WHERE PackageNumber = " + row.Cells[1].Text, connection);
            cmd0.ExecuteNonQuery();
            connection.Close();
        }

        protected void GridViewPreviousData_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            Timer2.Enabled = true;
        }

        protected void CheckBoxAutoIncrement_CheckedChanged(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(Global.ConnectionString);
            // Open the connection.
            connection.Open();
            if (CheckBoxAutoIncrement.Checked == true)
            {
                SqlCommand command = new SqlCommand("update RaptorTicketSettings set Auto=1", connection);
                command.ExecuteNonQuery();
            }
            else
            {
                SqlCommand command = new SqlCommand("update RaptorTicketSettings set Auto=0", connection);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        protected void GridViewPreviousData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Print")
            {
                SqlConnection connection = new SqlConnection(Global.ConnectionString);
                // Open the connection.
                connection.Open();
                int index = Convert.ToInt32(e.CommandArgument);
                SqlCommand command = new SqlCommand("exec selectTicket " + GridViewPreviousData.Rows[index].Cells[2].Text, connection);
                command.ExecuteNonQuery();

                connection.Close();
                GridViewPreviousData.DataBind();
                UpdatePanel5.Update();
            }
            if (e.CommandName == "Remove")
            {
                SqlConnection connection = new SqlConnection(Global.ConnectionString);
                // Open the connection.
                connection.Open();
                int index = Convert.ToInt32(e.CommandArgument);
                SqlCommand command = new SqlCommand("update ProductionPackages set TicketPrinted=1,Printed=1 where packagenumber= " + GridViewPreviousData.Rows[index].Cells[2].Text, connection);
                command.ExecuteNonQuery();
                SqlCommand command1 = new SqlCommand("update ProductionPackagesPrevious set TicketPrinted=1,Printed=1 where packagenumber= " + GridViewPreviousData.Rows[index].Cells[2].Text, connection);
                command1.ExecuteNonQuery();

                connection.Close();
                GridViewPreviousData.DataBind();
                UpdatePanel5.Update();
            }
        }
    }
}