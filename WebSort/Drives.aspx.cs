using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class Drives : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Drive Setup", User.Identity.Name);
            
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
                GridViewDrives.Enabled = false;
                ButtonRefreshAll.Enabled = false;
                ButtonRefreshAll0.Enabled = false;
                Grid1.Enabled = false;
                Grid2.Enabled = false;
                Grid3.Enabled = false;
            }
            else
            {
                Grid1.Enabled = true;
                Grid2.Enabled = true;
                Grid3.Enabled = true;
                GridView1.Enabled = true;
                GridViewDrives.Enabled = true;
                if (Global.OnlineSetup)
                {
                    try
                    {
                        ButtonRefreshAll0.Enabled = true;
                        //if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
                        ButtonRefreshAll.Enabled = true;
                    }
                    catch { }
                }
                else
                {
                    ButtonRefreshAll.Enabled = false;
                    ButtonRefreshAll0.Enabled = false;
                }
            }
            if (IsPostBack)
                return;
            GridView1.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");
            GridViewDrives.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");
            ASPxPageControl1.ActiveTabIndex = 0;
            Session["drivecounter"] = "0";

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            //SqlDataSourceAlarmSettings.SelectCommand = "SELECT * FROM [AlarmSettings],severity where alarmsettings.severity=severity.severityid";
            SqlCommand cmdcheck1 = new SqlCommand(SqlDataSourceDrives.SelectCommand, connection);
            Grid1.DataSource = cmdcheck1.ExecuteReader();
            Grid1.DataBind();

            Grid1.Height = (Grid1.RecordCount + 4) * Grid1.ItemHeight;

            SqlCommand cmdcheck11 = new SqlCommand(SqlDataSource2.SelectCommand, connection);
            Grid2.DataSource = cmdcheck11.ExecuteReader();
            Grid2.DataBind();

            Grid2.Height = (Grid2.RecordCount + 4) * Grid2.ItemHeight;

            SqlCommand cmdcheck111 = new SqlCommand(SqlDataSource3.SelectCommand, connection);
            Grid3.DataSource = cmdcheck111.ExecuteReader();
            Grid3.DataBind();

            Grid3.Height = (Grid2.RecordCount + 4) * Grid3.ItemHeight;

            SqlCommand command11 = new SqlCommand("Select mode from drivecurrentstate", connection);
            SqlDataReader reader11 = command11.ExecuteReader();
            reader11.Read();
            int Mode = int.Parse(reader11["mode"].ToString());
            reader11.Close();
            if (Mode == 1)  //single drive speed per drive
            {
                LabelProductLength.Visible = false;
                LabelPlanerSpeed.Visible = false;
                LabelProductLength1.Visible = false;
                LabelPlanerSpeed1.Visible = false;
            }
            reader11.Close();
            connection.Close();
        }

        protected void Button5_Click(object sender, System.EventArgs e)
        {
            // panChanges.Visible = true;
            //panChanges1.Visible = true;
            Button10.Visible = false;
            Button7.Visible = false;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            int index;//= Grid1.SelectedItemIndex;

            string s = string.Empty;
            if (Grid1.ChangedItems.Length == 0)
            {
                s += "No alarm attributes changed.";
                s += "<br />";
            }
            if (Grid1.ChangedItems.Length > 0)
            {
                foreach (EO.Web.GridItem item in Grid1.ChangedItems)
                {
                    foreach (EO.Web.GridCell cell in item.Cells)
                    {
                        if (cell.Modified)
                        {
                            if (cell.Column.DataField == "Command")
                            {
                                SqlCommand cmd = new SqlCommand("update drives set command=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "L1")
                            {
                                SqlCommand cmd = new SqlCommand("update drives set Length1Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }

                            index = Convert.ToInt16(item.Key);
                            string text = string.Format(
                                "Cell Changed: Drive = {0}, Field = {1}, New Value = {2}",
                                item.Key,
                                cell.Column.HeaderText,
                                cell.Value);

                            s += text;
                            s += "<br />";

                            if (CurrentUser.Access != 1)
                            {
                                return;
                            }
                            int LengthID;
                            int Type, MasterLink = 0;
                            bool Slave = false, Master1 = false, Independent = false, Lineal = false, Transverse = false, Lugged = false, Custom = false;

                            SqlCommand command = new SqlCommand("Select numlengths from websortsetup", connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            int NumLengths = int.Parse(reader["numlengths"].ToString());
                            reader.Close();

                            SqlCommand command1111 = new SqlCommand("Select * from drivesettings where driveid=" + index, connection);
                            SqlDataReader reader1111 = command1111.ExecuteReader();
                            reader1111.Read();

                            SqlCommand cmdmultipliermode = new SqlCommand("select multipliermode from drivecurrentstate", connection);
                            SqlDataReader readermultipliermode = cmdmultipliermode.ExecuteReader();
                            readermultipliermode.Read();
                            /*if (readermultipliermode["multipliermode"].ToString() == "1")
                            {
                                Type = -1;
                                Independent = true;
                            }
                            else*/
                            {
                                if (reader1111["type"].ToString() == "-1")  //stand alone
                                {
                                    Independent = true;
                                    Type = -1;
                                }
                                else if (reader1111["type"].ToString() == "0")  //master
                                {
                                    Master1 = true;
                                    Type = 0;
                                }
                                else  //slave
                                {
                                    Slave = true;
                                    MasterLink = Convert.ToInt32(reader1111["type"].ToString());
                                    Type = Convert.ToInt32(reader1111["type"].ToString());
                                }
                            }
                            readermultipliermode.Close();
                            SqlCommand command12 = new SqlCommand("Select configuration from drivesettings where driveid=" + index, connection);
                            SqlDataReader reader12 = command12.ExecuteReader();
                            reader12.Read();
                            if (reader12["configuration"].ToString() == "0")  //lineal
                            {
                                Lineal = true;
                            }
                            else if (reader12["configuration"].ToString() == "1")  //transverse
                            {
                                Transverse = true;
                            }
                            else if (reader12["configuration"].ToString() == "2") //lugged
                            {
                                Lugged = true;
                            }
                            else
                                Custom = true;
                            reader12.Close();

                            reader1111.Close();

                            SqlCommand command111 = new SqlCommand("Select mode from drivecurrentstate", connection);
                            SqlDataReader reader111 = command111.ExecuteReader();
                            reader111.Read();
                            int Mode = int.Parse(reader111["mode"].ToString());
                            reader111.Close();
                            if (Mode == 1)  //single drive speed per drive
                                LengthID = 1;
                            else
                            {
                                SqlCommand cmdlength = new SqlCommand("select lengthid from lengths where lengthnominal=(select productlength*12 from drivecurrentstate)", connection);
                                SqlDataReader readerlength = cmdlength.ExecuteReader();
                                readerlength.Read();
                                LengthID = int.Parse(readerlength["lengthid"].ToString());
                                readerlength.Close();
                            }
                            //send to PLC
                            //if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
                            {
                                if (Global.OnlineSetup)
                                {
                                    SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1024", connection);
                                    cmd0.ExecuteNonQuery();
                                    SqlCommand cmd = new SqlCommand("insert into datarequestsdrive select getdate()," + index + "," + Grid1.Items[index - 1].Cells[1 + LengthID].Value.ToString() + ",0," + MasterLink + ",maxspeed,gearingactual," + Grid1.Items[index - 1].Cells[3 + LengthID].Value.ToString() + ",'" + Slave + "','" + Master1 + "','" + Independent + "','" + Lineal + "','" + Transverse + "','" + Lugged + "','" + Custom + "',1,0 from drivesettings where driveid=" + index + " select id=(select max(id) from datarequestsdrive with(NOLOCK))", connection);
                                    SqlDataReader reader1 = cmd.ExecuteReader();
                                    reader1.Read();
                                    // SqlCommand cmdx = new SqlCommand("insert into RaptorCommLog select getdate(),'" + cmd.CommandText.Replace("'", "''") + "," + LengthID.ToString() + "'", connection);
                                    //cmdx.ExecuteNonQuery();

                                    //make sure message is processed
                                    bool succeeded = Raptor.MessageAckConfirm("datarequestsdrive", int.Parse(reader1["id"].ToString()));
                                    if (!succeeded)
                                    {
                                        LabelPLCTimeout.Visible = true;
                                        return;
                                    }
                                    SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024", connection);
                                    cmd1.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }

            Literal info = new Literal();
            info.Text = s;
            panChanges.Controls.Add(info);
            panChanges1.Controls.Add(info);

            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = true;

            Button10.Visible = false;
            Button7.Visible = false;

            //SqlDataSourceAlarmSettings.SelectCommand = "SELECT * FROM [AlarmSettings],severity where alarmsettings.severity=severity.severityid " + SortBy;
            SqlCommand cmdcheck1 = new SqlCommand(SqlDataSourceDrives.SelectCommand, connection);
            Grid1.DataSource = cmdcheck1.ExecuteReader();
            Grid1.DataBind();

            Grid1.Height = (Grid1.RecordCount + 4) * Grid1.ItemHeight;

            connection.Close();
            //Timer2.Interval = 1;
            //Timer2.Enabled = true;
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
        }

        protected void Grid1_ItemCommand(object sender, EO.Web.GridCommandEventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = false;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            //SqlDataSourceAlarmSettings.SelectCommand = "SELECT * FROM [AlarmSettings],severity where alarmsettings.severity=severity.severityid " + SortBy;

            SqlCommand cmdcheck1 = new SqlCommand(SqlDataSourceDrives.SelectCommand, connection);
            Grid1.DataSource = cmdcheck1.ExecuteReader();
            Grid1.DataBind();

            Grid1.Height = (Grid1.RecordCount + 4) * Grid1.ItemHeight;
            int index = (int)e.Item.Cells[0].Value;

            Button10.Visible = true;
            Button7.Visible = true;
            UpdatePanel7.Update();
            connection.Close();
        }

        protected void Grid2_ItemCommand(object sender, EO.Web.GridCommandEventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = false;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand cmdcheck1 = new SqlCommand("select * from [2inchspeeds] order by driveid", connection);
            Grid2.DataSource = cmdcheck1.ExecuteReader();
            Grid2.DataBind();

            Grid2.Height = (Grid2.RecordCount + 4) * Grid2.ItemHeight;
            int index = (int)e.Item.Cells[0].Value;

            connection.Close();
        }

        protected void Grid3_ItemCommand(object sender, EO.Web.GridCommandEventArgs e)
        {
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

            SqlCommand command = new SqlCommand("Select distinct RecipeLabel,recipeid,online,editing From Recipes order by RecipeLabel", connection);
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

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand command = new SqlCommand("update Recipes set editing=0 update recipes set editing=1 where recipeid=" + TreeView1.SelectedNode.Value, connection);
            command.ExecuteNonQuery();
            connection.Close();
            //GridViewDrives.DataBind();
            UpdatePanel7.Update();
            if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
            {
                Timer2.Enabled = true;
                if (Global.OnlineSetup && CurrentUser.Access == 1)
                    ButtonRefreshAll.Enabled = true;
            }
            else
            {
                Timer2.Enabled = false;
                ButtonRefreshAll.Enabled = false;
            }
            Session["selectednode"] = TreeView1.SelectedNode.Text;
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = false;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#EEFFAA'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    DataRowView row = (DataRowView)e.Row.DataItem;
                    Raptor cs1 = new Raptor();
                    string connectionString = Global.ConnectionString;
                    System.Data.SqlClient.SqlConnection connection;
                    connection = new SqlConnection(connectionString);
                    // Open the connection.
                    connection.Open();
                    SqlCommand cmdmultipliermode = new SqlCommand("select multipliermode from drivecurrentstate", connection);
                    SqlDataReader readermultipliermode = cmdmultipliermode.ExecuteReader();
                    readermultipliermode.Read();
                    //if (readermultipliermode["multipliermode"].ToString() == "0")
                    {
                        DropDownList dd = (DropDownList)e.Row.FindControl("DropDownList1");

                        dd.Items.Clear();
                        dd.Items.Add("None");
                        dd.Items[0].Value = "0";
                        dd.DataBind();
                        RadioButtonList chk = (RadioButtonList)e.Row.FindControl("RadioButtonList1");
                        if ((int)row["Type"] == -1)  //stand alone
                            chk.SelectedIndex = 0;
                        else if ((int)row["Type"] == 0)  //master
                            chk.SelectedIndex = 1;
                        else  //slave
                        {
                            chk.SelectedIndex = 2;
                            dd.SelectedValue = row["Type"].ToString();
                            //dd.Visible = true;
                        }
                    }
                    readermultipliermode.Close();

                    RadioButtonList chk1 = (RadioButtonList)e.Row.FindControl("RadioButtonList2");

                    if ((int)row["Configuration"] == 0) //lineal
                        chk1.SelectedIndex = 0;
                    else if ((int)row["Configuration"] == 1)  //transverse
                        chk1.SelectedIndex = 1;
                    else if ((int)row["Configuration"] == 2)  //lugged
                        chk1.SelectedIndex = 2;
                    else   //custom
                        chk1.SelectedIndex = 3;

                    connection.Close();
                }
                else
                {
                    DataRowView row = (DataRowView)e.Row.DataItem;
                    Raptor cs1 = new Raptor();
                    string connectionString = Global.ConnectionString;
                    System.Data.SqlClient.SqlConnection connection;
                    connection = new SqlConnection(connectionString);
                    // Open the connection.
                    connection.Open();

                    SqlCommand cmdmultipliermode = new SqlCommand("select multipliermode from drivecurrentstate", connection);
                    SqlDataReader readermultipliermode = cmdmultipliermode.ExecuteReader();
                    readermultipliermode.Read();
                    // if (readermultipliermode["multipliermode"].ToString() == "0")
                    {
                        RadioButtonList chk = (RadioButtonList)e.Row.FindControl("RadioButtonList1");
                        RadioButtonList chk1 = (RadioButtonList)e.Row.FindControl("RadioButtonList2");
                        Label lb = (Label)e.Row.FindControl("Label1");

                        lb.Visible = false;
                        if ((int)row["Type"] == -1)  //stand alone
                            chk.SelectedIndex = 0;
                        else if ((int)row["Type"] == 0)  //master
                            chk.SelectedIndex = 1;
                        else  //slave
                        {
                            chk.SelectedIndex = 2;

                            SqlCommand command = new SqlCommand("Select drivelabel from drivesettings where driveid = " + (int)row["Type"], connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            lb.Text = reader["drivelabel"].ToString();
                            connection.Close();
                            lb.Visible = true;
                        }
                        if ((int)row["Configuration"] == 0)  //lineal
                            chk1.SelectedIndex = 0;
                        else if ((int)row["Configuration"] == 1)  //transverse
                            chk1.SelectedIndex = 1;
                        else if ((int)row["Configuration"] == 2) //lugged
                            chk1.SelectedIndex = 2;
                        else  //custom
                            chk1.SelectedIndex = 3;
                    }
                    /*else
                    {
                        RadioButtonList chk1 = (RadioButtonList)e.Row.FindControl("RadioButtonList2");

                        if ((int)row["Configuration"] == 0)  //lineal
                            chk1.SelectedIndex = 0;
                        else if ((int)row["Configuration"] == 1)  //transverse
                            chk1.SelectedIndex = 1;
                        else if ((int)row["Configuration"] == 2) //lugged
                            chk1.SelectedIndex = 2;
                        else  //custom
                            chk1.SelectedIndex = 3;
                    }*/
                    readermultipliermode.Close();
                    connection.Close();
                }
            }
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (CurrentUser.Access != 1)
            {
                e.Cancel = true; // stops the automatic update command from happening
                return;
            }
            int index = e.RowIndex, Type, Configuration, MasterLink = 0;
            bool Slave = false, Master = false, Independent = false, Lineal = false, Transverse = false, Lugged = false, Custom = false;
            GridViewRow row = GridView1.Rows[index];
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand cmdmultipliermode = new SqlCommand("select multipliermode from drivecurrentstate", connection);
            SqlDataReader readermultipliermode = cmdmultipliermode.ExecuteReader();
            readermultipliermode.Read();
            /*if (readermultipliermode["multipliermode"].ToString() == "1")
            {
                Type = -1;
                Independent = true;
                SqlDataSource1.UpdateParameters["Type"].DefaultValue = "-1";
            }
            else*/
            {
                RadioButtonList chk = (RadioButtonList)row.FindControl("RadioButtonList1");
                DropDownList dd = (DropDownList)row.FindControl("DropDownList1");
                if (chk.SelectedIndex == 0)  //stand alone
                {
                    Independent = true;
                    Type = -1;
                }
                else if (chk.SelectedIndex == 1)  //master
                {
                    Master = true;
                    Type = 0;
                }
                else  //slave
                {
                    Slave = true;
                    MasterLink = Convert.ToInt32(dd.SelectedValue);
                    Type = Convert.ToInt32(dd.SelectedValue);
                }
            }
            readermultipliermode.Close();
            RadioButtonList chk1 = (RadioButtonList)row.FindControl("RadioButtonList2");
            if (chk1.SelectedIndex == 0)  //lineal
            {
                Lineal = true;
                Configuration = 0;
            }
            else if (chk1.SelectedIndex == 1)  //transverse
            {
                Transverse = true;
                Configuration = 1;
            }
            else if (chk1.SelectedIndex == 2) //lugged
            {
                Lugged = true;
                Configuration = 2;
            }
            else
            {
                Custom = true;
                Configuration = 3;
            }
            SqlDataSource1.UpdateParameters["Configuration"].DefaultValue = Configuration.ToString();
            SqlDataSource1.UpdateParameters["Type"].DefaultValue = Type.ToString();
            //send to PLC
            if (Global.OnlineSetup)
            {
                int LengthID;
                SqlCommand command11 = new SqlCommand("Select mode from drivecurrentstate", connection);
                SqlDataReader reader11 = command11.ExecuteReader();
                reader11.Read();
                int Mode = int.Parse(reader11["mode"].ToString());
                reader11.Close();
                if (Mode == 1)  //single drive speed per drive
                    LengthID = 1;
                else
                {
                    SqlCommand cmdlength = new SqlCommand("select lengthid from lengths where lengthnominal=(select productlength*12 from drivecurrentstate)", connection);
                    SqlDataReader readerlength = cmdlength.ExecuteReader();
                    readerlength.Read();
                    LengthID = int.Parse(readerlength["lengthid"].ToString());
                    readerlength.Close();
                }

                SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1024", connection);
                cmd0.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand("insert into datarequestsdrive select getdate()," + row.Cells[1].Text + ",command,0," + MasterLink + "," + ((TextBox)row.Cells[5].Controls[0]).Text + "," + ((TextBox)row.Cells[6].Controls[0]).Text + ",Length" + LengthID + "Multiplier,'" + Slave + "','" + Master + "','" + Independent + "','" + Lineal + "','" + Transverse + "','" + Lugged + "','" + Custom + "',1,0 from drives where driveid=" + row.Cells[1].Text + "  select id=(select max(id) from datarequestsdrive with(NOLOCK))", connection);
                SqlDataReader reader1 = cmd.ExecuteReader();
                reader1.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsdrive", int.Parse(reader1["id"].ToString()));
                if (!succeeded)
                {
                    LabelPLCTimeout.Visible = true;
                    return;
                }
                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024", connection);
                cmd1.ExecuteNonQuery();
                connection.Close();
            }
            connection.Close();
        }

        protected void GridViewDrives_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int CurrentLengthID = 0;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand command = new SqlCommand("Select numlengths=count(*) from lengths where petflag=0 and lengthid>0", connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int NumLengths = int.Parse(reader["numlengths"].ToString());
            reader.Close();
            SqlCommand command20 = new SqlCommand("Select lengthid from lengths where lengthnominal=(select productlength*12 from drivecurrentstate)", connection);
            SqlDataReader reader20 = command20.ExecuteReader();
            reader20.Read();
            if (reader20.HasRows)
                CurrentLengthID = int.Parse(reader20["lengthid"].ToString());
            reader20.Close();
            SqlCommand command11 = new SqlCommand("Select mode from drivecurrentstate", connection);
            SqlDataReader reader11 = command11.ExecuteReader();
            reader11.Read();
            int Mode = int.Parse(reader11["mode"].ToString());
            reader11.Close();
            if (Mode == 1)  //single drive speed per drive
            {
                NumLengths = 1;
                CurrentLengthID = 0;
                LabelProductLength.Visible = false;
                LabelPlanerSpeed.Visible = false;
                LabelProductLength1.Visible = false;
                LabelPlanerSpeed1.Visible = false;
            }
            if (e.Row.RowType == DataControlRowType.Header)
            {
                int i = 6;
                DataRowView row = (DataRowView)e.Row.DataItem;
                if (Mode == 0)
                    for (int j = 1; j <= NumLengths; j++)
                    {
                        SqlCommand command1 = new SqlCommand("Select lengthlabel from lengths where lengthid=" + j, connection);
                        SqlDataReader reader1 = command1.ExecuteReader();
                        reader1.Read();
                        e.Row.Cells[i].Text = reader1["lengthlabel"].ToString();
                        i++;
                        reader1.Close();
                    }
                else
                    e.Row.Cells[i].Text = "Multiplier";

                for (int j = NumLengths + 6; j < e.Row.Cells.Count; j++)
                    e.Row.Cells[j].Visible = false;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#EEFFAA'");
                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");
                }
                //if ((e.Row.RowState & DataControlRowState.Edit) == 0)
                {
                    DataRowView row = (DataRowView)e.Row.DataItem;
                    RadioButtonList chk = (RadioButtonList)e.Row.FindControl("RadioButtonList1");
                    Label lb = (Label)e.Row.FindControl("Label1");

                    if (CurrentLengthID > 0)
                    {
                        e.Row.Cells[CurrentLengthID + 5].BackColor = System.Drawing.Color.FromArgb(0x990000);
                        e.Row.Cells[CurrentLengthID + 5].ForeColor = System.Drawing.Color.White;
                    }
                    lb.Visible = false;
                    if ((int)row["Type"] == -1)
                        chk.SelectedIndex = 0;
                    else if ((int)row["Type"] == 0)
                        chk.SelectedIndex = 1;
                    else
                    {
                        chk.SelectedIndex = 2;
                        SqlCommand command2 = new SqlCommand("Select drivelabel from drivesettings where driveid = " + (int)row["Type"], connection);
                        SqlDataReader reader2 = command2.ExecuteReader();
                        reader2.Read();
                        lb.Text = reader2["drivelabel"].ToString();
                        reader2.Close();
                        lb.Visible = true;
                    }
                }
                for (int j = NumLengths + 6; j < e.Row.Cells.Count; j++)
                    e.Row.Cells[j].Visible = false;
            }
            connection.Close();
        }

        protected void ASPxPageControl1_ActiveTabChanged(object source, DevExpress.Web.ASPxTabControl.TabControlEventArgs e)
        {
            //When flipping between tabs, the treeview seems to lose which node is selected, let's remind it.
            TreeNodeCollection childnodes = TreeView1.Nodes[0].ChildNodes;
            try
            {
                foreach (TreeNode n in childnodes)
                    if (n.Text == Session["selectednode"].ToString())
                        n.Select();
            }
            catch { }
            //GridViewDrives.DataBind();
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            //SqlDataSourceAlarmSettings.SelectCommand = "SELECT * FROM [AlarmSettings],severity where alarmsettings.severity=severity.severityid";
            SqlCommand cmdcheck1 = new SqlCommand(SqlDataSourceDrives.SelectCommand, connection);
            Grid1.DataSource = cmdcheck1.ExecuteReader();
            Grid1.DataBind();
            SqlCommand cmdcheck11 = new SqlCommand(SqlDataSource2.SelectCommand, connection);
            Grid2.DataSource = cmdcheck11.ExecuteReader();
            Grid2.DataBind();
            SqlCommand cmdcheck111 = new SqlCommand(SqlDataSource3.SelectCommand, connection);
            Grid3.DataSource = cmdcheck111.ExecuteReader();
            Grid3.DataBind();
            connection.Close();

            if (e.Tab.Index == 0)
            {
                Timer2.Enabled = true;
                Timer3.Enabled = true;
            }
            else
            {
                Timer2.Enabled = false;
                Timer3.Enabled = false;
            }
        }

        protected void GridViewDrives_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (CurrentUser.Access != 1)
            {
                e.Cancel = true; // stops the automatic update command from happening
                return;
            }
            int index = e.RowIndex;
            int LengthID;
            int Type, MasterLink = 0;
            bool Slave = false, Master = false, Independent = false, Lineal = false, Transverse = false, Lugged = false, Custom = false;
            string L1 = "0", L2 = "0", L3 = "0", L4 = "0", L5 = "0", L6 = "0", L7 = "0", L8 = "0", L9 = "0", L10 = "0";
            GridViewRow row = GridViewDrives.Rows[index];

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand command = new SqlCommand("Select numlengths from websortsetup", connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int NumLengths = int.Parse(reader["numlengths"].ToString());
            reader.Close();

            SqlCommand cmdmultipliermode = new SqlCommand("select multipliermode from drivecurrentstate", connection);
            SqlDataReader readermultipliermode = cmdmultipliermode.ExecuteReader();
            readermultipliermode.Read();
            if (readermultipliermode["multipliermode"].ToString() == "1")
            {
                Type = -1;
                Independent = true;
            }
            else
            {
                RadioButtonList chk = (RadioButtonList)row.FindControl("RadioButtonList1");
                //RadioButtonList chk1 = (RadioButtonList)row.FindControl("RadioButtonList2");

                if (chk.SelectedIndex == 0)  //stand alone
                {
                    Independent = true;
                    Type = -1;
                }
                else if (chk.SelectedIndex == 1)  //master
                {
                    Master = true;
                    Type = 0;
                }
                else  //slave
                {
                    Label lb = (Label)row.FindControl("Label1");
                    SqlCommand command11 = new SqlCommand("Select driveid from drivesettings where drivelabel='" + lb.Text + "'", connection);
                    SqlDataReader reader11 = command11.ExecuteReader();
                    reader11.Read();
                    Slave = true;
                    MasterLink = Convert.ToInt32(reader11["driveid"].ToString());
                    Type = Convert.ToInt32(reader11["driveid"].ToString());
                    reader11.Close();
                }
            }
            readermultipliermode.Close();
            SqlCommand command12 = new SqlCommand("Select configuration from drivesettings where driveid=" + row.Cells[1].Text, connection);
            SqlDataReader reader12 = command12.ExecuteReader();
            reader12.Read();
            if (reader12["configuration"].ToString() == "0")  //lineal
            {
                Lineal = true;
            }
            else if (reader12["configuration"].ToString() == "1")  //transverse
            {
                Transverse = true;
            }
            else if (reader12["configuration"].ToString() == "2") //lugged
            {
                Lugged = true;
            }
            else
                Custom = true;
            reader12.Close();

            L1 = ((TextBox)row.Cells[6].Controls[0]).Text;
            if (NumLengths > 1)
                L2 = ((TextBox)row.Cells[7].Controls[0]).Text;
            if (NumLengths > 2)
                L3 = ((TextBox)row.Cells[8].Controls[0]).Text;
            if (NumLengths > 3)
                L4 = ((TextBox)row.Cells[9].Controls[0]).Text;
            if (NumLengths > 4)
                L5 = ((TextBox)row.Cells[10].Controls[0]).Text;
            if (NumLengths > 5)
                L6 = ((TextBox)row.Cells[11].Controls[0]).Text;
            if (NumLengths > 6)
                L7 = ((TextBox)row.Cells[12].Controls[0]).Text;
            if (NumLengths > 7)
                L8 = ((TextBox)row.Cells[13].Controls[0]).Text;
            if (NumLengths > 8)
                L9 = ((TextBox)row.Cells[14].Controls[0]).Text;
            if (NumLengths > 9)
                L10 = ((TextBox)row.Cells[15].Controls[0]).Text;

            SqlCommand command111 = new SqlCommand("Select mode from drivecurrentstate", connection);
            SqlDataReader reader111 = command111.ExecuteReader();
            reader111.Read();
            int Mode = int.Parse(reader111["mode"].ToString());
            reader111.Close();
            if (Mode == 1)  //single drive speed per drive
                LengthID = 1;
            else
            {
                SqlCommand cmdlength = new SqlCommand("select lengthid from lengths where lengthnominal=(select productlength*12 from drivecurrentstate)", connection);
                SqlDataReader readerlength = cmdlength.ExecuteReader();
                readerlength.Read();
                LengthID = int.Parse(readerlength["lengthid"].ToString());
                readerlength.Close();
            }
            //send to PLC
            //if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
            {
                if (Global.OnlineSetup)
                {
                    SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1024", connection);
                    cmd0.ExecuteNonQuery();
                    SqlCommand cmd = new SqlCommand("insert into datarequestsdrive select getdate()," + row.Cells[1].Text + "," + ((TextBox)row.Cells[3 + LengthID].Controls[0]).Text + ",0," + MasterLink + ",maxspeed,gearingactual," + ((TextBox)row.Cells[5 + LengthID].Controls[0]).Text + ",'" + Slave + "','" + Master + "','" + Independent + "','" + Lineal + "','" + Transverse + "','" + Lugged + "','" + Custom + "',1,0 from drivesettings where driveid=" + row.Cells[1].Text + " select id=(select max(id) from datarequestsdrive with(NOLOCK))", connection);
                    SqlDataReader reader1 = cmd.ExecuteReader();
                    reader1.Read();

                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequestsdrive", int.Parse(reader1["id"].ToString()));
                    if (!succeeded)
                    {
                        LabelPLCTimeout.Visible = true;
                        e.Cancel = true; // stops the automatic update command from happening
                        return;
                    }
                    SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024", connection);
                    cmd1.ExecuteNonQuery();
                }
            }
            SqlDataSourceDrives.UpdateCommand = "update drives set command=" + ((TextBox)row.Cells[3 + LengthID].Controls[0]).Text + ",Length1Multiplier=" + L1 + ",Length2Multiplier=" + L2 + ",Length3Multiplier=" + L3 + ",Length4Multiplier=" + L4 + ",Length5Multiplier=" + L5 + ",Length6Multiplier=" + L6 + ",Length7Multiplier=" + L7 + ",Length8Multiplier=" + L8 + ",Length9Multiplier=" + L9 + ",Length10Multiplier=" + L10 + " where driveid=" + GridViewDrives.Rows[e.RowIndex].Cells[1].Text;
            connection.Close();
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            int i = Convert.ToInt32(Session["drivecounter"]);
            Timer2.Interval = 500;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            //if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
            if (Global.OnlineSetup)
            {
                LabelPLCTimeout.Visible = false;
                SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1024", connection);
                cmd0.ExecuteNonQuery();
                // for (int i = 0; i < 2; i++)
                //for (int i = 0; i < GridViewDrives.Rows.Count; i++)
                {
                    SqlCommand cmd = new SqlCommand("insert into datarequestsdrive select getdate()," + GridViewDrives.Rows[i].Cells[1].Text + ",0,0,0,0,0,0,0,0,0,0,0,0,0,0,0  select id=(select max(id) from datarequestsdrive with(NOLOCK))", connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequestsdrive", int.Parse(reader["id"].ToString()));
                    reader.Close();
                    if (!succeeded)
                    {
                        LabelPLCTimeout.Visible = true;
                        //break;
                    }
                }
                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024", connection);
                cmd1.ExecuteNonQuery();
                //GridViewDrives.DataBind();
                Session["drivecounter"] = (Convert.ToInt32(Session["drivecounter"]) + 1).ToString();
                if (Convert.ToInt32(Session["drivecounter"]) >= GridViewDrives.Rows.Count)
                    Session["drivecounter"] = "0";
            }
            //GridViewDrives.DataBind();
            SqlCommand cmdcheck1 = new SqlCommand(SqlDataSourceDrives.SelectCommand, connection);
            Grid1.DataSource = cmdcheck1.ExecuteReader();
            Grid1.DataBind();
            connection.Close();
        }

        protected void GridViewDrives_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            //if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
            {
                Timer2.Interval = 1;
                Timer2.Enabled = true;
            }
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = true;
        }

        protected void GridViewDrives_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Timer2.Enabled = false;
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = false;
        }

        protected void GridViewDrives_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            Timer2.Interval = 1;
            Timer2.Enabled = true;
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = true;
        }

        protected void Timer3_Tick(object sender, EventArgs e)
        {
            if (LabelPlanerSpeed.Visible == false)
            {
                Timer3.Enabled = false;
                return;
            }
            Timer3.Interval = 2000;
            try
            {
                //update length and planer speed
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand command = new SqlCommand("Select * from drivecurrentstate", connection);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                LabelPlanerSpeed1.Text = reader["planerspeed"].ToString();
                LabelProductLength1.Text = reader["productlength"].ToString();
                reader.Close();
                connection.Close();
            }
            catch { }
        }

        protected void ButtonRefreshAll0_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            LabelPLCTimeout.Visible = false;
            SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1024", connection);
            cmd0.ExecuteNonQuery();
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestsdrive select getdate()," + GridView1.Rows[i].Cells[1].Text + ",0,0,0,0,0,0,0,0,0,0,0,0,0,0  select id=(select max(id) from datarequestsdrive with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsdrive", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelPLCTimeout.Visible = true;
                    break;
                }
            }
            SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024", connection);
            cmd1.ExecuteNonQuery();
            GridView1.DataBind();

            connection.Close();
        }

        protected void SqlDataSource1_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
        }

        protected void ButtonRefreshAll_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            LabelPLCTimeout.Visible = false;
            SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1024", connection);
            cmd0.ExecuteNonQuery();
            for (int i = 0; i < GridViewDrives.Rows.Count; i++)
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestsdrive select getdate()," + GridViewDrives.Rows[i].Cells[1].Text + ",0,0,0,0,0,0,0,0,0,0,0,0,0,0,0  select id=(select max(id) from datarequestsdrive with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsdrive", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelPLCTimeout.Visible = true;
                    break;
                }
            }
            SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024", connection);
            cmd1.ExecuteNonQuery();
            // GridViewDrives.DataBind();

            connection.Close();
        }

        protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = true;
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = true;
        }

        protected void Grid1_ItemChanged(object sender, EO.Web.GridItemEventArgs e)
        {
            Timer2.Enabled = false;
        }

        protected void Grid2_ItemChanged(object sender, EO.Web.GridItemEventArgs e)
        {
            //Timer2.Enabled = false;
        }

        protected void Button14_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            int index;//= Grid1.SelectedItemIndex;

            string s = string.Empty;
            if (Grid2.ChangedItems.Length == 0)
            {
                s += "No attributes changed.";
                s += "<br />";
            }
            if (Grid2.ChangedItems.Length > 0)
            {
                foreach (EO.Web.GridItem item in Grid2.ChangedItems)
                {
                    foreach (EO.Web.GridCell cell in item.Cells)
                    {
                        if (cell.Modified)
                        {
                            if (cell.Column.DataField == "DriveLabel")
                            {
                                SqlCommand cmd = new SqlCommand("update [2inchspeeds] set drivelabel='" + cell.Value.ToString() + "' where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                                SqlCommand cmd1 = new SqlCommand("update [4inchspeeds] set drivelabel='" + cell.Value.ToString() + "' where driveid=" + item.Key.ToString(), connection);
                                cmd1.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width4Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [2inchspeeds] set Width4Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width6Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [2inchspeeds] set Width6Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width8Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [2inchspeeds] set Width8Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width10Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [2inchspeeds] set Width10Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width12Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [2inchspeeds] set Width12Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }

                            index = Convert.ToInt16(item.Key);
                            string text = string.Format(
                                "Cell Changed: Drive = {0}, Field = {1}, New Value = {2}",
                                item.Key,
                                cell.Column.HeaderText,
                                cell.Value);

                            s += text;
                            s += "<br />";
                        }
                    }
                }
            }
            if (Grid3.ChangedItems.Length > 0)
            {
                foreach (EO.Web.GridItem item in Grid3.ChangedItems)
                {
                    foreach (EO.Web.GridCell cell in item.Cells)
                    {
                        if (cell.Modified)
                        {
                            if (cell.Column.DataField == "DriveLabel")
                            {
                                SqlCommand cmd = new SqlCommand("update [2inchspeeds] set drivelabel='=" + cell.Value.ToString() + "' where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                                SqlCommand cmd1 = new SqlCommand("update [4inchspeeds] set drivelabel='=" + cell.Value.ToString() + "' where driveid=" + item.Key.ToString(), connection);
                                cmd1.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width4Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [4inchspeeds] set Width4Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width6Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [4inchspeeds] set Width6Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width8Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [4inchspeeds] set Width8Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width10Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [4inchspeeds] set Width10Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            if (cell.Column.DataField == "Width12Multiplier")
                            {
                                SqlCommand cmd = new SqlCommand("update [4inchspeeds] set Width12Multiplier=" + cell.Value.ToString() + " where driveid=" + item.Key.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }

                            index = Convert.ToInt16(item.Key);
                            string text = string.Format(
                                "Cell Changed: Drive = {0}, Field = {1}, New Value = {2}",
                                item.Key,
                                cell.Column.HeaderText,
                                cell.Value);

                            s += text;
                            s += "<br />";
                        }
                    }
                }
            }

            //send to PLC
            //if (TreeView1.SelectedNode.Text.Contains("ACTIVE"))
            {
                if (Global.OnlineSetup)
                {
                    SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 33554432", connection);
                    cmd0.ExecuteNonQuery();
                }
            }

            Literal info = new Literal();
            info.Text = s;
            panChanges2.Controls.Add(info);
            panChanges3.Controls.Add(info);

            System.Web.UI.Timer temptimer = (System.Web.UI.Timer)Master.FindControl("TimerHeader");
            temptimer.Enabled = true;

            SqlCommand cmdcheck1 = new SqlCommand(SqlDataSource2.SelectCommand, connection);
            Grid2.DataSource = cmdcheck1.ExecuteReader();
            Grid2.DataBind();

            Grid2.Height = (Grid2.RecordCount + 4) * Grid2.ItemHeight;

            SqlCommand cmdcheck11 = new SqlCommand(SqlDataSource3.SelectCommand, connection);
            Grid3.DataSource = cmdcheck11.ExecuteReader();
            Grid3.DataBind();

            Grid3.Height = (Grid3.RecordCount + 4) * Grid3.ItemHeight;

            connection.Close();
        }

        protected void Button15_Click(object sender, EventArgs e)
        {
        }
    }
}