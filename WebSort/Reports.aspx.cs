using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class Reports : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Reports", User.Identity.Name);
            
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
            CrystalReportViewer1.ToolbarStyle.Width = new Unit("1100px");

            if (IsPostBack)
                return;
            SetCurrentShift();
            UpdateReportHeader();
            //RefreshReport();
            GetPrinters();

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from ReportSettings", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            TextBox1.Text = reader["Copies"].ToString();

            reader.Close();
            connection.Close();
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
        }

        private void RefreshReport()
        {
            try
            {
                if (TreeView1.SelectedNode.Depth == 3)
                {
                    Raptor cs1 = new Raptor();
                    string connectionString = Global.ConnectionString;
                    System.Data.SqlClient.SqlConnection connection;
                    connection = new SqlConnection(connectionString);
                    // Open the connection.
                    connection.Open();

                    SqlCommand command = new SqlCommand("update ReportHeader set printrecipelabel='" + TreeView1.SelectedNode.Text + "', printrecipeid=" + TreeView1.SelectedNode.Value, connection);
                    command.ExecuteNonQuery();

                    connection.Close();
                    CrystalReportSource1.Report.FileName = Server.MapPath(".") + "\\app_data\\Reports\\Recipe.rpt";
                }
                else if (TreeView1.SelectedNode.Text == "Active Recipe")
                {
                    Raptor cs1 = new Raptor();
                    string connectionString = Global.ConnectionString;
                    System.Data.SqlClient.SqlConnection connection;
                    connection = new SqlConnection(connectionString);
                    // Open the connection.
                    connection.Open();

                    SqlCommand command = new SqlCommand("update ReportHeader set printrecipelabel=(select recipelabel from recipes where online=1), printrecipeid=(select recipeid from recipes where online=1)", connection);
                    command.ExecuteNonQuery();

                    connection.Close();
                    CrystalReportSource1.Report.FileName = Server.MapPath(".") + "\\app_data\\Reports\\" + TreeView1.SelectedNode.Text + ".rpt";
                }
                else
                    CrystalReportSource1.Report.FileName = Server.MapPath(".") + "\\app_data\\Reports\\" + TreeView1.SelectedNode.Text + ".rpt";

                Raptor cs2 = new Raptor();
                string connectionString2 = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection2;
                connection2 = new SqlConnection(connectionString2);
                // Open the connection.
                connection2.Open();
                SqlCommand command1 = new SqlCommand("update ReportHeader set printreportname='" + TreeView1.SelectedNode.Text + ".rpt'", connection2);
                command1.ExecuteNonQuery();
                connection2.Close();
            }
            catch { }
        }

        private void SetCurrentShift()
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("update ReportHeader set shiftindexstart=(select max(shiftindex) from shifts),shiftindexend=(select max(shiftindex) from shifts),recipeid=0,recipelabel='<ALL>'", connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        private void SetSelectedShift()
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            try
            {
                SqlCommand command = new SqlCommand("update reportheader set recipeid = " + DropDownListRecipes.SelectedValue + ",recipelabel='" + DropDownListRecipes.SelectedItem.Text + "', shiftindexstart=" + RadioButtonListShiftStart.SelectedItem.Value + ", shiftindexend=" + RadioButtonListShiftEnd.SelectedItem.Value, connection);
                command.ExecuteNonQuery();
            }
            catch { }

            connection.Close();
        }

        private void UpdateReportHeader()
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("execute upReportHeader", connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        private void GetPrinters()
        {
            // Retrieve list of local printers
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbPrinters.Items.Add(printer.ToString());
            }
            // Use the ObjectQuery to get the list of network configured printers
            /*System.Management.ObjectQuery oquery =
                new System.Management.ObjectQuery("SELECT * FROM Win32_Printer");

            System.Management.ManagementObjectSearcher mosearcher =
                new System.Management.ManagementObjectSearcher(oquery);

            System.Management.ManagementObjectCollection moc = mosearcher.Get();

            foreach (ManagementObject mo in moc)
            {
                System.Management.PropertyDataCollection pdc = mo.Properties;
                foreach (System.Management.PropertyData pd in pdc)
                {
                    if ((bool)mo["Network"])
                    {
                        try{
                            cmbPrinters.Items.Add(mo[pd.Name].ToString());
                        }
                        catch{}
                    }
                }
            }*/

            // display selected printer
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("Select PrinterName from ReportSettings", connection);

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                try
                {
                    cmbPrinters.Items.Add(reader["PrinterName"].ToString());
                    cmbPrinters.Text = reader["PrinterName"].ToString();
                }
                catch
                { }
            }

            reader.Close();
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
                        FillReportCategories(e.Node);
                        break;

                    case 1:
                        FillReportNames(e.Node);
                        break;
                }
            }
            connection.Close();
        }

        private void FillReportCategories(TreeNode node)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("Select distinct CategoryName,categoryid From Reports order by CategoryID", connection);
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

        private void FillReportNames(TreeNode node)
        {
            string CategoryID = node.Value;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("Select ReportName, ReportID, automaticreport From Reports Where CategoryID=" + CategoryID.ToString(), connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet Reports = new DataSet();
            adapter.Fill(Reports);
            if (Reports.Tables.Count > 0)
            {
                foreach (DataRow row in Reports.Tables[0].Rows)
                {
                    TreeNode newNode = new TreeNode(row["ReportName"].ToString(), row["reportID"].ToString());
                    if (row["automaticreport"].ToString() == "1")
                        newNode.Checked = true;

                    newNode.PopulateOnDemand = false;
                    newNode.SelectAction = TreeNodeSelectAction.Select;

                    node.ChildNodes.Add(newNode);
                    if (newNode.Text == "Production Summary")
                    {
                        newNode.Select();
                        RefreshReport();
                    }
                    if (row["ReportName"].ToString() == "Recipes")
                    {
                        newNode.SelectAction = TreeNodeSelectAction.Expand;
                        newNode.Expanded = false;
                        SqlCommand command1 = new SqlCommand("Select RecipeLabel, RecipeID From Recipes order by recipelabel", connection);

                        SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                        DataSet Recipes = new DataSet();
                        adapter1.Fill(Recipes);
                        if (Recipes.Tables.Count > 0)
                        {
                            foreach (DataRow row1 in Recipes.Tables[0].Rows)
                            {
                                TreeNode newNode1 = new TreeNode(row1["RecipeLabel"].ToString(), row1["RecipeID"].ToString());

                                newNode1.PopulateOnDemand = false;
                                newNode1.SelectAction = TreeNodeSelectAction.Select;
                                newNode1.ShowCheckBox = false;
                                newNode.ChildNodes.Add(newNode1);
                            }
                        }
                    }
                }
            }
            connection.Close();
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            RefreshReport();
        }

        protected void TreeView1_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            int autochecked = 0;
            if (e.Node.Checked == true)
                autochecked = 1;
            SqlCommand command = new SqlCommand("update reports set automaticreport = " + autochecked.ToString() + "where reportid = " + e.Node.Value.ToString(), connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        protected void cmbPrinters_SelectedIndexChanged(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("update reportsettings set printername= '" + cmbPrinters.Text + "'", connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        protected void RadioButtonCurrentShift_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonCurrentShift.Checked == true)
            {
                SetCurrentShift();
                UpdateReportHeader();
                PanelShift.Visible = false;
                RefreshReport();
                DropDownListRecipes.Visible = false;
                LabelRecipe.Visible = false;
            }
            else
            {
                PanelShift.Visible = true;
                UpdateReportHeader();
                DropDownListRecipes.Visible = false;
                LabelRecipe.Visible = false;
            }
        }

        protected void RadioButtonSelectedShift_CheckedChanged(object sender, EventArgs e)
        {
            CalendarShiftStart.SelectedDate = CalendarShiftStart.TodaysDate;
            CalendarShiftEnd.SelectedDate = CalendarShiftEnd.TodaysDate;
            CalendarShiftStart_SelectionChanged(sender, e);
            CalendarShiftEnd_SelectionChanged(sender, e);
            if (RadioButtonSelectedShift.Checked == true)
            {
                UpdateReportHeader();
                PanelShift.Visible = true;
            }
            else
            {
                SetCurrentShift();
                UpdateReportHeader();
                PanelShift.Visible = false;
                RefreshReport();
            }
        }

        protected void CalendarShiftStart_SelectionChanged(object sender, EventArgs e)
        {
            string query;
            int dayindex = 0;
            DropDownListRecipes.Visible = true;
            LabelRecipe.Visible = true;
            DropDownListRecipes.DataBind();
            LabelStartShift.Visible = true;
            //LabelEndShift.Visible = true;
            RadioButtonListShiftStart.Items.Clear();
            //RadioButtonListShiftEnd.Items.Clear();
            // display shifts
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            //Start Shift
            if (CalendarShiftStart.SelectedDates.Count == 1)  //single day selected
                query = "Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "')";
            else  //week or month selected
            {
                for (int i = 0; i < CalendarShiftStart.SelectedDates.Count; i++)
                {
                    SqlCommand commandtemp = new SqlCommand("Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "')", connection);
                    SqlDataReader readertemp = commandtemp.ExecuteReader();
                    if (readertemp.HasRows)
                    {
                        dayindex = i;
                        readertemp.Close();
                        break;
                    }
                    readertemp.Close();
                }
                query = "Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "')";
            }
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                LabelShiftStartError.Visible = true;
                //LabelStartShift.Visible = false;
                //LabelEndShift.Visible = false;
                RadioButtonListShiftStart.Visible = false;
                //RadioButtonListShiftEnd.Visible = false;
                DropDownListRecipes.Visible = false;
                LabelRecipe.Visible = false;
                return;
            }
            else
            {
                LabelShiftStartError.Visible = false;
                //LabelStartShift.Visible = true;
                //LabelEndShift.Visible = true;
                RadioButtonListShiftStart.Visible = true;
                //RadioButtonListShiftEnd.Visible = true;
            }

            while (reader.Read())
            {
                RadioButtonListShiftStart.Items.Add(reader["shiftstart"].ToString());
                RadioButtonListShiftStart.Items[RadioButtonListShiftStart.Items.Count - 1].Value = reader["shiftindex"].ToString();
                RadioButtonListShiftStart.SelectedIndex = 0;
            }
            reader.Close();

            /*
            //End Shift
            if (CalendarShiftStart.SelectedDates.Count == 1)  //single day selected
                query = "Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "')";
            else  //week or month selected
            {
                for (int i = CalendarShiftStart.SelectedDates.Count - 1; i >= 0; i--)
                {
                    SqlCommand commandtemp = new SqlCommand("Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "')", connection);
                    SqlDataReader readertemp = commandtemp.ExecuteReader();
                    if (readertemp.HasRows)
                    {
                        dayindex = i;
                        readertemp.Close();
                        break;
                    }
                    readertemp.Close();
                }
                query = "Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "')";
            }

            SqlCommand command1 = new SqlCommand(query, connection);
            SqlDataReader reader1 = command1.ExecuteReader();

            if (!reader1.HasRows)
            {
                LabelShiftError.Visible = true;
                DropDownListRecipes.Visible = false;
                LabelRecipe.Visible = false;
                return;
            }
            else
                LabelShiftError.Visible = false;

            while (reader1.Read())
            {
                if (reader1["shiftend"].ToString() == "")
                    RadioButtonListShiftEnd.Items.Add("Current Shift (in progress)");
                else
                    RadioButtonListShiftEnd.Items.Add(reader1["shiftend"].ToString());
                RadioButtonListShiftEnd.Items[RadioButtonListShiftEnd.Items.Count - 1].Value = reader1["shiftindex"].ToString();
                RadioButtonListShiftEnd.SelectedIndex = 0;
            }
            reader1.Close();
            */

            SetSelectedShift();
            UpdateReportHeader();
            connection.Close();
            RefreshReport();
            //CalendarShiftEnd.SelectedDate = CalendarShiftStart.SelectedDate;
        }

        protected void RadioButtonListShiftStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*try
            {
                if (RadioButtonListShiftStart.SelectedIndex > RadioButtonListShiftEnd.SelectedIndex)
                    RadioButtonListShiftEnd.SelectedIndex = RadioButtonListShiftStart.SelectedIndex;
            }
            catch { }*/
            SetSelectedShift();
            UpdateReportHeader();
            RefreshReport();
        }

        protected void RadioButtonListShiftEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*try
            {
            if (RadioButtonListShiftEnd.SelectedIndex < RadioButtonListShiftStart.SelectedIndex)
                RadioButtonListShiftStart.SelectedIndex = RadioButtonListShiftEnd.SelectedIndex;
            }
            catch { }*/
            SetSelectedShift();
            UpdateReportHeader();
            RefreshReport();
        }

        protected void DropDownListRecipes_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSelectedShift();
            UpdateReportHeader();
            RefreshReport();
        }

        protected void ButtonPrintNow_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand commandtemp = new SqlCommand("Select c=count(*) from reports where automaticreport=1", connection);
            SqlDataReader readertemp = commandtemp.ExecuteReader();
            readertemp.Read();
            if (readertemp["c"].ToString() == "0")
            {
                Raptor msg = new Raptor();
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("There are no reports selected for end of shift printing."));
                connection.Close();
                return;
            }

            //flag the auto reporting service stored procedure to print out end of shift reports
            SqlCommand command0 = new SqlCommand("update ReportSettings set PrintEndOfShiftReports=2", connection);
            command0.ExecuteNonQuery();

            connection.Close();
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            //this.Validate("copies");
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            try
            {
                //flag the auto reporting service stored procedure to print out end of shift reports
                SqlCommand command0 = new SqlCommand("update ReportSettings set copies=" + TextBox1.Text, connection);
                command0.ExecuteNonQuery();
            }
            catch { }

            connection.Close();
        }

        protected void ButtonPrint_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            //flag the reporting service stored procedure to print a report
            SqlCommand command0 = new SqlCommand("update ReportSettings set PrintEndOfShiftReports=3", connection);
            command0.ExecuteNonQuery();

            connection.Close();
        }

        protected void CalendarShiftEnd_SelectionChanged(object sender, EventArgs e)
        {
            string query;
            int dayindex = 0;
            DropDownListRecipes.Visible = true;
            LabelRecipe.Visible = true;
            DropDownListRecipes.DataBind();
            //LabelStartShift.Visible = true;
            LabelEndShift.Visible = true;
            //RadioButtonListShiftStart.Items.Clear();
            RadioButtonListShiftEnd.Items.Clear();
            // display shifts
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            //Start Shift
            /*if (CalendarShiftStart.SelectedDates.Count == 1)  //single day selected
                query = "Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[0].Month + "/" + CalendarShiftStart.SelectedDates[0].Day + "/" + CalendarShiftStart.SelectedDates[0].Year + "')";
            else  //week or month selected
            {
                for (int i = 0; i < CalendarShiftStart.SelectedDates.Count; i++)
                {
                    SqlCommand commandtemp = new SqlCommand("Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[i].Month + "/" + CalendarShiftStart.SelectedDates[i].Day + "/" + CalendarShiftStart.SelectedDates[i].Year + "')", connection);
                    SqlDataReader readertemp = commandtemp.ExecuteReader();
                    if (readertemp.HasRows)
                    {
                        dayindex = i;
                        readertemp.Close();
                        break;
                    }
                    readertemp.Close();
                }
                query = "Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftStart.SelectedDates[dayindex].Month + "/" + CalendarShiftStart.SelectedDates[dayindex].Day + "/" + CalendarShiftStart.SelectedDates[dayindex].Year + "')";
            }
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                LabelShiftError.Visible = true;
                LabelStartShift.Visible = false;
                //LabelEndShift.Visible = false;
                RadioButtonListShiftStart.Visible = false;
                //RadioButtonListShiftEnd.Visible = false;
                DropDownListRecipes.Visible = false;
                LabelRecipe.Visible = false;
                return;
            }
            else
            {
                LabelShiftError.Visible = false;
                LabelStartShift.Visible = true;
                //LabelEndShift.Visible = true;
                RadioButtonListShiftStart.Visible = true;
                //RadioButtonListShiftEnd.Visible = true;
            }

            while (reader.Read())
            {
                RadioButtonListShiftStart.Items.Add(reader["shiftstart"].ToString());
                RadioButtonListShiftStart.Items[RadioButtonListShiftStart.Items.Count - 1].Value = reader["shiftindex"].ToString();
                RadioButtonListShiftStart.SelectedIndex = 0;
            }
            reader.Close();*/

            //End Shift
            if (CalendarShiftStart.SelectedDates.Count == 1)  //single day selected
                query = "Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftend) = datepart(yy,'" + CalendarShiftEnd.SelectedDates[0].Month + "/" + CalendarShiftEnd.SelectedDates[0].Day + "/" + CalendarShiftEnd.SelectedDates[0].Year + "') and datepart(m,shiftend) = datepart(m,'" + CalendarShiftEnd.SelectedDates[0].Month + "/" + CalendarShiftEnd.SelectedDates[0].Day + "/" + CalendarShiftEnd.SelectedDates[0].Year + "') and datepart(dd,shiftend) = datepart(dd,'" + CalendarShiftEnd.SelectedDates[0].Month + "/" + CalendarShiftEnd.SelectedDates[0].Day + "/" + CalendarShiftEnd.SelectedDates[0].Year + "')";
            else  //week or month selected
            {
                for (int i = CalendarShiftStart.SelectedDates.Count - 1; i >= 0; i--)
                {
                    SqlCommand commandtemp = new SqlCommand("Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftend) = datepart(yy,'" + CalendarShiftEnd.SelectedDates[i].Month + "/" + CalendarShiftEnd.SelectedDates[i].Day + "/" + CalendarShiftEnd.SelectedDates[i].Year + "') and datepart(m,shiftend) = datepart(m,'" + CalendarShiftEnd.SelectedDates[i].Month + "/" + CalendarShiftEnd.SelectedDates[i].Day + "/" + CalendarShiftEnd.SelectedDates[i].Year + "') and datepart(dd,shiftend) = datepart(dd,'" + CalendarShiftEnd.SelectedDates[i].Month + "/" + CalendarShiftEnd.SelectedDates[i].Day + "/" + CalendarShiftEnd.SelectedDates[i].Year + "')", connection);
                    SqlDataReader readertemp = commandtemp.ExecuteReader();
                    if (readertemp.HasRows)
                    {
                        dayindex = i;
                        readertemp.Close();
                        break;
                    }
                    readertemp.Close();
                }
                query = "Select shiftindex,shiftstart,shiftend from shifts where datepart(yy,shiftstart) = datepart(yy,'" + CalendarShiftEnd.SelectedDates[dayindex].Month + "/" + CalendarShiftEnd.SelectedDates[dayindex].Day + "/" + CalendarShiftEnd.SelectedDates[dayindex].Year + "') and datepart(m,shiftstart) = datepart(m,'" + CalendarShiftEnd.SelectedDates[dayindex].Month + "/" + CalendarShiftEnd.SelectedDates[dayindex].Day + "/" + CalendarShiftEnd.SelectedDates[dayindex].Year + "') and datepart(dd,shiftstart) = datepart(dd,'" + CalendarShiftEnd.SelectedDates[dayindex].Month + "/" + CalendarShiftEnd.SelectedDates[dayindex].Day + "/" + CalendarShiftEnd.SelectedDates[dayindex].Year + "')";
            }

            SqlCommand command1 = new SqlCommand(query, connection);
            SqlDataReader reader1 = command1.ExecuteReader();

            if (!reader1.HasRows)
            {
                LabelShiftError.Visible = true;
                //LabelEndShift.Visible = false;
                RadioButtonListShiftEnd.Visible = false;
                DropDownListRecipes.Visible = false;
                LabelRecipe.Visible = false;
                return;
            }
            else
            {
                LabelShiftError.Visible = false;
                //LabelEndShift.Visible = true;
                RadioButtonListShiftEnd.Visible = true;
                DropDownListRecipes.Visible = true;
                LabelRecipe.Visible = true;
            }

            while (reader1.Read())
            {
                if (reader1["shiftend"].ToString() == "")
                    RadioButtonListShiftEnd.Items.Add("Current Shift (in progress)");
                else
                    RadioButtonListShiftEnd.Items.Add(reader1["shiftend"].ToString());
                RadioButtonListShiftEnd.Items[RadioButtonListShiftEnd.Items.Count - 1].Value = reader1["shiftindex"].ToString();
                RadioButtonListShiftEnd.SelectedIndex = 0;
            }
            reader1.Close();

            SetSelectedShift();
            UpdateReportHeader();
            connection.Close();
            RefreshReport();
        }
    }
}