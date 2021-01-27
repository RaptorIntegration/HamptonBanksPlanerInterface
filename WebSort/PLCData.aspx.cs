using Logix;
using System;
using System.Web.UI.WebControls;
using WebSort.Model;
using System.Data.SqlClient;

namespace WebSort
{
    public partial class PLCData : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("PLC Data", User.Identity.Name);
            
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
                ButtonRead.Enabled = false;
                ButtonWrite.Enabled = false;
            }
            else
            {
                ButtonRead.Enabled = true;
                ButtonWrite.Enabled = true;
            }

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from RaptorCommSettings", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                Global.MyPLC.IPAddress = reader["PLCIPAddress"].ToString();
                Global.MyPLC.Path = reader["PLCProcessorSlot"].ToString();
                Global.MyPLC.Timeout = int.Parse(reader["PLCTimeout"].ToString());
            }
            reader.Close();
            connection.Close();

            string str = "openwindow('http://" + Global.MyPLC.IPAddress + "/index.html');";
            Button1.Attributes.Clear();
            Button1.Attributes.Add("onclick", str);
        }

        protected void ButtonRead_Click(object sender, EventArgs e)
        {
            LabelError.Text = "";

            if (Global.MyPLC.Connect() != ResultCode.E_SUCCESS)
            {
                LabelError.Text = Global.MyPLC.ErrorString;
                return;
            }
            if (TextBox1.Text == "")
            {
                LabelError.Text = "Please enter a tag name.";
                return;
            }
            Tag MyTag = new Tag(TextBox1.Text);
            if (RadioButtonTypeInt.Checked == true)
                MyTag.DataType = Logix.Tag.ATOMIC.DINT;
            else if (RadioButtonTypeBool.Checked == true)
                MyTag.DataType = Logix.Tag.ATOMIC.BOOL;
            else if (RadioButtonTypeString.Checked == true)
                MyTag.DataType = Logix.Tag.ATOMIC.STRING;

            try
            {
                Global.MyPLC.ReadTag(MyTag);
                TextBoxWrite.Text = MyTag.Value.ToString();
                LabelError.Text = "Succeeded";
            }
            catch
            {
                LabelError.Text = MyTag.ErrorString;
            }
        }

        protected void ButtonWrite_Click(object sender, EventArgs e)
        {
            LabelError.Text = "";

            if (Global.MyPLC.Connect() != ResultCode.E_SUCCESS)
            {
                LabelError.Text = Global.MyPLC.ErrorString;
                return;
            }
            if (TextBox1.Text == "")
            {
                LabelError.Text = "Please enter a tag name.";
                return;
            }
            Tag MyTag = new Tag(TextBox1.Text);
            if (RadioButtonTypeInt.Checked == true)
                MyTag.DataType = Logix.Tag.ATOMIC.DINT;
            else if (RadioButtonTypeBool.Checked == true)
                MyTag.DataType = Logix.Tag.ATOMIC.BOOL;
            else if (RadioButtonTypeString.Checked == true)
                MyTag.DataType = Logix.Tag.ATOMIC.STRING;
            else if (RadioButtonTypeReal.Checked == true)
                MyTag.DataType = Logix.Tag.ATOMIC.REAL;

            try
            {
                MyTag.Value = TextBoxWrite.Text;
                Global.MyPLC.WriteTag(MyTag);
                LabelError.Text = "Succeeded";
            }
            catch
            {
                LabelError.Text = MyTag.ErrorString;
            }
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            Timer2.Interval = 2000;
            if (ButtonPause.Text == "Resume")
                return;
            if (GridViewLug.Visible == true)
                GridViewLug.DataBind();
            if (GridViewBinReceived.Visible == true)
                GridViewBinReceived.DataBind();
            if (GridViewBinSent.Visible == true)
                GridViewBinSent.DataBind();
            if (GridViewProductSent.Visible == true)
                GridViewProductSent.DataBind();
            if (GridViewSortSent.Visible == true)
                GridViewSortSent.DataBind();
            if (GridViewAlarmsReceived.Visible == true)
                GridViewAlarmsReceived.DataBind();
            if (GridViewAlarmsSent.Visible == true)
                GridViewAlarmsSent.DataBind();
            if (GridViewLengthSent.Visible == true)
                GridViewLengthSent.DataBind();
            if (GridViewPETLengthSent.Visible == true)
                GridViewPETLengthSent.DataBind();
            if (GridViewGradeSent.Visible == true)
                GridViewGradeSent.DataBind();
            if (GridViewGraderTest.Visible == true)
                GridViewGraderTest.DataBind();
            if (GridViewMSRSample.Visible == true)
                GridViewMSRSample.DataBind();
            if (GridViewMoistureSent.Visible == true)
                GridViewMoistureSent.DataBind();
            if (GridViewThickness.Visible == true)
                GridViewThickness.DataBind();
            if (GridViewWidth.Visible == true)
                GridViewWidth.DataBind();
            if (GridViewTiming.Visible == true)
                GridViewTiming.DataBind();
            if (GridViewDiagnostics.Visible == true)
            {
                GridViewDiagnostics.DataBind();
                GridViewDiagnostics1.DataBind();
            }
            if (GridViewDrives.Visible == true)
                GridViewDrives.DataBind();
        }

        protected void CheckBoxList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckBoxList cb = (CheckBoxList)sender;
            if (cb.Items[0].Selected)
                GridViewLug.Visible = true;
            else
                GridViewLug.Visible = false;
            if (cb.Items[1].Selected)
            {
                GridViewBinReceived.Visible = true;
                GridViewBinSent.Visible = true;
            }
            else
            {
                GridViewBinReceived.Visible = false;
                GridViewBinSent.Visible = false;
            }
            if (cb.Items[2].Selected)
                GridViewSortSent.Visible = true;
            else
                GridViewSortSent.Visible = false;
            if (cb.Items[3].Selected)
            {
                GridViewAlarmsReceived.Visible = true;
                GridViewAlarmsSent.Visible = true;
            }
            else
            {
                GridViewAlarmsReceived.Visible = false;
                GridViewAlarmsSent.Visible = false;
            }
            if (cb.Items[4].Selected)
                GridViewProductSent.Visible = true;
            else
                GridViewProductSent.Visible = false;
            if (cb.Items[5].Selected)
                GridViewGradeSent.Visible = true;
            else
                GridViewGradeSent.Visible = false;
            if (cb.Items[6].Selected)
                GridViewGraderTest.Visible = true;
            else
                GridViewGraderTest.Visible = false;
            if (cb.Items[7].Selected)
                GridViewMSRSample.Visible = true;
            else
                GridViewMSRSample.Visible = false;
            if (cb.Items[8].Selected)
                GridViewMoistureSent.Visible = true;
            else
                GridViewMoistureSent.Visible = false;
            if (cb.Items[9].Selected)
                GridViewThickness.Visible = true;
            else
                GridViewThickness.Visible = false;
            if (cb.Items[10].Selected)
                GridViewWidth.Visible = true;
            else
                GridViewWidth.Visible = false;
            if (cb.Items[11].Selected)
                GridViewLengthSent.Visible = true;
            else
                GridViewLengthSent.Visible = false;
            if (cb.Items[12].Selected)
                GridViewPETLengthSent.Visible = true;
            else
                GridViewPETLengthSent.Visible = false;
            if (cb.Items[13].Selected)
                GridViewTiming.Visible = true;
            else
                GridViewTiming.Visible = false;
            if (cb.Items[14].Selected)
            {
                GridViewDiagnostics.Visible = true;
                GridViewDiagnostics1.Visible = true;
            }
            else
            {
                GridViewDiagnostics.Visible = false;
                GridViewDiagnostics1.Visible = false;
            }
            if (cb.Items[15].Selected)
                GridViewDrives.Visible = true;
            else
                GridViewDrives.Visible = false;
        }

        protected void ButtonPause_Click(object sender, EventArgs e)
        {
            if (ButtonPause.Text == "Pause")
            {
                ButtonPause.Text = "Resume";
                Timer2.Enabled = false;
                GridViewLug.AllowPaging = false;
                GridViewBinReceived.AllowPaging = false;
                GridViewBinSent.AllowPaging = false;
                GridViewProductSent.AllowPaging = false;
                GridViewSortSent.AllowPaging = false;
                GridViewAlarmsReceived.AllowPaging = false;
                GridViewAlarmsSent.AllowPaging = false;
                GridViewLengthSent.AllowPaging = false;
                GridViewPETLengthSent.AllowPaging = false;
                GridViewGradeSent.AllowPaging = false;
                GridViewGraderTest.AllowPaging = false;
                GridViewMSRSample.AllowPaging = false;
                GridViewMoistureSent.AllowPaging = false;
                GridViewThickness.AllowPaging = false;
                GridViewWidth.AllowPaging = false;
                GridViewTiming.AllowPaging = false;
                GridViewDiagnostics.AllowPaging = false;
                GridViewDrives.AllowPaging = false;
            }
            else if (ButtonPause.Text == "Resume")
            {
                ButtonPause.Text = "Pause";
                Timer2.Interval = 1;
                Timer2.Enabled = true;
                GridViewLug.AllowPaging = true;
                GridViewBinReceived.AllowPaging = true;
                GridViewBinSent.AllowPaging = true;
                GridViewProductSent.AllowPaging = true;
                GridViewSortSent.AllowPaging = true;
                GridViewAlarmsReceived.AllowPaging = true;
                GridViewAlarmsSent.AllowPaging = true;
                GridViewLengthSent.AllowPaging = true;
                GridViewPETLengthSent.AllowPaging = true;
                GridViewGradeSent.AllowPaging = true;
                GridViewGraderTest.AllowPaging = true;
                GridViewMSRSample.AllowPaging = true;
                GridViewMoistureSent.AllowPaging = true;
                GridViewThickness.AllowPaging = true;
                GridViewWidth.AllowPaging = true;
                GridViewTiming.AllowPaging = true;
                GridViewDiagnostics.AllowPaging = true;
                GridViewDrives.AllowPaging = true;
            }
        }

        protected void GridViewProductSent_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //Response.Redirect("http://" + IPAddress ,false);
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
        }
    }
}