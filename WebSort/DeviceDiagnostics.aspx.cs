using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class DeviceDiagnostics : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Device Diagnostics", User.Identity.Name);

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
                TreeView1.Enabled = false;
                TreeView2.Enabled = false;
                ButtonAddNewTest.Enabled = false;
                ButtonAddNewMode0.Enabled = false;
                ButtonRenameItem.Enabled = false;
                ButtonChangeItemID.Enabled = false;
                ButtonDeleteItem.Enabled = false;
                ButtonRenameItem0.Enabled = false;
                ButtonChangeItemID0.Enabled = false;
                ButtonDeleteItem0.Enabled = false;
            }
            else
            {
                TreeView1.Enabled = true;
                TreeView2.Enabled = true;
                ButtonAddNewTest.Enabled = true;
                ButtonAddNewMode0.Enabled = true;
                ButtonRenameItem.Enabled = true;
                ButtonChangeItemID.Enabled = true;
                ButtonDeleteItem.Enabled = true;
                ButtonRenameItem0.Enabled = true;
                ButtonChangeItemID0.Enabled = true;
                ButtonDeleteItem0.Enabled = true;
            }

            if (IsPostBack)
                return;
            Session["editing"] = "0";
            TextBoxParameter.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");
            TextBoxParameterDescription.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");
            TextBoxParameter0.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");
            TextBoxParameterDescription0.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");
            //Session["DiagSelectedNode"] = null;
            // read the diagnostic data from the PLC
            if (Global.OnlineSetup)
            {
                bool succeeded = false;
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();

                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 128", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmdd = new SqlCommand("select deviceid from diagnostictests", connection);
                SqlDataReader readerd = cmdd.ExecuteReader();
                while (readerd.Read())
                {
                    SqlCommand cmd = new SqlCommand("insert into datarequestsdiagnostic select getdate()," + readerd["deviceid"].ToString() + ",0,'',0,0 select id=(select max(id) from datarequestsdiagnostic with(NOLOCK))", connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    succeeded = Raptor.MessageAckConfirm("datarequestsdiagnostic", int.Parse(reader["id"].ToString()));
                    reader.Close();
                    if (!succeeded)
                    {
                        LabelTimeout.Visible = true;
                        return;
                    }
                }
                readerd.Close();
                SqlCommand cmdd1 = new SqlCommand("select deviceid from diagnostictests1", connection);
                SqlDataReader readerd1 = cmdd1.ExecuteReader();
                while (readerd1.Read())
                {
                    SqlCommand cmd = new SqlCommand("insert into datarequestsdiagnostic1 select getdate()," + readerd1["deviceid"].ToString() + ",0,'',0,0 select id=(select max(id) from datarequestsdiagnostic1 with(NOLOCK))", connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    succeeded = Raptor.MessageAckConfirm("datarequestsdiagnostic1", int.Parse(reader["id"].ToString()));
                    reader.Close();
                    if (!succeeded)
                    {
                        LabelTimeout.Visible = true;
                        return;
                    }
                }
                readerd1.Close();
                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-128 where (datarequests & 128)=128", connection);
                cmd1.ExecuteNonQuery();
                connection.Close();
            }
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
                        FillDeviceCategories(e.Node);
                        break;

                    case 1:
                        FillDeviceNames(e.Node);
                        break;
                }
            }
            connection.Close();
        }

        private void FillDeviceCategories(TreeNode node)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("Select distinct CategoryName,categoryid From DiagnosticTests order by CategoryID", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            DataSet ReportCategories = new DataSet();
            adapter.Fill(ReportCategories);

            if (ReportCategories.Tables.Count > 0)
            {
                foreach (DataRow row in ReportCategories.Tables[0].Rows)
                {
                    TreeNode newNode = new
                    TreeNode(row["CategoryName"].ToString(), row["CategoryID"].ToString());

                    newNode.PopulateOnDemand = true;
                    newNode.SelectAction = TreeNodeSelectAction.Expand;
                    node.ChildNodes.Add(newNode);
                }
            }
            connection.Close();
        }

        private void FillDeviceCategories1(TreeNode node)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("Select distinct CategoryName,categoryid From DiagnosticTests1 order by CategoryID", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            DataSet ReportCategories = new DataSet();
            adapter.Fill(ReportCategories);

            if (ReportCategories.Tables.Count > 0)
            {
                foreach (DataRow row in ReportCategories.Tables[0].Rows)
                {
                    TreeNode newNode = new
                    TreeNode(row["CategoryName"].ToString(), row["CategoryID"].ToString());

                    newNode.PopulateOnDemand = true;
                    newNode.SelectAction = TreeNodeSelectAction.Expand;
                    node.ChildNodes.Add(newNode);
                }
            }
            connection.Close();
        }

        private void FillDeviceNames(TreeNode node)
        {
            string CategoryID = node.Value;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("Select * From DiagnosticTests Where CategoryID=" + CategoryID.ToString(), connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet Reports = new DataSet();
            adapter.Fill(Reports);
            if (Reports.Tables.Count > 0)
            {
                foreach (DataRow row in Reports.Tables[0].Rows)
                {
                    //TreeNode newNode = new TreeNode(row["DeviceLabel"].ToString(), row["DeviceID"].ToString());
                    if (row["DiagnosticOn"].ToString() == "True")
                    {
                        //TreeNode newNode = new TreeNode("<span style='color: blue;'>" + row["DeviceLabel"].ToString() + " {" + row["DeviceID"].ToString() + "}" + "</span", row["DeviceID"].ToString());
                        TreeNode newNode = new TreeNode(row["DeviceLabel"].ToString() + " {" + row["DeviceID"].ToString() + "}", row["DeviceID"].ToString());
                        newNode.Checked = true;
                        newNode.PopulateOnDemand = false;
                        newNode.SelectAction = TreeNodeSelectAction.Select;
                        node.ChildNodes.Add(newNode);
                        if (newNode.Text.Contains("ncoder") && newNode.Checked == true)
                        {
                            Timer3.Enabled = true;
                            Labelenc.Visible = true;
                        }
                        else
                        {
                            Timer3.Enabled = false;
                            Labelenc.Visible = false;
                        }

                        if (Session["DiagSelectedNode"] == null)
                            Session["DiagSelectedNode"] = "1";
                        else if (newNode.Text.Contains("[New"))
                        {
                            newNode.Selected = true;
                            Session["DiagSelectedNode"] = newNode.Value.ToString();
                        }
                        else if (Session["DiagSelectedNode"].ToString() == row["DeviceID"].ToString())
                            newNode.Select();
                    }
                    else
                    {
                        TreeNode newNode = new TreeNode(row["DeviceLabel"].ToString() + " {" + row["DeviceID"].ToString() + "}", row["DeviceID"].ToString());
                        newNode.PopulateOnDemand = false;
                        newNode.SelectAction = TreeNodeSelectAction.Select;
                        node.ChildNodes.Add(newNode);
                        if (Session["DiagSelectedNode"] == null)
                            Session["DiagSelectedNode"] = "1";
                        else if (newNode.Text.Contains("[New"))
                        {
                            newNode.Selected = true;
                            Session["DiagSelectedNode"] = newNode.Value.ToString();
                        }
                        else if (Session["DiagSelectedNode"].ToString() == row["DeviceID"].ToString())
                            newNode.Select();
                    }
                }
                if (Session["DiagSelectedNode"].ToString() == "1")
                    TreeView1.Nodes[0].ChildNodes[0].ChildNodes[0].Select();
                try
                {
                    SqlCommand cmd00 = new SqlCommand("select * from diagnostictests where deviceid=" + TreeView1.SelectedNode.Value, connection);
                    SqlDataReader reader00 = cmd00.ExecuteReader();
                    reader00.Read();
                    TextBoxParameterDescription.Text = reader00["parameterdescription"].ToString();
                    TextBoxParameter.Text = reader00["parameter"].ToString();
                    reader00.Close();
                }
                catch { }
            }
            connection.Close();
        }

        private void FillDeviceNames1(TreeNode node)
        {
            string CategoryID = node.Value;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("Select * From DiagnosticTests1 Where CategoryID=" + CategoryID.ToString(), connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet Reports = new DataSet();
            adapter.Fill(Reports);
            if (Reports.Tables.Count > 0)
            {
                foreach (DataRow row in Reports.Tables[0].Rows)
                {
                    //TreeNode newNode = new TreeNode(row["DeviceLabel"].ToString(), row["DeviceID"].ToString());
                    if (row["DiagnosticOn"].ToString() == "True")
                    {
                        //TreeNode newNode = new TreeNode("<span style='color: blue;'>" + row["DeviceLabel"].ToString() + " {" + row["DeviceID"].ToString() + "}" + "</span", row["DeviceID"].ToString());
                        TreeNode newNode = new TreeNode(row["DeviceLabel"].ToString() + " {" + row["DeviceID"].ToString() + "}", row["DeviceID"].ToString());
                        newNode.Checked = true;
                        newNode.PopulateOnDemand = false;
                        newNode.SelectAction = TreeNodeSelectAction.Select;
                        node.ChildNodes.Add(newNode);
                        if (Session["DiagSelectedNode"] == null)
                            Session["DiagSelectedNode"] = "1";
                        else if (newNode.Text.Contains("[New"))
                        {
                            newNode.Selected = true;
                            Session["DiagSelectedNode"] = newNode.Value.ToString();
                        }
                        else if (Session["DiagSelectedNode"].ToString() == row["DeviceID"].ToString())
                            newNode.Select();
                    }
                    else
                    {
                        TreeNode newNode = new TreeNode(row["DeviceLabel"].ToString() + " {" + row["DeviceID"].ToString() + "}", row["DeviceID"].ToString());
                        newNode.PopulateOnDemand = false;
                        newNode.SelectAction = TreeNodeSelectAction.Select;
                        node.ChildNodes.Add(newNode);

                        if (Session["DiagSelectedNode"] == null)
                            Session["DiagSelectedNode"] = "1";
                        else if (newNode.Text.Contains("[New"))
                        {
                            newNode.Selected = true;
                            Session["DiagSelectedNode"] = newNode.Value.ToString();
                        }
                        else if (Session["DiagSelectedNode"].ToString() == row["DeviceID"].ToString())
                            newNode.Select();
                    }
                }
                if (Session["DiagSelectedNode"].ToString() == "1")
                    TreeView2.Nodes[0].ChildNodes[0].ChildNodes[0].Select();
                try
                {
                    SqlCommand cmd00 = new SqlCommand("select * from diagnostictests1 where deviceid=" + TreeView2.SelectedNode.Value, connection);
                    SqlDataReader reader00 = cmd00.ExecuteReader();
                    reader00.Read();
                    TextBoxParameterDescription0.Text = reader00["parameterdescription"].ToString();
                    TextBoxParameter0.Text = reader00["parameter"].ToString();
                    reader00.Close();
                }
                catch { }
            }
            connection.Close();
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            //Timer2.Enabled = true;
            Session["DiagSelectedNode"] = TreeView1.SelectedNode.Value.ToString();
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd0 = new SqlCommand("select * from diagnostictests where deviceid=" + TreeView1.SelectedNode.Value, connection);
            SqlDataReader reader0 = cmd0.ExecuteReader();
            reader0.Read();
            TextBoxParameterDescription.Text = reader0["parameterdescription"].ToString();
            TextBoxParameter.Text = reader0["parameter"].ToString();
            UpdatePanel7.Update();
            reader0.Close();
            connection.Close();
        }

        protected void TreeView1_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            bool succeeded = false;
            int DiagnosticMap = 0;

            Timer2.Enabled = false;
            Session["editing"] = "1";

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            foreach (TreeNode node in TreeView1.Nodes)
            {
                foreach (TreeNode node1 in node.ChildNodes)
                {
                    foreach (TreeNode node2 in node1.ChildNodes)
                    {
                        if (node2.Text.Contains("ncoder"))
                        {
                            Timer3.Enabled = node2.Checked;
                            Labelenc.Visible = node2.Checked;
                        }
                    }
                }
            }

            UpdatePanel11.Update();
            e.Node.Select();
            Session["DiagSelectedNode"] = e.Node.Value.ToString();

            //if (Session["DiagSelectedNode"].ToString() != e.Node.Value.ToString())
            {
                SqlCommand cmd00 = new SqlCommand("select * from diagnostictests where deviceid=" + e.Node.Value, connection);
                SqlDataReader reader00 = cmd00.ExecuteReader();
                reader00.Read();
                //if (TextBoxParameter.Text != reader00["parameter"].ToString())
                {
                    TextBoxParameterDescription.Text = reader00["parameterdescription"].ToString();
                    TextBoxParameter.Text = reader00["parameter"].ToString();
                    UpdatePanel7.Update();
                }
                reader00.Close();
            }

            SqlCommand cmd0 = new SqlCommand("select x=sum(power(2,deviceid)) from diagnostictests where diagnosticon=1", connection);
            SqlDataReader reader0 = cmd0.ExecuteReader();
            reader0.Read();
            if (reader0["x"].ToString() == "")
                DiagnosticMap = 0;
            else
                DiagnosticMap = int.Parse(reader0["x"].ToString());

            if (e.Node.Checked)
                DiagnosticMap = DiagnosticMap | (int)Math.Pow(2, Convert.ToInt32(e.Node.Value));
            else if ((DiagnosticMap & (int)Math.Pow(2, Convert.ToInt32(e.Node.Value))) == (int)Math.Pow(2, Convert.ToInt32(e.Node.Value)))
                DiagnosticMap = DiagnosticMap - (int)Math.Pow(2, Convert.ToInt32(e.Node.Value));

            try
            {
                // avoid putting duplicate entries in datarequestsdiagnostic due to the different behavior of the checkbox postbacks in the treeview
                //SqlCommand cmd11 = new SqlCommand("select c=abs(datediff(ms,getdate(),timestamp)) from datarequestsdiagnostic where write=1 and id = (select max(id) from datarequestsdiagnostic where diagnosticid=" + e.Node.Value + ")", connection);
                SqlCommand cmd11 = new SqlCommand("select c=count(*) from datarequestsdiagnostic where write=1 and id = (select max(id) from datarequestsdiagnostic) and diagnosticid=" + e.Node.Value + " and diagnosticmap=" + DiagnosticMap, connection);

                SqlDataReader reader11 = cmd11.ExecuteReader();
                reader11.Read();
                //if (int.Parse(reader11["c"].ToString()) < 4000)
                if (int.Parse(reader11["c"].ToString()) > 0)
                    return;
                reader11.Close();
            }
            catch { }
            if (Global.OnlineSetup)
            {
                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 128", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand("insert into datarequestsdiagnostic select getdate()," + e.Node.Value + "," + DiagnosticMap.ToString() + ",'" + TextBoxParameter.Text + "',1,0 select id=(select max(id) from datarequestsdiagnostic with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                succeeded = Raptor.MessageAckConfirm("datarequestsdiagnostic", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout.Visible = true;
                    e.Node.Checked = !e.Node.Checked;
                    //Timer2.Enabled = true;
                    Session["editing"] = "0";
                    return;
                }
                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-128 where (datarequests & 128)=128", connection);
                cmd1.ExecuteNonQuery();
            }
            if (succeeded || !Global.OnlineSetup)
            {
                int autochecked = 0;
                if (e.Node.Checked == true)
                    autochecked = 1;
                SqlCommand command = new SqlCommand("update diagnostictests set diagnosticon = " + autochecked.ToString() + " where deviceid = " + e.Node.Value.ToString(), connection);
                command.ExecuteNonQuery();
                //Timer2.Enabled = true;
                Session["editing"] = "0";
                //Response.Redirect("devicediagnostics.aspx");
            }
            connection.Close();
        }

        protected void ButtonRefreshAll_Click(object sender, EventArgs e)
        {
        }

        protected void ButtonDeleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd = new SqlCommand("delete from diagnostictests where deviceid =" + TreeView1.SelectedNode.Value.ToString(), connection);

                cmd.ExecuteNonQuery();
                connection.Close();
                Session["DiagSelectedNode"] = null;
                Response.Redirect("devicediagnostics.aspx");
            }
            catch
            {
                Raptor msg = new Raptor();
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Please select an item."));
            }
        }

        protected void ButtonRenameItem_Click(object sender, EventArgs e)
        {
            try
            {
                LabelOldName1.Text = TreeView1.SelectedNode.Text.Substring(0, TreeView1.SelectedNode.Text.IndexOf("{") - 1);
                LabelOldName.Visible = true;
                LabelOldName1.Visible = true;
                ButtonSave.Visible = true;
                ButtonCancel.Visible = true;
                LabelNewName.Visible = true;
                TextBoxNewName.Text = "";
                TextBoxNewName.Visible = true;
                UpdatePanel8.Update();
            }
            catch
            {
                Raptor msg = new Raptor();
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Please select an item."));
            }
        }

        protected void ButtonChangeItemID_Click(object sender, EventArgs e)
        {
            try
            {
                LabelOldItemID1.Text = TreeView1.SelectedNode.Value.ToString();
                DropDownListItemID.Items.Clear();

                for (int i = 0; i < 32; i++)
                    DropDownListItemID.Items.Add(i.ToString());
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd = new SqlCommand("select distinct deviceid from diagnostictests", connection);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                        DropDownListItemID.Items.Remove(reader["deviceid"].ToString());
                reader.Close();
                connection.Close();

                LabelOldItemID.Visible = true;
                LabelOldItemID1.Visible = true;
                LabelNewItemID.Visible = true;
                DropDownListItemID.Visible = true;
                ButtonSave0.Visible = true;
                ButtonCancel0.Visible = true;
                UpdatePanel8.Update();
            }
            catch
            {
                Raptor msg = new Raptor();
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Please select an item."));
            }
        }

        protected void ButtonAddNewTest_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("if (select count(*) from diagnostictests) = 32 return else if (select count(*) from diagnostictests) > 0 insert into diagnostictests select max(deviceid)+1,'[New Test]',0,1,'Device Tests',0,'' from diagnostictests else insert into diagnostictests select 0,'[New Test]}',0,1,'Device Tests',0,''", connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            Response.Redirect("devicediagnostics.aspx");
        }

        protected void ButtonAddNewMode_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("if (select count(*) from diagnostictests where deviceid>=16) = 16 return else if (select count(*) from diagnostictests where deviceid>=16) > 0 insert into diagnostictests select max(deviceid)+1,'[New Mode]',0,2,'Device Modes',0,'' from diagnostictests where deviceid>=16 else insert into diagnostictests select 16,'[New Mode]',0,2,'Device Modes',0,''", connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            Response.Redirect("devicediagnostics.aspx");
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("update diagnostictests set devicelabel='" + TextBoxNewName.Text.Replace("'", "''") + "' where deviceid=" + TreeView1.SelectedNode.Value.ToString(), connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            LabelOldName.Visible = false;
            LabelOldName1.Visible = false;
            ButtonSave.Visible = false;
            ButtonCancel.Visible = false;
            LabelNewName.Visible = false;
            TextBoxNewName.Visible = false;
            Response.Redirect("devicediagnostics.aspx");
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

        protected void ButtonSave0_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("update diagnostictests set deviceid='" + DropDownListItemID.Text + "' where deviceid=" + TreeView1.SelectedNode.Value.ToString(), connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            LabelOldItemID.Visible = false;
            LabelOldItemID1.Visible = false;
            LabelNewItemID.Visible = false;
            DropDownListItemID.Visible = false;
            ButtonSave0.Visible = false;
            ButtonCancel0.Visible = false;
            Session["DiagSelectedNode"] = DropDownListItemID.Text;
            Response.Redirect("devicediagnostics.aspx");
        }

        protected void ButtonCancel0_Click(object sender, EventArgs e)
        {
            LabelOldItemID.Visible = false;
            LabelOldItemID1.Visible = false;
            LabelNewItemID.Visible = false;
            DropDownListItemID.Visible = false;
            ButtonSave0.Visible = false;
            ButtonCancel0.Visible = false;
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            if (Session["editing"].ToString() == "1")
                return;
            // read the diagnostic data from the PLC
            if (Global.OnlineSetup)
            {
                bool succeeded = false;
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();

                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 128", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmdd = new SqlCommand("select deviceid from diagnostictests", connection);
                SqlDataReader readerd = cmdd.ExecuteReader();
                while (readerd.Read())
                {
                    if (Session["editing"].ToString() == "1")
                        return;
                    SqlCommand cmd = new SqlCommand("insert into datarequestsdiagnostic select getdate()," + readerd["deviceid"].ToString() + ",0,'',0,0 select id=(select max(id) from datarequestsdiagnostic with(NOLOCK))", connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    succeeded = Raptor.MessageAckConfirm("datarequestsdiagnostic", int.Parse(reader["id"].ToString()));
                    reader.Close();
                    if (!succeeded)
                    {
                        LabelTimeout.Visible = true;
                        return;
                    }
                }
                if (Session["editing"].ToString() != "1")
                {
                    SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-128 where (datarequests & 128)=128", connection);
                    cmd1.ExecuteNonQuery();
                }

                //loop through the tree to see if the checked values are different than the database, if so, reload the screen
                TreeNodeCollection childnodes = TreeView1.Nodes[0].ChildNodes[0].ChildNodes;
                TreeNodeCollection childnodes1 = TreeView1.Nodes[0].ChildNodes[1].ChildNodes;
                try
                {
                    foreach (TreeNode n in childnodes)
                    {
                        if (Session["editing"].ToString() == "1")
                            return;
                        SqlCommand cmdtree = new SqlCommand("select diagnosticon from diagnostictests where deviceid=" + n.Value.ToString(), connection);
                        SqlDataReader readertree = cmdtree.ExecuteReader();
                        readertree.Read();
                        if (n.Checked != Convert.ToBoolean(readertree["diagnosticon"].ToString()))
                        {
                            Response.Redirect("devicediagnostics.aspx");
                            break;
                        }
                    }
                    foreach (TreeNode n in childnodes1)
                    {
                        if (Session["editing"].ToString() == "1")
                            return;
                        SqlCommand cmdtree = new SqlCommand("select diagnosticon from diagnostictests where deviceid=" + n.Value.ToString(), connection);
                        SqlDataReader readertree = cmdtree.ExecuteReader();
                        readertree.Read();
                        if (n.Checked != Convert.ToBoolean(readertree["diagnosticon"].ToString()))
                        {
                            Response.Redirect("devicediagnostics.aspx");
                            break;
                        }
                    }
                }
                catch { }
                connection.Close();
            }
        }

        protected void TextBoxNewName_TextChanged(object sender, EventArgs e)
        {
        }

        protected void ButtonSave1_Click(object sender, EventArgs e)
        {
            Timer2.Enabled = false;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand command = new SqlCommand("update diagnostictests set parameter=" + TextBoxParameter.Text + ",parameterdescription='" + TextBoxParameterDescription.Text + "' where deviceid = " + TreeView1.SelectedNode.Value.ToString(), connection);
            command.ExecuteNonQuery();

            if (Global.OnlineSetup)
            {
                int DiagnosticMap = 0;
                SqlCommand cmd0 = new SqlCommand("select x=sum(power(2,deviceid)) from diagnostictests where diagnosticon=1", connection);
                SqlDataReader reader0 = cmd0.ExecuteReader();
                reader0.Read();
                if (reader0["x"].ToString() == "")
                    DiagnosticMap = 0;
                else
                    DiagnosticMap = int.Parse(reader0["x"].ToString());
                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 128", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand("insert into datarequestsdiagnostic select getdate()," + TreeView1.SelectedNode.Value.ToString() + "," + DiagnosticMap.ToString() + ",'" + TextBoxParameter.Text + "',1,0 select id=(select max(id) from datarequestsdiagnostic with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsdiagnostic", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout.Visible = true;
                    //Timer2.Enabled = true;
                    return;
                }
                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-128 where (datarequests & 128)=128", connection);
                cmd1.ExecuteNonQuery();
            }

            connection.Close();
            //Timer2.Enabled = true;
        }

        protected void TreeView2_TreeNodePopulate(object sender, TreeNodeEventArgs e)
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
                        FillDeviceCategories1(e.Node);
                        break;

                    case 1:
                        FillDeviceNames1(e.Node);
                        break;
                }
            }
            connection.Close();
        }

        protected void TreeView2_SelectedNodeChanged(object sender, EventArgs e)
        {
            //Timer2.Enabled = true;
            Session["DiagSelectedNode"] = TreeView2.SelectedNode.Value.ToString();
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd0 = new SqlCommand("select * from diagnostictests1 where deviceid=" + TreeView2.SelectedNode.Value, connection);
            SqlDataReader reader0 = cmd0.ExecuteReader();
            reader0.Read();
            TextBoxParameterDescription0.Text = reader0["parameterdescription"].ToString();
            TextBoxParameter0.Text = reader0["parameter"].ToString();
            UpdatePanel10.Update();
            reader0.Close();
            connection.Close();
        }

        protected void TreeView2_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            bool succeeded = false;
            int DiagnosticMap = 0;

            Timer2.Enabled = false;
            Session["editing"] = "1";

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            e.Node.Select();
            Session["DiagSelectedNode"] = e.Node.Value.ToString();

            //if (Session["DiagSelectedNode"].ToString() != e.Node.Value.ToString())
            {
                SqlCommand cmd00 = new SqlCommand("select * from diagnostictests1 where deviceid=" + e.Node.Value, connection);
                SqlDataReader reader00 = cmd00.ExecuteReader();
                reader00.Read();
                //if (TextBoxParameter.Text != reader00["parameter"].ToString())
                {
                    TextBoxParameterDescription0.Text = reader00["parameterdescription"].ToString();
                    TextBoxParameter0.Text = reader00["parameter"].ToString();
                    UpdatePanel10.Update();
                }
                reader00.Close();
            }

            SqlCommand cmd0 = new SqlCommand("select x=sum(power(2,deviceid)) from diagnostictests1 where diagnosticon=1", connection);
            SqlDataReader reader0 = cmd0.ExecuteReader();
            reader0.Read();
            if (reader0["x"].ToString() == "")
                DiagnosticMap = 0;
            else
                DiagnosticMap = int.Parse(reader0["x"].ToString());

            if (e.Node.Checked)
                DiagnosticMap = DiagnosticMap | (int)Math.Pow(2, Convert.ToInt32(e.Node.Value));
            else if ((DiagnosticMap & (int)Math.Pow(2, Convert.ToInt32(e.Node.Value))) == (int)Math.Pow(2, Convert.ToInt32(e.Node.Value)))
                DiagnosticMap = DiagnosticMap - (int)Math.Pow(2, Convert.ToInt32(e.Node.Value));

            try
            {
                // avoid putting duplicate entries in datarequestsdiagnostic due to the different behavior of the checkbox postbacks in the treeview
                //SqlCommand cmd11 = new SqlCommand("select c=abs(datediff(ms,getdate(),timestamp)) from datarequestsdiagnostic where write=1 and id = (select max(id) from datarequestsdiagnostic where diagnosticid=" + e.Node.Value + ")", connection);
                SqlCommand cmd11 = new SqlCommand("select c=count(*) from datarequestsdiagnostic1 where write=1 and id = (select max(id) from datarequestsdiagnostic1) and diagnosticid=" + e.Node.Value + " and diagnosticmap=" + DiagnosticMap, connection);

                SqlDataReader reader11 = cmd11.ExecuteReader();
                reader11.Read();
                //if (int.Parse(reader11["c"].ToString()) < 4000)
                if (int.Parse(reader11["c"].ToString()) > 0)
                    return;
                reader11.Close();
            }
            catch { }
            if (Global.OnlineSetup)
            {
                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 128", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand("insert into datarequestsdiagnostic1 select getdate()," + e.Node.Value + "," + DiagnosticMap.ToString() + ",'" + TextBoxParameter0.Text + "',1,0 select id=(select max(id) from datarequestsdiagnostic1 with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                succeeded = Raptor.MessageAckConfirm("datarequestsdiagnostic1", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout.Visible = true;
                    e.Node.Checked = !e.Node.Checked;
                    //Timer2.Enabled = true;
                    Session["editing"] = "0";
                    return;
                }
                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-128 where (datarequests & 128)=128", connection);
                cmd1.ExecuteNonQuery();
            }
            if (succeeded || !Global.OnlineSetup)
            {
                int autochecked = 0;
                if (e.Node.Checked == true)
                    autochecked = 1;
                SqlCommand command = new SqlCommand("update diagnostictests1 set diagnosticon = " + autochecked.ToString() + " where deviceid = " + e.Node.Value.ToString(), connection);
                command.ExecuteNonQuery();
                //Timer2.Enabled = true;
                Session["editing"] = "0";
                //Response.Redirect("devicediagnostics.aspx");
            }
            connection.Close();
        }

        protected void ButtonAddNewMode0_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("if (select count(*) from diagnostictests1) = 32 return else if (select count(*) from diagnostictests1) > 0 insert into diagnostictests1 select max(deviceid)+1,'[New Mode]',0,1,'Device Modes',0,'' from diagnostictests1 else insert into diagnostictests1 select 0,'[New Mode]}',0,1,'Device Modes',0,''", connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            Response.Redirect("devicediagnostics.aspx");
        }

        protected void ButtonRenameItem0_Click(object sender, EventArgs e)
        {
            try
            {
                LabelOldName3.Text = TreeView2.SelectedNode.Text.Substring(0, TreeView2.SelectedNode.Text.IndexOf("{") - 1);
                LabelOldName2.Visible = true;
                LabelOldName3.Visible = true;
                ButtonSave2.Visible = true;
                ButtonCancel2.Visible = true;
                LabelNewName0.Visible = true;
                TextBoxNewName0.Text = "";
                TextBoxNewName0.Visible = true;
                UpdatePanel9.Update();
            }
            catch
            {
                Raptor msg = new Raptor();
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Please select an item."));
            }
        }

        protected void ButtonDeleteItem0_Click(object sender, EventArgs e)
        {
            try
            {
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd = new SqlCommand("delete from diagnostictests1 where deviceid =" + TreeView2.SelectedNode.Value.ToString(), connection);

                cmd.ExecuteNonQuery();
                connection.Close();
                Session["DiagSelectedNode"] = null;
                Response.Redirect("devicediagnostics.aspx");
            }
            catch
            {
                Raptor msg = new Raptor();
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Please select an item."));
            }
        }

        protected void ButtonChangeItemID0_Click(object sender, EventArgs e)
        {
            try
            {
                LabelOldItemID3.Text = TreeView2.SelectedNode.Value.ToString();
                DropDownListItemID0.Items.Clear();

                for (int i = 0; i < 32; i++)
                    DropDownListItemID0.Items.Add(i.ToString());
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand cmd = new SqlCommand("select distinct deviceid from diagnostictests1", connection);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                        DropDownListItemID0.Items.Remove(reader["deviceid"].ToString());
                reader.Close();
                connection.Close();

                LabelOldItemID2.Visible = true;
                LabelOldItemID3.Visible = true;
                LabelNewItemID0.Visible = true;
                DropDownListItemID0.Visible = true;
                ButtonSave3.Visible = true;
                ButtonCancel3.Visible = true;
                UpdatePanel9.Update();
            }
            catch
            {
                Raptor msg = new Raptor();
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Please select an item."));
            }
        }

        protected void ButtonSave4_Click(object sender, EventArgs e)
        {
            Timer2.Enabled = false;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand command = new SqlCommand("update diagnostictests1 set parameter=" + TextBoxParameter0.Text + ",parameterdescription='" + TextBoxParameterDescription0.Text + "' where deviceid = " + TreeView2.SelectedNode.Value.ToString(), connection);
            command.ExecuteNonQuery();

            if (Global.OnlineSetup)
            {
                int DiagnosticMap = 0;
                SqlCommand cmd0 = new SqlCommand("select x=sum(power(2,deviceid)) from diagnostictests1 where diagnosticon=1", connection);
                SqlDataReader reader0 = cmd0.ExecuteReader();
                reader0.Read();
                if (reader0["x"].ToString() == "")
                    DiagnosticMap = 0;
                else
                    DiagnosticMap = int.Parse(reader0["x"].ToString());
                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 128", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand("insert into datarequestsdiagnostic1 select getdate()," + TreeView2.SelectedNode.Value.ToString() + "," + DiagnosticMap.ToString() + ",'" + TextBoxParameter0.Text + "',1,0 select id=(select max(id) from datarequestsdiagnostic1 with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsdiagnostic1", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout.Visible = true;
                    //Timer2.Enabled = true;
                    return;
                }
                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-128 where (datarequests & 128)=128", connection);
                cmd1.ExecuteNonQuery();
            }

            connection.Close();
            //Timer2.Enabled = true;
        }

        protected void ButtonSave2_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("update diagnostictests1 set devicelabel='" + TextBoxNewName0.Text.Replace("'", "''") + "' where deviceid=" + TreeView2.SelectedNode.Value.ToString(), connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            LabelOldName2.Visible = false;
            LabelOldName3.Visible = false;
            ButtonSave2.Visible = false;
            ButtonCancel2.Visible = false;
            LabelNewName0.Visible = false;
            TextBoxNewName0.Visible = false;
            Response.Redirect("devicediagnostics.aspx");
        }

        protected void ButtonCancel2_Click(object sender, EventArgs e)
        {
            LabelOldName2.Visible = false;
            LabelOldName3.Visible = false;
            ButtonSave2.Visible = false;
            ButtonCancel2.Visible = false;
            LabelNewName0.Visible = false;
            TextBoxNewName0.Visible = false;
        }

        protected void ButtonSave3_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("update diagnostictests1 set deviceid='" + DropDownListItemID0.Text + "' where deviceid=" + TreeView2.SelectedNode.Value.ToString(), connection);

            cmd.ExecuteNonQuery();
            connection.Close();
            LabelOldItemID2.Visible = false;
            LabelOldItemID3.Visible = false;
            LabelNewItemID0.Visible = false;
            DropDownListItemID0.Visible = false;
            ButtonSave3.Visible = false;
            ButtonCancel3.Visible = false;
            Session["DiagSelectedNode"] = DropDownListItemID0.Text;
            Response.Redirect("devicediagnostics.aspx");
        }

        protected void ButtonCancel3_Click(object sender, EventArgs e)
        {
            LabelOldItemID2.Visible = false;
            LabelOldItemID3.Visible = false;
            LabelNewItemID0.Visible = false;
            DropDownListItemID0.Visible = false;
            ButtonSave3.Visible = false;
            ButtonCancel3.Visible = false;
        }

        protected void Timer3_Tick(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand cmde = new SqlCommand("select mainencoderactual from currentstate", connection);
            SqlDataReader readere = cmde.ExecuteReader();
            readere.Read();
            Labelenc.Text = readere["mainencoderactual"].ToString();
            readere.Close();

            UpdatePanel11.Update();

            connection.Close();
        }
    }
}