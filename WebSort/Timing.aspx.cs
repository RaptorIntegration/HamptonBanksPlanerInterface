using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebSort.Model;
using Logix;

namespace WebSort
{
    public partial class Timing : BasePage
    {
        private static User CurrentUser;
        Logix.Controller MyPLC = new Logix.Controller();

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Timing", User.Identity.Name);
            
            Global.GetOnlineSetup();
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
                ButtonInsertNewItem.Enabled = false;
                ButtonInsertNewItem0.Enabled = false;
                ButtonAddNewGroup.Enabled = false;
                ButtonRenameGroup0.Enabled = false;
                ButtonChangeGroupID.Enabled = false;
                ButtonDeleteGroup.Enabled = false;
                ButtonRefreshAll.Enabled = false;
            }
            else
            {
                GridView1.Enabled = true;
                ButtonInsertNewItem.Enabled = true;
                ButtonInsertNewItem0.Enabled = true;
                ButtonAddNewGroup.Enabled = true;
                ButtonRenameGroup0.Enabled = true;
                ButtonChangeGroupID.Enabled = true;
                ButtonDeleteGroup.Enabled = true;
                if (Global.OnlineSetup)
                {
                    ButtonRefreshAll.Enabled = true;
                    Button4.Enabled = true;
                }
                else
                {
                    ButtonRefreshAll.Enabled = false;
                    Button4.Enabled = false;
                }
            }

