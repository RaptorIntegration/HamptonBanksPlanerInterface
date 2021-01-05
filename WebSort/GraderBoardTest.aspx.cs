using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class GraderBoardTest : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Grader Board Test", User.Identity.Name);
            
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
                ButtonTest.Enabled = false;
            }
            else
            {
                ButtonTest.Enabled = true;
            }
            if ((int)Session["OnlineSetup"] != 1)
                ButtonTest.Enabled = false;

            if (IsPostBack)
                return;

            TextBoxSampleSize.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");
            TextBoxInterval.Attributes.Add("onkeydown", "if(event.keyCode==13)return false;");

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from gradertest", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            if (reader["Active"].ToString() == "True")
            {
                ButtonTest.Text = "Cancel Test";
                LabelStatus.Visible = true;
                LabelStatus.Text = "Grader Board Test In Progress: " + reader["SamplesRemaining"].ToString();
            }
            else
            {
                ButtonTest.Text = "Begin Test";
                LabelStatus.Visible = false;
                //DropDownListBay.Text = "0";
            }
            DropDownListBay.Text = reader["bayid"].ToString();
            if (reader["Stamp"].ToString() == "True")
                CheckBoxStamp.Checked = true;
            else
                CheckBoxStamp.Checked = false;
            if (reader["Trim"].ToString() == "True")
                CheckBoxTrim.Checked = true;
            else
                CheckBoxTrim.Checked = false;

            TextBoxInterval.Text = reader["Interval"].ToString();
            TextBoxSampleSize.Text = reader["SampleSize"].ToString();

            //decode Grader, Grade and Length Maps
            CheckBoxList1.DataBind();
            CheckBoxList2.DataBind();
            int GraderMap, GradeMap;
            //int LengthMap=0;
            GraderMap = int.Parse(reader["Graders"].ToString());
            foreach (ListItem oItem in CheckBoxList1.Items)
            {
                if (oItem.Value != "0")
                    if ((GraderMap & (int)Math.Pow(2, int.Parse(oItem.Value))) == (int)Math.Pow(2, int.Parse(oItem.Value)))
                        oItem.Selected = true;
            }
            bool All = true;
            foreach (ListItem oItem in CheckBoxList1.Items)
            {
                if (oItem.Value != "0")
                    if (oItem.Selected == false)
                        All = false;
            }
            if (All)
            {
                foreach (ListItem oItem in CheckBoxList1.Items)
                    oItem.Selected = false;
                CheckBoxList1.Items[0].Selected = true;
            }

            GradeMap = int.Parse(reader["Grades"].ToString());
            foreach (ListItem oItem in CheckBoxList2.Items)
            {
                if (oItem.Value != "0")
                    if ((GradeMap & (int)Math.Pow(2, int.Parse(oItem.Value))) == (int)Math.Pow(2, int.Parse(oItem.Value)))
                        oItem.Selected = true;
            }
            All = true;
            foreach (ListItem oItem in CheckBoxList2.Items)
            {
                if (oItem.Value != "0")
                    if (oItem.Selected == false)
                        All = false;
            }
            if (All)
            {
                foreach (ListItem oItem in CheckBoxList2.Items)
                    oItem.Selected = false;
                CheckBoxList2.Items[0].Selected = true;
            }

            UpdatePanel3.Update();
            UpdatePanel4.Update();
            reader.Close();

            connection.Close();
        }

        protected void CheckBoxList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*CheckBoxList cb = (CheckBoxList)sender;
            if (cb.Items[0].Selected)
                foreach (ListItem oItem in cb.Items)
                    oItem.Selected = true;*/
        }

        protected void ButtonTest_Click(object sender, EventArgs e)
        {
            int GraderMap = 0, GradeMap = 0, LengthMap = 0;

            Timer3.Enabled = false;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            foreach (ListItem oItem in CheckBoxList1.Items)
            {
                if (oItem.Value == "0" && oItem.Selected)  //ALL
                {
                    GraderMap = -1;  //bits 1-31 turned on
                    break;
                }
                else
                {
                    if (oItem.Selected)
                        GraderMap = GraderMap | (int)Math.Pow(2, int.Parse(oItem.Value));
                }
            }
            foreach (ListItem oItem in CheckBoxList2.Items)
            {
                if (oItem.Value == "0" && oItem.Selected)  //ALL
                {
                    GradeMap = -1;  //bits 0-31 turned on
                    break;
                }
                else
                {
                    if (oItem.Selected)
                        GradeMap = GradeMap | (int)Math.Pow(2, int.Parse(oItem.Value));
                }
            }
            LengthMap = -1;

            SqlCommand cmdgt = new SqlCommand("update gradertest set Graders=" + GraderMap + ",grades=" + GradeMap + ",lengths=" + LengthMap + ",SampleSize=" + TextBoxSampleSize.Text + ",SamplesRemaining=" + TextBoxSampleSize.Text + ",Bayid=" + DropDownListBay.Text + ",interval=" + TextBoxInterval.Text + ",stamp='" + CheckBoxStamp.Checked.ToString() + "',trim='" + CheckBoxTrim.Checked.ToString() + "'", connection);
            cmdgt.ExecuteNonQuery();

            //send to PLC
            if ((int)Session["OnlineSetup"] == 1)
            {
                bool TestActive = false;

                if (ButtonTest.Text == "Begin Test")
                    TestActive = true;

                if (TestActive == true)
                {
                    //check to see if bin is spare
                    if (DropDownListBay.Text == "0")
                    {
                        LabelBayError0.Visible = true;
                        UpdatePanel5.Update();
                        Timer3.Enabled = true;
                        return;
                    }
                    SqlCommand cmdt = new SqlCommand("select binstatus from bins where binid=" + DropDownListBay.Text, connection);
                    SqlDataReader readert = cmdt.ExecuteReader();
                    readert.Read();
                    if (readert["binstatus"].ToString() != "0")
                    {
                        LabelBayError.Visible = true;
                        LabelBayError0.Visible = false;
                        readert.Close();
                        Timer3.Enabled = true;
                        return;
                    }
                    readert.Close();

                    SqlCommand cmd110 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1", connection);
                    cmd110.ExecuteNonQuery();
                    //disable the bin and send details to PLC
                    string sqltext = "insert into datarequestsbin select getdate()," + DropDownListBay.Text + ",'Grader Test',3," + TextBoxSampleSize.Text + ",0,0,0,0,0,0,0,0,0,0,0,'" + CheckBoxTrim.Checked.ToString() + "',0,0,1,0 select id=(select max(id) from datarequestsbin with(NOLOCK))";
                    SqlCommand cmd = new SqlCommand(sqltext, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequestsbin", int.Parse(reader["id"].ToString()));
                    reader.Close();
                    if (!succeeded)
                    {
                        LabelPLCTimeout.Visible = true;
                        LabelBayError.Visible = false;
                        LabelBayError0.Visible = false;
                        Timer3.Enabled = true;
                        return;
                    }

                    SqlCommand cmdt1 = new SqlCommand("update bins set binlabel='Grader Test',binstatuslabel='Disabled',binsize=" + TextBoxSampleSize.Text + ",binstatus=3 where binid=" + DropDownListBay.Text, connection);
                    cmdt1.ExecuteNonQuery();
                    SqlCommand cmd11 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1 where (datarequests & 1)=1", connection);
                    cmd11.ExecuteNonQuery();
                }

                SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 4096", connection);
                cmd0.ExecuteNonQuery();
                SqlCommand cmd111 = new SqlCommand("insert into datarequestsgradertest select getdate()," + GraderMap + "," + GradeMap + "," + LengthMap + "," + TextBoxSampleSize.Text + "," + TextBoxSampleSize.Text + "," + DropDownListBay.Text + "," + TextBoxInterval.Text + ",'" + TestActive.ToString() + "','" + CheckBoxStamp.Checked.ToString() + "','" + CheckBoxTrim.Checked.ToString() + "',1,0 select id=(select max(id) from datarequestsgradertest with(NOLOCK))", connection);
                SqlDataReader reader111 = cmd111.ExecuteReader();
                reader111.Read();
                //make sure message is processed
                bool succeeded1 = Raptor.MessageAckConfirm("datarequestsgradertest", int.Parse(reader111["id"].ToString()));
                reader111.Close();
                if (!succeeded1)
                {
                    LabelPLCTimeout.Visible = true;
                    LabelBayError.Visible = false;
                    LabelBayError0.Visible = false;
                    Timer3.Enabled = true;
                    return;
                }
                SqlCommand cmd1 = new SqlCommand("insert into datarequestsgradertest select getdate(),0,0,0,0,0,0,0,0,0,0,0,0 select id=(select max(id) from datarequestsgradertest with(NOLOCK))", connection);
                SqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Read();
                //make sure message is processed
                bool succeeded2 = Raptor.MessageAckConfirm("datarequestsgradertest", int.Parse(reader1["id"].ToString()));
                reader1.Close();
                if (!succeeded2)
                {
                    LabelPLCTimeout.Visible = true;
                    return;
                }
                SqlCommand cmd1a = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-4096 where (datarequests & 4096)=4096", connection);
                cmd1a.ExecuteNonQuery();
                if (ButtonTest.Text == "Begin Test")
                {
                    //ButtonTest.Text = "Cancel Test";
                    //LabelStatus.Visible = true;
                    LabelBayError.Visible = false;
                    LabelBayError0.Visible = false;
                }
                else
                {
                    //ButtonTest.Text = "Begin Test";
                    // LabelStatus.Visible = false;
                    LabelBayError.Visible = false;
                    LabelBayError0.Visible = false;
                }
            }
            connection.Close();
            Timer3.Enabled = true;
        }

        protected void Timer3_Tick(object sender, EventArgs e)
        {
            //if (LabelStatus.Text == "Begin Test")
            // return;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from gradertest", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader["Active"].ToString() == "True")
            {
                ButtonTest.Text = "Cancel Test";
                LabelStatus.Visible = true;
            }
            else
            {
                ButtonTest.Text = "Begin Test";
                LabelStatus.Visible = false;
            }
            if (reader["SamplesRemaining"].ToString() != "0")
                LabelStatus.Text = "Grader Board Test In Progress: " + reader["SamplesRemaining"].ToString();
            else
                LabelStatus.Text = "Grader Board Test In Progress";

            UpdatePanel3.Update();
            UpdatePanel4.Update();
            UpdatePanel7.Update();
            reader.Close();
            connection.Close();
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            if (LabelStatus.Visible == false)
                return;
            if ((int)Session["OnlineSetup"] == 1)
            {
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                LabelPLCTimeout.Visible = false;
                SqlCommand cmd0 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 4096", connection);
                cmd0.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand("insert into datarequestsgradertest select getdate(),0,0,0,0,0,0,0,0,0,0,0,0 select id=(select max(id) from datarequestsgradertest with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsgradertest", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelPLCTimeout.Visible = true;
                    return;
                }

                SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-4096 where (datarequests & 4096)=4096", connection);
                cmd1.ExecuteNonQuery();

                connection.Close();
            }
        }

        protected void TextBoxSampleSize_TextChanged(object sender, EventArgs e)
        {
            //Timer3.Enabled = false;
        }
    }
}