using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class Securuty : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Security", User.Identity.Name);
            
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
                ButtonAddUser.Enabled = false;
                ButtonDeleteUser.Enabled = false;
                ButtonAddUser1.Enabled = false;
            }
            else
            {
                GridView1.Enabled = true;
                ButtonAddUser.Enabled = true;
                ButtonDeleteUser.Enabled = true;
                ButtonAddUser1.Enabled = true;
            }
            if (IsPostBack)
                return;
            TextBoxUserName.Text = "";
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
            {
                Response.Redirect("security.aspx");
                return;
            }
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.Parent.Parent;
            int idx = row.RowIndex;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand cmd = new SqlCommand("update securityscreenaccess set securityaccess=" + ddl.Text + " where UserID=(select userid from users where username='" + DropDownListUsers.Text + "') and screenname='" + GridView1.Rows[idx].Cells[0].Text + "'", connection);
            cmd.ExecuteNonQuery();

            connection.Close();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }

        protected void GridView1_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
            DropDownListUsers.DataBind();
            DropDownListUsers.Text = User.Identity.Name;
            int idx = 0;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            DropDownList ddl = new DropDownList();
            foreach (GridViewRow row in GridView1.Rows)
            {
                ddl = (DropDownList)row.FindControl("DropDownList1");
                SqlCommand cmd = new SqlCommand("select securityaccess from securityscreenaccess where UserID=(select userid from users where username='" + DropDownListUsers.Text + "') and screenname='" + GridView1.Rows[idx].Cells[0].Text + "'", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    ddl.Text = reader["securityaccess"].ToString();
                }

                idx++;
                reader.Close();
            }
            connection.Close();
        }

        protected void DropDownListUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            int idx = 0;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            DropDownList ddl = new DropDownList();
            foreach (GridViewRow row in GridView1.Rows)
            {
                ddl = (DropDownList)row.FindControl("DropDownList1");
                SqlCommand cmd = new SqlCommand("select securityaccess from securityscreenaccess where UserID=(select userid from users where username='" + DropDownListUsers.Text + "') and screenname='" + GridView1.Rows[idx].Cells[0].Text + "'", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    ddl.Text = reader["securityaccess"].ToString();
                }

                idx++;
                reader.Close();
            }
            connection.Close();
        }

        protected void ButtonDeleteUser_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            Raptor msg = new Raptor();
            if (DropDownListUsers.Text == "Operator" || DropDownListUsers.Text == "Raptor")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("This user is reserved, and cannot be deleted."));
                return;
            }
            if (DropDownListUsers.Text == User.Identity.Name)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("You cannot delete this user because it is currently logged in."));
                return;
            }

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand cmd1 = new SqlCommand("delete from securityscreenaccess where UserID=(select userid from users where username='" + DropDownListUsers.Text + "')", connection);
            cmd1.ExecuteNonQuery();
            SqlCommand cmd = new SqlCommand("delete from users where username='" + DropDownListUsers.Text + "'", connection);
            cmd.ExecuteNonQuery();

            connection.Close();
            DropDownListUsers.DataBind();
        }

        protected void ButtonAddUser_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            Raptor msg = new Raptor();
            if (TextBoxUserName.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("User Name cannot be blank."));
                return;
            }
            for (int i = 0; i < DropDownListUsers.Items.Count; i++)
            {
                if (DropDownListUsers.Items[i].Text == TextBoxUserName.Text)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("User Name already exists."));
                    return;
                }
            }
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand cmd1 = new SqlCommand("insert into users select max(userid)+1,'" + TextBoxUserName.Text + "','" + TextBoxPassword.Text + "' from users", connection);
            cmd1.ExecuteNonQuery();
            SqlCommand cmd = new SqlCommand("insert into securityscreenaccess select screenname,max(users.userid),0 from users,securityscreenaccess where securityscreenaccess.userid=0 group by screenname", connection);
            cmd.ExecuteNonQuery();

            connection.Close();
            DropDownListUsers.DataBind();
        }

        protected void ButtonAddUser1_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            Raptor msg = new Raptor();
            if (DropDownListUsers.Text == "Operator")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Operator Login does not require a password."));
                return;
            }

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand cmd = new SqlCommand("select password from users where username='" + DropDownListUsers.Text + "'", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                if (reader["password"].ToString() != TextBoxOldPassword.Text)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Old Password is incorrect."));
                    return;
                }
            }

            reader.Close();

            SqlCommand cmd1 = new SqlCommand("update users set password = '" + TextBoxNewPassword.Text + "' where username='" + DropDownListUsers.Text + "'", connection);
            cmd1.ExecuteNonQuery();
            connection.Close();
        }
    }
}