            if (IsPostBack)
                return;
            GridView1.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");
            string str = "return confirm_delete('" + LabelPLCID.Text + "');";
            ButtonDeleteGroup.Attributes.Clear();
            ButtonDeleteGroup.Attributes.Add("onclick", str);
            if (Global.OnlineSetup)
            {
                //read all timing values from PLC
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                LabelTimeout0.Visible = false;
                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 256", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmdb1 = new SqlCommand("select distinct plcid from timing with(NOLOCK)", connection);
                SqlDataReader readermain = cmdb1.ExecuteReader();
                while (readermain.Read())
                {
                    SqlCommand cmd = new SqlCommand("insert into datarequeststiming select getdate()," + readermain["plcid"].ToString() + ",0,0,0,0,0,0,0,0,0,0,0,0 select id=(select max(id) from datarequeststiming with(NOLOCK))", connection);
                    SqlDataReader reader1 = cmd.ExecuteReader();
                    reader1.Read();
                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequeststiming", int.Parse(reader1["id"].ToString()));
                    reader1.Close();
                    if (!succeeded)
                    {
                        LabelTimeout0.Visible = true;
                        break;
                    }
                }
                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-256 where (datarequests & 256)=256", connection);
                cmd1.ExecuteNonQuery();

                readermain.Close();

                SqlCommand cmd01a = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1048576", connection);
                cmd01a.ExecuteNonQuery();
                SqlCommand cmdb1b = new SqlCommand("select distinct id from [parameters] with(NOLOCK)", connection);
                SqlDataReader readermaina = cmdb1b.ExecuteReader();
                while (readermaina.Read())
                {
                    SqlCommand cmd = new SqlCommand("insert into datarequestsparameters select getdate()," + readermaina["id"].ToString() + ",0,0,0 select id=(select max(id) from datarequestsparameters with(NOLOCK))", connection);

                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequestsparameters", int.Parse(reader["id"].ToString()));
                    reader.Close();

                    if (!succeeded)
                    {
                        Label1.Visible = true;
                        break;
                    }
                }
                SqlCommand cmd1aa = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1048576 where (datarequests & 1048576)=1048576", connection);
                cmd1aa.ExecuteNonQuery();
                readermaina.Close();
                connection.Close();
            }
            //GridView1.DataBind();
        }

        public void LoadTabControl(int gotolasttab)
        {
            int lasttab = 0;
            try
            {
                lasttab = ASPxTabControl1.ActiveTab.Index;
            }
            catch { }
            ASPxTabControl1.Tabs.Clear();
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("Select distinct plcid,categoryname from timing order by plcid", connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                {
                    try
                    {
                        ASPxTabControl1.Tabs.Add(reader["CategoryName"].ToString(), reader["plcid"].ToString());
                        if (ASPxTabControl1.Tabs.Count % 10 == 0)
                            ASPxTabControl1.Tabs[ASPxTabControl1.Tabs.Count - 1].NewLine = true;
                    }
                    catch
                    { }
                }

            reader.Close();
            connection.Close();
            if (gotolasttab == 1)
                ASPxTabControl1.ActiveTab = ASPxTabControl1.Tabs[ASPxTabControl1.Tabs.Count - 1];
            else if (gotolasttab == 2)
                ASPxTabControl1.ActiveTab = ASPxTabControl1.Tabs[lasttab];

            LabelPLCID.Text = "PLC Group ID: " + ASPxTabControl1.ActiveTab.Name;
            SqlDataSource1.SelectCommand = "SELECT [plcID], id,[ItemName], [ItemValue],[PreviousValue],[min],[max] FROM [Timing] WHERE [plcid] =" + LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2) + " order by id";
            GridView1.DataBind();
        }

        protected void ASPxTabControl1_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
            LoadTabControl(0);
        }

        protected void ASPxTabControl1_ActiveTabChanged(object source, DevExpress.Web.ASPxTabControl.TabControlEventArgs e)
        {
            LabelPLCID.Text = "PLC Timing ID: " + ASPxTabControl1.ActiveTab.Name;
            SqlDataSource1.SelectCommand = "SELECT [plcID], id,[ItemName], [ItemValue],[PreviousValue],[min],[max] FROM [Timing] WHERE [plcid] =" + LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2) + " order by id";
            GridView1.DataBind();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int index = e.RowIndex;
            GridViewRow row = GridView1.Rows[index];

            ((Label)GridView1.Rows[index].FindControl("LabelPLCTimeoutProd")).Visible = false;
            //String id = GridView1.Rows[index].Cells[1].Text;
            String id = LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2);

            if (Convert.ToInt32(((TextBox)row.Cells[4].Controls[0]).Text) < Convert.ToInt32(((TextBox)row.Cells[6].Controls[0]).Text))
            {
                ((Label)GridView1.Rows[index].FindControl("LabelTooSmall")).Visible = true;
                e.Cancel = true; // stops the automatic update command from happening
                return;
            }
            else if (Convert.ToInt32(((TextBox)row.Cells[4].Controls[0]).Text) > Convert.ToInt32(((TextBox)row.Cells[7].Controls[0]).Text))
            {
                ((Label)GridView1.Rows[index].FindControl("LabelTooLarge")).Visible = true;
                e.Cancel = true; // stops the automatic update command from happening
                return;
            }
            SqlDataSource1.UpdateParameters["PreviousValue"].DefaultValue = e.OldValues[1].ToString();
            if (Global.OnlineSetup)
            {
                int[] ItemArray = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    row = GridView1.Rows[i];
                    if (i == index)
                        ItemArray[i] = Convert.ToInt32(((TextBox)row.Cells[4].Controls[0]).Text);
                    else
                        ItemArray[i] = Convert.ToInt32(GridView1.Rows[i].Cells[4].Text);
                }

                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 256", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand("insert into datarequeststiming select getdate()," + id.ToString() + "," + ItemArray[0] + "," + ItemArray[1] + "," + ItemArray[2] + "," + ItemArray[3] + "," + ItemArray[4] + "," + ItemArray[5] + "," + ItemArray[6] + "," + ItemArray[7] + "," + ItemArray[8] + "," + ItemArray[9] + ",1,0 select id=(select max(id) from datarequeststiming with(NOLOCK))", connection);

                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequeststiming", int.Parse(reader["id"].ToString()));
                reader.Close();

                if (!succeeded)
                {
                    ((Label)GridView1.Rows[index].FindControl("LabelPLCTimeoutProd")).Visible = true;
                    e.Cancel = true; // stops the automatic update command from happening
                }

                //read the values back in, in case PLC has updated some fields based on others changing
                SqlCommand cmd1 = new SqlCommand("insert into datarequeststiming select getdate()," + id.ToString() + ",0,0,0,0,0,0,0,0,0,0,0,0 select id=(select max(id) from datarequeststiming with(NOLOCK))", connection);
                SqlDataReader reader2 = cmd1.ExecuteReader();
                reader2.Read();
                //make sure message is processed
                succeeded = Raptor.MessageAckConfirm("datarequeststiming", int.Parse(reader2["id"].ToString()));
                reader2.Close();
                if (!succeeded)
                {
                    LabelTimeout0.Visible = true;
                    //break;
                }
                reader2.Close();
                SqlCommand cmd3 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-256 where (datarequests & 256)=256", connection);
                cmd3.ExecuteNonQuery();
                connection.Close();
            }
        }

        protected void ButtonInsertNewItem_Click(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count == 10)
                return;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("insert into timing select " + LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2) + "," + ((GridView1.Rows.Count - 1) + 1).ToString() + ",'" + ASPxTabControl1.ActiveTab.Text + "','{New Item}',0,0,0,0", connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            GridView1.DataBind();
        }

        protected void ButtonAddNewGroup_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("if (select count(*) from timing) > 0 insert into timing select max(plcid)+1,0,'{New Group}','{New Item}',0,0,0,0 from timing else insert into timing select 0,0,'{New Group}','{New Item}',0,0,0,0", connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            LoadTabControl(1);
        }

        protected void ButtonDeleteGroup_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("delete from timing where plcid=" + LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2), connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            LoadTabControl(0);
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            LabelOldName.Visible = false;
            LabelOldName1.Visible = false;
            ButtonSave.Visible = false;
            ButtonCancel.Visible = false;
            LabelNewName.Visible = false;
            TextBoxNewName.Visible = false;
        }

        protected void ButtonRenameGroup0_Click(object sender, EventArgs e)
        {
            LabelOldName1.Text = ASPxTabControl1.ActiveTab.Text;
            LabelOldName.Visible = true;
            LabelOldName1.Visible = true;
            ButtonSave.Visible = true;
            ButtonCancel.Visible = true;
            LabelNewName.Visible = true;
            TextBoxNewName.Text = "";
            TextBoxNewName.Visible = true;
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("update timing set categoryname='" + TextBoxNewName.Text + "' where plcid=" + LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2), connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            LabelOldName.Visible = false;
            LabelOldName1.Visible = false;
            ButtonSave.Visible = false;
            ButtonCancel.Visible = false;
            LabelNewName.Visible = false;
            TextBoxNewName.Visible = false;
            LoadTabControl(2);
        }

        protected void ButtonChangeGroupID_Click(object sender, EventArgs e)
        {
            LabelOldGroupID1.Text = LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2);
            DropDownListGroupID.Items.Clear();
            for (int i = 0; i <= 99; i++)
                DropDownListGroupID.Items.Add(i.ToString());
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select distinct plcid from timing", connection);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                    DropDownListGroupID.Items.Remove(reader["plcid"].ToString());
            reader.Close();
            connection.Close();

            LabelOldGroupID.Visible = true;
            LabelOldGroupID1.Visible = true;
            LabelNewGroupID.Visible = true;
            DropDownListGroupID.Visible = true;
            ButtonSave0.Visible = true;
            ButtonCancel0.Visible = true;
        }

        protected void ButtonCancel0_Click(object sender, EventArgs e)
        {
            LabelOldGroupID.Visible = false;
            LabelOldGroupID1.Visible = false;
            LabelNewGroupID.Visible = false;
            DropDownListGroupID.Visible = false;
            ButtonSave0.Visible = false;
            ButtonCancel0.Visible = false;
        }

        protected void ButtonSave0_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("update timing set plcid='" + DropDownListGroupID.Text + "' where plcid=" + LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2), connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            LabelOldGroupID.Visible = false;
            LabelOldGroupID1.Visible = false;
            LabelNewGroupID.Visible = false;
            DropDownListGroupID.Visible = false;
            ButtonSave0.Visible = false;
            ButtonCancel0.Visible = false;
            LoadTabControl(2);
        }

        protected void ButtonRefreshAll_Click(object sender, EventArgs e)
        {
            int[] ItemArray = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            String id = LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                ItemArray[i] = Convert.ToInt32(GridView1.Rows[i].Cells[4].Text);
            }

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 256", connection);
            cmd01.ExecuteNonQuery();
            SqlCommand cmd = new SqlCommand("insert into datarequeststiming select getdate()," + id.ToString() + "," + ItemArray[0] + "," + ItemArray[1] + "," + ItemArray[2] + "," + ItemArray[3] + "," + ItemArray[4] + "," + ItemArray[5] + "," + ItemArray[6] + "," + ItemArray[7] + "," + ItemArray[8] + "," + ItemArray[9] + ",0,0 select id=(select max(id) from datarequeststiming with(NOLOCK))", connection);

            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            //make sure message is processed
            bool succeeded = Raptor.MessageAckConfirm("datarequeststiming", int.Parse(reader["id"].ToString()));
            reader.Close();

            if (!succeeded)
            {
                LabelTimeout0.Visible = true;
            }
            SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-256 where (datarequests & 256)=256", connection);
            cmd1.ExecuteNonQuery();
            connection.Close();
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count == 10)
                ButtonInsertNewItem.Enabled = false;
            else
                ButtonInsertNewItem.Enabled = true;
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            //SqlDataSource2.SelectParameters["RecipeID"].DefaultValue = TreeView1.SelectedNode.Value;
            SqlDataSource2.SelectCommand = "SELECT id,[ItemName], [ItemValue],[min],[max] from [parameters] where id<50 and recipeid=" + TreeView1.SelectedNode.Value;
            GridView2.DataBind();
            UpdatePanel5.Update();

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand command = new SqlCommand("update Recipes set editing=0 update recipes set editing=1 where recipeid=" + TreeView1.SelectedNode.Value, connection);
            command.ExecuteNonQuery();
            connection.Close();

            if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
            {
                if (Global.OnlineSetup && CurrentUser.Access == 1)
                    Button4.Enabled = true;
            }
            else
            {
                ButtonRefreshAll.Enabled = false;
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1048576", connection);
            cmd01.ExecuteNonQuery();
            for (int i = 1; i < GridView2.Rows.Count; i++)
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestsparameters select getdate()," + GridView2.Rows[i].Cells[1].Text + ",0,0,0 select id=(select max(id) from datarequestsparameters with(NOLOCK))", connection);

                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsparameters", int.Parse(reader["id"].ToString()));
                reader.Close();

                if (!succeeded)
                {
                    LabelTimeout0.Visible = true;
                    break;
                }
            }
            SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1048576 where (datarequests & 1048576)=1048576", connection);
            cmd1.ExecuteNonQuery();
            connection.Close();
            GridView2.DataBind();
        }

        protected void GridView2_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridView2.SelectedIndex = -1;
        }

        protected void GridView2_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            GridView2.SelectedIndex = -1;
        }

        protected void GridView2_RowEditing(object sender, GridViewEditEventArgs e)
        {
            SqlDataSource2.SelectCommand = "SELECT id,[ItemName], [ItemValue],[min],[max] from [parameters] where id<50 and recipeid=" + TreeView1.SelectedNode.Value;
        }

        protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            SqlDataSource2.SelectCommand = "SELECT id,[ItemName], [ItemValue],[min],[max] from [parameters] where id<50 and recipeid=" + TreeView1.SelectedNode.Value;
        }

        protected void GridView2_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            SqlDataSource2.SelectCommand = "SELECT id,[ItemName], [ItemValue],[min],[max] from [parameters] where id<50 and recipeid=" + TreeView1.SelectedNode.Value;
        }

        protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int index = e.RowIndex;
            float item;
            GridViewRow row = GridView2.Rows[index];

            ((Label)GridView2.Rows[index].FindControl("LabelPLCTimeoutProd0")).Visible = false;
            String id = GridView2.Rows[index].Cells[1].Text;
            //String id = LabelPLCID.Text.Substring(LabelPLCID.Text.IndexOf(":") + 2);

            if (Convert.ToInt32(((TextBox)row.Cells[3].Controls[0]).Text) < Convert.ToInt32(((TextBox)row.Cells[4].Controls[0]).Text))
            {
                ((Label)GridView2.Rows[index].FindControl("LabelTooSmall0")).Visible = true;
                e.Cancel = true; // stops the automatic update command from happening
                return;
            }
            else if (Convert.ToInt32(((TextBox)row.Cells[3].Controls[0]).Text) > Convert.ToInt32(((TextBox)row.Cells[5].Controls[0]).Text))
            {
                ((Label)GridView2.Rows[index].FindControl("LabelTooLarge0")).Visible = true;
                e.Cancel = true; // stops the automatic update command from happening
                return;
            }
            item = Convert.ToSingle(((TextBox)row.Cells[3].Controls[0]).Text);
            if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
            {
                if (Global.OnlineSetup)
                {
                    Raptor cs1 = new Raptor();
                    string connectionString = Global.ConnectionString;
                    System.Data.SqlClient.SqlConnection connection;
                    connection = new SqlConnection(connectionString);
                    // Open the connection.
                    connection.Open();
                    SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1048576", connection);
                    cmd01.ExecuteNonQuery();
                    SqlCommand cmd = new SqlCommand("insert into datarequestsparameters select getdate()," + id.ToString() + "," + item + ",1,0 select id=(select max(id) from datarequestsparameters with(NOLOCK))", connection);

                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequestsparameters", int.Parse(reader["id"].ToString()));
                    reader.Close();

                    if (!succeeded)
                    {
                        ((Label)GridView2.Rows[index].FindControl("LabelPLCTimeoutProd0")).Visible = true;
                        e.Cancel = true; // stops the automatic update command from happening
                    }

                    SqlCommand cmd3 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1048576 where (datarequests & 1048576)=1048576", connection);
                    cmd3.ExecuteNonQuery();
                    connection.Close();
                }
            }

            SqlDataSource2.UpdateParameters["RecipeID"].DefaultValue = TreeView1.SelectedNode.Value;
        }

        protected void TreeView1_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            if (e.Node.ChildNodes.Count == 0)
            {
                switch (e.Node.Depth)
                {
                    case 0:
                        FillRecipeCategories(e.Node);
                        break;
                        //case 1:
                        //FillReportNames(e.Node);
                        //break;
                }
            }

            connection.Close();
        }

        private void FillRecipeCategories(TreeNode node)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("Select distinct RecipeLabel,recipeid,online,editing From Recipes order by Recipelabel", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            DataSet Recipes = new DataSet();
            adapter.Fill(Recipes);

            if (Recipes.Tables.Count > 0)
            {
                foreach (DataRow row in Recipes.Tables[0].Rows)
                {
                    TreeNode newNode = new
                    TreeNode(row["RecipeLabel"].ToString(), row["recipeid"].ToString());

                    newNode.PopulateOnDemand = true;
                    newNode.SelectAction = TreeNodeSelectAction.Select;
                    node.ChildNodes.Add(newNode);
                    if (row["online"].ToString() == "1")
                    {
                        newNode.Text = newNode.Text + "<span style='color: blue;'>" + "  [ACTIVE]" + "</span";
                    }
                    if (row["editing"].ToString() == "1")
                    {
                        newNode.Select();
                        Session["selectednode"] = TreeView1.SelectedNode.Text;
                    }
                    if (row["online"].ToString() == "1" && row["editing"].ToString() == "1")
                    {
                        if (Global.OnlineSetup)
                            ButtonRefreshAll.Enabled = true;
                    }
                    else if (row["editing"].ToString() == "1" && row["online"].ToString() != "1")
                    {
                        ButtonRefreshAll.Enabled = false;
                    }
                }
            }
            connection.Close();
        }

        protected void ButtonInsertNewItem0_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("insert into parameters select recipeid,max(id)+1,'{New Item}',0,0,0 from parameters  where id<50 group by recipeid", connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            GridView2.DataBind();
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
        }

        protected void GridView3_DataBound(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count == 10)
                ButtonInsertNewItem.Enabled = false;
            else
                ButtonInsertNewItem.Enabled = true;
        }

        protected void GridView3_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //if ((int)Session["OnlineSetup"] == 1)
            {
                Tag MyTagX, MyTagY;


                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand cmdip = new SqlCommand("select * from RaptorCommSettings", connection);
                SqlDataReader readerip = cmdip.ExecuteReader();
                readerip.Read();
                if (readerip.HasRows)
                {
                    MyPLC.IPAddress = readerip["PLCIPAddress"].ToString();
                    MyPLC.Path = readerip["PLCProcessorSlot"].ToString();
                    MyPLC.Timeout = int.Parse(readerip["PLCTimeout"].ToString());

                }
                readerip.Close();
                connection.Close();
                if (MyPLC.Connect() != ResultCode.E_SUCCESS)
                {
                    return;
                }
                //for (int i = 0; i < 10; i++)
                {
                    MyTagX = new Tag("WebSortCountX[" + e.RowIndex + "].PRE");
                    MyTagX.DataType = Logix.Tag.ATOMIC.DINT;
                    MyTagY = new Tag("WebSortCountY[" + e.RowIndex + "].PRE");
                    MyTagY.DataType = Logix.Tag.ATOMIC.DINT;

                    try
                    {
                        int index = e.RowIndex;
                        GridViewRow row = GridView3.Rows[index];

                                          
                        MyTagX.Value = (((TextBox)row.Cells[3].Controls[0]).Text);
                        MyPLC.WriteTag(MyTagX);
                        MyTagY.Value = (((TextBox)row.Cells[4].Controls[0]).Text);
                        MyPLC.WriteTag(MyTagY);
                    }
                    catch { }
                }
            }
        }
    }
}