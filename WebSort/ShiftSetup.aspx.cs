using System;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class ShiftSetup : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Shift Setup", User.Identity.Name);
            
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
                CheckBoxAutoIncrement.Enabled = false;
                ButtonManualShiftEnd.Enabled = false;
                ButtonFillDays.Enabled = false;
                ButtonFillAfternoons.Enabled = false;
                ButtonFillNights.Enabled = false;
                TimePicker1.Enabled = false;
                TimePicker2.Enabled = false;
                TimePicker3.Enabled = false;
                TimePicker4.Enabled = false;
                TimePicker5.Enabled = false;
                TimePicker6.Enabled = false;
                TimePicker7.Enabled = false;
                TimePicker8.Enabled = false;
                TimePicker9.Enabled = false;
                TimePicker10.Enabled = false;
                TimePicker11.Enabled = false;
                TimePicker12.Enabled = false;
                TimePicker13.Enabled = false;
                TimePicker14.Enabled = false;
                TimePicker15.Enabled = false;
                TimePicker16.Enabled = false;
                TimePicker17.Enabled = false;
                TimePicker18.Enabled = false;
                TimePicker19.Enabled = false;
                TimePicker20.Enabled = false;
                TimePicker21.Enabled = false;
                TimePicker22.Enabled = false;
                TimePicker23.Enabled = false;
                TimePicker24.Enabled = false;
                TimePicker25.Enabled = false;
                TimePicker26.Enabled = false;
                TimePicker27.Enabled = false;
                TimePicker28.Enabled = false;
                TimePicker29.Enabled = false;
                TimePicker30.Enabled = false;
                TimePicker31.Enabled = false;
                TimePicker32.Enabled = false;
                TimePicker33.Enabled = false;
                TimePicker34.Enabled = false;
                TimePicker35.Enabled = false;
                TimePicker36.Enabled = false;
                TimePicker37.Enabled = false;
                TimePicker38.Enabled = false;
                TimePicker39.Enabled = false;
                TimePicker40.Enabled = false;
                TimePicker41.Enabled = false;
                TimePicker42.Enabled = false;
                BreakTimePicker1.Enabled = false;
                BreakTimePicker2.Enabled = false;
                BreakTimePicker3.Enabled = false;
                BreakTimePicker4.Enabled = false;
                BreakTimePicker5.Enabled = false;
                BreakTimePicker6.Enabled = false;
                BreakTimePicker7.Enabled = false;
                BreakTimePicker8.Enabled = false;
                BreakTimePicker9.Enabled = false;
                BreakTimePicker10.Enabled = false;
                BreakTimePicker11.Enabled = false;
                BreakTimePicker12.Enabled = false;
                BreakTimePicker13.Enabled = false;
                BreakTimePicker14.Enabled = false;
                BreakTimePicker15.Enabled = false;
                BreakTimePicker16.Enabled = false;
                BreakTimePicker17.Enabled = false;
                BreakTimePicker18.Enabled = false;
                BreakTimePicker19.Enabled = false;
                BreakTimePicker20.Enabled = false;
                BreakTimePicker21.Enabled = false;
                BreakTimePicker22.Enabled = false;
                BreakTimePicker23.Enabled = false;
                BreakTimePicker24.Enabled = false;
                CheckBox1.Enabled = false;
                CheckBox2.Enabled = false;
                CheckBox3.Enabled = false;
                CheckBox4.Enabled = false;
                CheckBox5.Enabled = false;
                CheckBox6.Enabled = false;
                CheckBox7.Enabled = false;
                CheckBox8.Enabled = false;
                CheckBox9.Enabled = false;
                CheckBox10.Enabled = false;
                CheckBox11.Enabled = false;
                CheckBox12.Enabled = false;
                CheckBox13.Enabled = false;
                CheckBox14.Enabled = false;
                CheckBox15.Enabled = false;
                CheckBox16.Enabled = false;
                CheckBox17.Enabled = false;
                CheckBox18.Enabled = false;
                CheckBox19.Enabled = false;
                CheckBox20.Enabled = false;
                CheckBox21.Enabled = false;
                BreakCheckBox1.Enabled = false;
                BreakCheckBox2.Enabled = false;
                BreakCheckBox3.Enabled = false;
                BreakCheckBox4.Enabled = false;
                BreakCheckBox5.Enabled = false;
                BreakCheckBox6.Enabled = false;
                BreakCheckBox7.Enabled = false;
                BreakCheckBox8.Enabled = false;
                BreakCheckBox9.Enabled = false;
                BreakCheckBox10.Enabled = false;
                BreakCheckBox11.Enabled = false;
                BreakCheckBox12.Enabled = false;
            }
            else
            {
                CheckBoxAutoIncrement.Enabled = true;
                ButtonManualShiftEnd.Enabled = true;
                ButtonFillDays.Enabled = true;
                ButtonFillAfternoons.Enabled = true;
                ButtonFillNights.Enabled = true;
                TimePicker1.Enabled = true;
                TimePicker2.Enabled = true;
                TimePicker3.Enabled = true;
                TimePicker4.Enabled = true;
                TimePicker5.Enabled = true;
                TimePicker6.Enabled = true;
                TimePicker7.Enabled = true;
                TimePicker8.Enabled = true;
                TimePicker9.Enabled = true;
                TimePicker10.Enabled = true;
                TimePicker11.Enabled = true;
                TimePicker12.Enabled = true;
                TimePicker13.Enabled = true;
                TimePicker14.Enabled = true;
                TimePicker15.Enabled = true;
                TimePicker16.Enabled = true;
                TimePicker17.Enabled = true;
                TimePicker18.Enabled = true;
                TimePicker19.Enabled = true;
                TimePicker20.Enabled = true;
                TimePicker21.Enabled = true;
                TimePicker22.Enabled = true;
                TimePicker23.Enabled = true;
                TimePicker24.Enabled = true;
                TimePicker25.Enabled = true;
                TimePicker26.Enabled = true;
                TimePicker27.Enabled = true;
                TimePicker28.Enabled = true;
                TimePicker29.Enabled = true;
                TimePicker30.Enabled = true;
                TimePicker31.Enabled = true;
                TimePicker32.Enabled = true;
                TimePicker33.Enabled = true;
                TimePicker34.Enabled = true;
                TimePicker35.Enabled = true;
                TimePicker36.Enabled = true;
                TimePicker37.Enabled = true;
                TimePicker38.Enabled = true;
                TimePicker39.Enabled = true;
                TimePicker40.Enabled = true;
                TimePicker41.Enabled = true;
                TimePicker42.Enabled = true;
                BreakTimePicker1.Enabled = true;
                BreakTimePicker2.Enabled = true;
                BreakTimePicker3.Enabled = true;
                BreakTimePicker4.Enabled = true;
                BreakTimePicker5.Enabled = true;
                BreakTimePicker6.Enabled = true;
                BreakTimePicker7.Enabled = true;
                BreakTimePicker8.Enabled = true;
                BreakTimePicker9.Enabled = true;
                BreakTimePicker10.Enabled = true;
                BreakTimePicker11.Enabled = true;
                BreakTimePicker12.Enabled = true;
                BreakTimePicker13.Enabled = true;
                BreakTimePicker14.Enabled = true;
                BreakTimePicker15.Enabled = true;
                BreakTimePicker16.Enabled = true;
                BreakTimePicker17.Enabled = true;
                BreakTimePicker18.Enabled = true;
                BreakTimePicker19.Enabled = true;
                BreakTimePicker20.Enabled = true;
                BreakTimePicker21.Enabled = true;
                BreakTimePicker22.Enabled = true;
                BreakTimePicker23.Enabled = true;
                BreakTimePicker24.Enabled = true;
                CheckBox1.Enabled = true;
                CheckBox2.Enabled = true;
                CheckBox3.Enabled = true;
                CheckBox4.Enabled = true;
                CheckBox5.Enabled = true;
                CheckBox6.Enabled = true;
                CheckBox7.Enabled = true;
                CheckBox8.Enabled = true;
                CheckBox9.Enabled = true;
                CheckBox10.Enabled = true;
                CheckBox11.Enabled = true;
                CheckBox12.Enabled = true;
                CheckBox13.Enabled = true;
                CheckBox14.Enabled = true;
                CheckBox15.Enabled = true;
                CheckBox16.Enabled = true;
                CheckBox17.Enabled = true;
                CheckBox18.Enabled = true;
                CheckBox19.Enabled = true;
                CheckBox20.Enabled = true;
                CheckBox21.Enabled = true;
                BreakCheckBox1.Enabled = true;
                BreakCheckBox2.Enabled = true;
                BreakCheckBox3.Enabled = true;
                BreakCheckBox4.Enabled = true;
                BreakCheckBox5.Enabled = true;
                BreakCheckBox6.Enabled = true;
                BreakCheckBox7.Enabled = true;
                BreakCheckBox8.Enabled = true;
                BreakCheckBox9.Enabled = true;
                BreakCheckBox10.Enabled = true;
                BreakCheckBox11.Enabled = true;
                BreakCheckBox12.Enabled = true;
            }
            if (ASPxPageControl1.ActiveTabIndex == 2)
            {
                Timer3.Enabled = true;
            }
            else
            {
                Timer3.Enabled = false;
            }
            if (IsPostBack)
                return;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from ReportSettings", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            CheckBoxAutoIncrement.Checked = Convert.ToBoolean(reader["AutomaticShiftIncrementing"]);

            reader.Close();
            connection.Close();
        }

        protected void TimePicker1_TimeChanged(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            eWorld.UI.TimePicker tp = sender as eWorld.UI.TimePicker;
            string ctrlName = ((Control)sender).ID;
            ctrlName = ctrlName.Substring(10);
            int day, shiftid, ctrln;

            ctrln = int.Parse(ctrlName.ToString());
            if (int.Parse(ctrlName.ToString()) % 2 == 0)
                shiftid = int.Parse(ctrlName.ToString()) / 2;
            else
                shiftid = (int.Parse(ctrlName.ToString()) + 1) / 2;

            if (ctrln == 1 || ctrln == 2 || ctrln == 15 || ctrln == 16 || ctrln == 29 || ctrln == 30)
                day = 2;
            else if (ctrln == 3 || ctrln == 4 || ctrln == 17 || ctrln == 18 || ctrln == 31 || ctrln == 32)
                day = 3;
            else if (ctrln == 5 || ctrln == 6 || ctrln == 19 || ctrln == 20 || ctrln == 33 || ctrln == 34)
                day = 4;
            else if (ctrln == 7 || ctrln == 8 || ctrln == 21 || ctrln == 22 || ctrln == 35 || ctrln == 36)
                day = 5;
            else if (ctrln == 9 || ctrln == 10 || ctrln == 23 || ctrln == 24 || ctrln == 37 || ctrln == 38)
                day = 6;
            else if (ctrln == 11 || ctrln == 12 || ctrln == 25 || ctrln == 26 || ctrln == 39 || ctrln == 40)
                day = 7;
            else if (ctrln == 13 || ctrln == 14 || ctrln == 27 || ctrln == 28 || ctrln == 41 || ctrln == 42)
                day = 1;
            else
                day = 0;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            if (int.Parse(ctrlName.ToString()) % 2 == 0) //shift end
            {
                //add code to increment the day if the end time goes past midnight
                if (tp.SelectedTime.Hour >= 0 && tp.SelectedTime.Hour <= 6)
                    day = day + 1;
                if (day == 8)
                    day = 1;
                SqlCommand cmd = new SqlCommand("update shiftschedule set shiftendday=" + day.ToString() + ",shiftendtime='" + tp.PostedTime + "' where shiftid=" + shiftid.ToString(), connection);
                cmd.ExecuteNonQuery();
            }
            else  //shift start
            {
                SqlCommand cmd = new SqlCommand("update shiftschedule set shiftstartday=" + day.ToString() + ",shiftstarttime='" + tp.PostedTime + "' where shiftid=" + shiftid.ToString(), connection);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }

        protected void TimePicker1_Init(object sender, EventArgs e)
        {
            int ctrln;
            eWorld.UI.TimePicker tp = sender as eWorld.UI.TimePicker;

            string ctrlName = ((Control)sender).ID;
            ctrlName = ctrlName.Substring(10);
            if (int.Parse(ctrlName.ToString()) % 2 == 0)
                ctrln = int.Parse(ctrlName.ToString()) / 2;
            else
                ctrln = (int.Parse(ctrlName.ToString()) + 1) / 2;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from shiftschedule where shiftid=" + ctrln.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (int.Parse(ctrlName.ToString()) % 2 == 0)
                    tp.SelectedTime = (System.DateTime)reader["shiftendtime"];
                else
                    tp.SelectedTime = (System.DateTime)reader["shiftstarttime"];
            }
            reader.Close();
            connection.Close();
        }

        protected void ButtonFillDays_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            TimePicker3.SelectedTime = TimePicker1.SelectedTime;
            TimePicker5.SelectedTime = TimePicker1.SelectedTime;
            TimePicker7.SelectedTime = TimePicker1.SelectedTime;
            TimePicker9.SelectedTime = TimePicker1.SelectedTime;
            TimePicker11.SelectedTime = TimePicker1.SelectedTime;
            TimePicker13.SelectedTime = TimePicker1.SelectedTime;
            TimePicker4.SelectedTime = TimePicker2.SelectedTime;
            TimePicker6.SelectedTime = TimePicker2.SelectedTime;
            TimePicker8.SelectedTime = TimePicker2.SelectedTime;
            TimePicker10.SelectedTime = TimePicker2.SelectedTime;
            TimePicker12.SelectedTime = TimePicker2.SelectedTime;
            TimePicker14.SelectedTime = TimePicker2.SelectedTime;
            CheckBox2.Checked = CheckBox1.Checked;
            CheckBox3.Checked = CheckBox1.Checked;
            CheckBox4.Checked = CheckBox1.Checked;
            CheckBox5.Checked = CheckBox1.Checked;
            CheckBox6.Checked = CheckBox1.Checked;
            CheckBox7.Checked = CheckBox1.Checked;
            //commit
            TimePicker1_TimeChanged(TimePicker1, e);
            TimePicker1_TimeChanged(TimePicker2, e);
            TimePicker1_TimeChanged(TimePicker3, e);
            TimePicker1_TimeChanged(TimePicker4, e);
            TimePicker1_TimeChanged(TimePicker5, e);
            TimePicker1_TimeChanged(TimePicker6, e);
            TimePicker1_TimeChanged(TimePicker7, e);
            TimePicker1_TimeChanged(TimePicker8, e);
            TimePicker1_TimeChanged(TimePicker9, e);
            TimePicker1_TimeChanged(TimePicker10, e);
            TimePicker1_TimeChanged(TimePicker11, e);
            TimePicker1_TimeChanged(TimePicker12, e);
            TimePicker1_TimeChanged(TimePicker13, e);
            TimePicker1_TimeChanged(TimePicker14, e);
            CheckBox1_CheckedChanged(CheckBox1, e);
            CheckBox1_CheckedChanged(CheckBox2, e);
            CheckBox1_CheckedChanged(CheckBox3, e);
            CheckBox1_CheckedChanged(CheckBox4, e);
            CheckBox1_CheckedChanged(CheckBox5, e);
            CheckBox1_CheckedChanged(CheckBox6, e);
            CheckBox1_CheckedChanged(CheckBox7, e);
        }

        protected void ButtonFillAfternoons_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            TimePicker17.SelectedTime = TimePicker15.SelectedTime;
            TimePicker19.SelectedTime = TimePicker15.SelectedTime;
            TimePicker21.SelectedTime = TimePicker15.SelectedTime;
            TimePicker23.SelectedTime = TimePicker15.SelectedTime;
            TimePicker25.SelectedTime = TimePicker15.SelectedTime;
            TimePicker27.SelectedTime = TimePicker15.SelectedTime;
            TimePicker18.SelectedTime = TimePicker16.SelectedTime;
            TimePicker20.SelectedTime = TimePicker16.SelectedTime;
            TimePicker22.SelectedTime = TimePicker16.SelectedTime;
            TimePicker24.SelectedTime = TimePicker16.SelectedTime;
            TimePicker26.SelectedTime = TimePicker16.SelectedTime;
            TimePicker28.SelectedTime = TimePicker16.SelectedTime;
            CheckBox9.Checked = CheckBox8.Checked;
            CheckBox10.Checked = CheckBox8.Checked;
            CheckBox11.Checked = CheckBox8.Checked;
            CheckBox12.Checked = CheckBox8.Checked;
            CheckBox13.Checked = CheckBox8.Checked;
            CheckBox14.Checked = CheckBox8.Checked;
            //commit
            TimePicker1_TimeChanged(TimePicker15, e);
            TimePicker1_TimeChanged(TimePicker16, e);
            TimePicker1_TimeChanged(TimePicker17, e);
            TimePicker1_TimeChanged(TimePicker18, e);
            TimePicker1_TimeChanged(TimePicker19, e);
            TimePicker1_TimeChanged(TimePicker20, e);
            TimePicker1_TimeChanged(TimePicker21, e);
            TimePicker1_TimeChanged(TimePicker22, e);
            TimePicker1_TimeChanged(TimePicker23, e);
            TimePicker1_TimeChanged(TimePicker24, e);
            TimePicker1_TimeChanged(TimePicker25, e);
            TimePicker1_TimeChanged(TimePicker26, e);
            TimePicker1_TimeChanged(TimePicker27, e);
            TimePicker1_TimeChanged(TimePicker28, e);
            CheckBox1_CheckedChanged(CheckBox8, e);
            CheckBox1_CheckedChanged(CheckBox9, e);
            CheckBox1_CheckedChanged(CheckBox10, e);
            CheckBox1_CheckedChanged(CheckBox11, e);
            CheckBox1_CheckedChanged(CheckBox12, e);
            CheckBox1_CheckedChanged(CheckBox13, e);
            CheckBox1_CheckedChanged(CheckBox14, e);
        }

        protected void ButtonFillNights_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            TimePicker31.SelectedTime = TimePicker29.SelectedTime;
            TimePicker33.SelectedTime = TimePicker29.SelectedTime;
            TimePicker35.SelectedTime = TimePicker29.SelectedTime;
            TimePicker37.SelectedTime = TimePicker29.SelectedTime;
            TimePicker39.SelectedTime = TimePicker29.SelectedTime;
            TimePicker41.SelectedTime = TimePicker29.SelectedTime;
            TimePicker32.SelectedTime = TimePicker30.SelectedTime;
            TimePicker34.SelectedTime = TimePicker30.SelectedTime;
            TimePicker36.SelectedTime = TimePicker30.SelectedTime;
            TimePicker38.SelectedTime = TimePicker30.SelectedTime;
            TimePicker40.SelectedTime = TimePicker30.SelectedTime;
            TimePicker42.SelectedTime = TimePicker30.SelectedTime;
            CheckBox16.Checked = CheckBox15.Checked;
            CheckBox17.Checked = CheckBox15.Checked;
            CheckBox18.Checked = CheckBox15.Checked;
            CheckBox19.Checked = CheckBox15.Checked;
            CheckBox20.Checked = CheckBox15.Checked;
            CheckBox21.Checked = CheckBox15.Checked;
            //commit
            TimePicker1_TimeChanged(TimePicker29, e);
            TimePicker1_TimeChanged(TimePicker30, e);
            TimePicker1_TimeChanged(TimePicker31, e);
            TimePicker1_TimeChanged(TimePicker32, e);
            TimePicker1_TimeChanged(TimePicker33, e);
            TimePicker1_TimeChanged(TimePicker34, e);
            TimePicker1_TimeChanged(TimePicker35, e);
            TimePicker1_TimeChanged(TimePicker36, e);
            TimePicker1_TimeChanged(TimePicker37, e);
            TimePicker1_TimeChanged(TimePicker38, e);
            TimePicker1_TimeChanged(TimePicker39, e);
            TimePicker1_TimeChanged(TimePicker40, e);
            TimePicker1_TimeChanged(TimePicker41, e);
            TimePicker1_TimeChanged(TimePicker42, e);
            CheckBox1_CheckedChanged(CheckBox15, e);
            CheckBox1_CheckedChanged(CheckBox16, e);
            CheckBox1_CheckedChanged(CheckBox17, e);
            CheckBox1_CheckedChanged(CheckBox18, e);
            CheckBox1_CheckedChanged(CheckBox19, e);
            CheckBox1_CheckedChanged(CheckBox20, e);
            CheckBox1_CheckedChanged(CheckBox21, e);
        }

        protected void CheckBox1_Init(object sender, EventArgs e)
        {
            int ctrln;
            CheckBox cb = sender as CheckBox;

            string ctrlName = ((Control)sender).ID;
            ctrlName = ctrlName.Substring(8);
            ctrln = int.Parse(ctrlName.ToString());

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select Enabled from shiftschedule where shiftid=" + ctrln.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader["enabled"].ToString() == "0")
                    cb.Checked = false;
                else
                    cb.Checked = true;
            }
            reader.Close();
            connection.Close();
            //if (SecurityAccess != 1)
            //    return;
            if (ctrln == 1)
            {
                TimePicker1.Enabled = cb.Checked;
                TimePicker2.Enabled = cb.Checked;
            }
            else if (ctrln == 2)
            {
                TimePicker3.Enabled = cb.Checked;
                TimePicker4.Enabled = cb.Checked;
            }
            else if (ctrln == 3)
            {
                TimePicker5.Enabled = cb.Checked;
                TimePicker6.Enabled = cb.Checked;
            }
            else if (ctrln == 4)
            {
                TimePicker7.Enabled = cb.Checked;
                TimePicker8.Enabled = cb.Checked;
            }
            else if (ctrln == 5)
            {
                TimePicker9.Enabled = cb.Checked;
                TimePicker10.Enabled = cb.Checked;
            }
            else if (ctrln == 6)
            {
                TimePicker11.Enabled = cb.Checked;
                TimePicker12.Enabled = cb.Checked;
            }
            else if (ctrln == 7)
            {
                TimePicker13.Enabled = cb.Checked;
                TimePicker14.Enabled = cb.Checked;
            }
            else if (ctrln == 8)
            {
                TimePicker15.Enabled = cb.Checked;
                TimePicker16.Enabled = cb.Checked;
            }
            else if (ctrln == 9)
            {
                TimePicker17.Enabled = cb.Checked;
                TimePicker18.Enabled = cb.Checked;
            }
            else if (ctrln == 10)
            {
                TimePicker19.Enabled = cb.Checked;
                TimePicker20.Enabled = cb.Checked;
            }
            else if (ctrln == 11)
            {
                TimePicker21.Enabled = cb.Checked;
                TimePicker22.Enabled = cb.Checked;
            }
            else if (ctrln == 12)
            {
                TimePicker23.Enabled = cb.Checked;
                TimePicker24.Enabled = cb.Checked;
            }
            else if (ctrln == 13)
            {
                TimePicker25.Enabled = cb.Checked;
                TimePicker26.Enabled = cb.Checked;
            }
            else if (ctrln == 14)
            {
                TimePicker27.Enabled = cb.Checked;
                TimePicker28.Enabled = cb.Checked;
            }
            else if (ctrln == 15)
            {
                TimePicker29.Enabled = cb.Checked;
                TimePicker30.Enabled = cb.Checked;
            }
            else if (ctrln == 16)
            {
                TimePicker31.Enabled = cb.Checked;
                TimePicker32.Enabled = cb.Checked;
            }
            else if (ctrln == 17)
            {
                TimePicker33.Enabled = cb.Checked;
                TimePicker34.Enabled = cb.Checked;
            }
            else if (ctrln == 18)
            {
                TimePicker35.Enabled = cb.Checked;
                TimePicker36.Enabled = cb.Checked;
            }
            else if (ctrln == 19)
            {
                TimePicker37.Enabled = cb.Checked;
                TimePicker38.Enabled = cb.Checked;
            }
            else if (ctrln == 20)
            {
                TimePicker39.Enabled = cb.Checked;
                TimePicker40.Enabled = cb.Checked;
            }
            else if (ctrln == 21)
            {
                TimePicker41.Enabled = cb.Checked;
                TimePicker42.Enabled = cb.Checked;
            }
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            CheckBox cb = sender as CheckBox;
            string ctrlName = ((Control)sender).ID;
            ctrlName = ctrlName.Substring(8);
            int enabled, ctrln;

            ctrln = int.Parse(ctrlName.ToString());

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            if (cb.Checked == true)
                enabled = 1;
            else
                enabled = 0;
            SqlCommand cmd = new SqlCommand("update shiftschedule set enabled=" + enabled.ToString() + " where shiftid=" + ctrln.ToString(), connection);
            cmd.ExecuteNonQuery();
            connection.Close();

            if (ctrln == 1)
            {
                TimePicker1.Enabled = cb.Checked;
                TimePicker2.Enabled = cb.Checked;
            }
            else if (ctrln == 2)
            {
                TimePicker3.Enabled = cb.Checked;
                TimePicker4.Enabled = cb.Checked;
            }
            else if (ctrln == 3)
            {
                TimePicker5.Enabled = cb.Checked;
                TimePicker6.Enabled = cb.Checked;
            }
            else if (ctrln == 4)
            {
                TimePicker7.Enabled = cb.Checked;
                TimePicker8.Enabled = cb.Checked;
            }
            else if (ctrln == 5)
            {
                TimePicker9.Enabled = cb.Checked;
                TimePicker10.Enabled = cb.Checked;
            }
            else if (ctrln == 6)
            {
                TimePicker11.Enabled = cb.Checked;
                TimePicker12.Enabled = cb.Checked;
            }
            else if (ctrln == 7)
            {
                TimePicker13.Enabled = cb.Checked;
                TimePicker14.Enabled = cb.Checked;
            }
            else if (ctrln == 8)
            {
                TimePicker15.Enabled = cb.Checked;
                TimePicker16.Enabled = cb.Checked;
            }
            else if (ctrln == 9)
            {
                TimePicker17.Enabled = cb.Checked;
                TimePicker18.Enabled = cb.Checked;
            }
            else if (ctrln == 10)
            {
                TimePicker19.Enabled = cb.Checked;
                TimePicker20.Enabled = cb.Checked;
            }
            else if (ctrln == 11)
            {
                TimePicker21.Enabled = cb.Checked;
                TimePicker22.Enabled = cb.Checked;
            }
            else if (ctrln == 12)
            {
                TimePicker23.Enabled = cb.Checked;
                TimePicker24.Enabled = cb.Checked;
            }
            else if (ctrln == 13)
            {
                TimePicker25.Enabled = cb.Checked;
                TimePicker26.Enabled = cb.Checked;
            }
            else if (ctrln == 14)
            {
                TimePicker27.Enabled = cb.Checked;
                TimePicker28.Enabled = cb.Checked;
            }
            else if (ctrln == 15)
            {
                TimePicker29.Enabled = cb.Checked;
                TimePicker30.Enabled = cb.Checked;
            }
            else if (ctrln == 16)
            {
                TimePicker31.Enabled = cb.Checked;
                TimePicker32.Enabled = cb.Checked;
            }
            else if (ctrln == 17)
            {
                TimePicker33.Enabled = cb.Checked;
                TimePicker34.Enabled = cb.Checked;
            }
            else if (ctrln == 18)
            {
                TimePicker35.Enabled = cb.Checked;
                TimePicker36.Enabled = cb.Checked;
            }
            else if (ctrln == 19)
            {
                TimePicker37.Enabled = cb.Checked;
                TimePicker38.Enabled = cb.Checked;
            }
            else if (ctrln == 20)
            {
                TimePicker39.Enabled = cb.Checked;
                TimePicker40.Enabled = cb.Checked;
            }
            else if (ctrln == 21)
            {
                TimePicker41.Enabled = cb.Checked;
                TimePicker42.Enabled = cb.Checked;
            }
        }

        protected void ButtonManualShiftEnd_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            //flag the auto reporting service stored procedure to increment the shift and print out end of shift reports
            SqlCommand command = new SqlCommand("update ReportSettings set PrintEndOfShiftReports=1", connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        protected void ButtonStart_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceController svcController2 = new ServiceController("RaptorShiftMaster");
                if (svcController2 != null)
                {
                    if (svcController2.Status == ServiceControllerStatus.Stopped)
                    {
                        svcController2.Start();
                        svcController2.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    }
                }
            }
            catch (Exception ex)
            {
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand command1 = new SqlCommand("Insert into RaptorShiftMasterLog select getdate(),'" + ex.Message.Replace("'", "''") + "'", connection);
                command1.ExecuteNonQuery();
                connection.Close();
            }
        }

        protected void ButtonStop_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceController svcController1 = new ServiceController("RaptorShiftMaster");
                if (svcController1 != null)
                {
                    if (svcController1.Status == ServiceControllerStatus.Running)
                    {
                        svcController1.Stop();
                        svcController1.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    }
                }
            }
            catch (Exception ex)
            {
                Raptor cs1 = new Raptor();
                string connectionString = Global.ConnectionString;
                System.Data.SqlClient.SqlConnection connection;
                connection = new SqlConnection(connectionString);
                // Open the connection.
                connection.Open();
                SqlCommand command1 = new SqlCommand("Insert into RaptorShiftMasterLog select getdate(),'" + ex.Message.Replace("'", "''") + "'", connection);
                command1.ExecuteNonQuery();
                connection.Close();
            }
        }

        protected void Timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                ServiceController svcController = new ServiceController("RaptorShiftMaster");
                if (svcController != null)
                {
                    LabelServiceStatus.Text = svcController.Status.ToString();
                }
            }
            catch { }
            if (LabelServiceStatus.Text == "")
            {
                LabelServiceStatus.Text = "Not Installed";
            }
            ListBox1.DataBind();
            Timer3.Interval = 1500;
        }

        protected void CheckBoxAutoIncrement_CheckedChanged(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            if (CheckBoxAutoIncrement.Checked == true)
            {
                SqlCommand command = new SqlCommand("update ReportSettings set AutomaticShiftIncrementing=1", connection);
                command.ExecuteNonQuery();
            }
            else
            {
                SqlCommand command = new SqlCommand("update ReportSettings set AutomaticShiftIncrementing=0", connection);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        protected void ASPxPageControl1_ActiveTabChanged(object source, DevExpress.Web.ASPxTabControl.TabControlEventArgs e)
        {
            if (e.Tab.Index == 2)
            {
                Timer3.Enabled = true;
            }
            else
            {
                Timer3.Enabled = false;
            }
        }

        protected void BreakTimePicker1_Init(object sender, EventArgs e)
        {
            int ctrln;
            eWorld.UI.TimePicker tp = sender as eWorld.UI.TimePicker;

            string ctrlName = ((Control)sender).ID;
            ctrlName = ctrlName.Substring(15);
            if (int.Parse(ctrlName.ToString()) % 2 == 0)
                ctrln = int.Parse(ctrlName.ToString()) / 2;
            else
                ctrln = (int.Parse(ctrlName.ToString()) + 1) / 2;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from shiftbreaks where id=" + ctrln.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (int.Parse(ctrlName.ToString()) % 2 == 0)
                    tp.SelectedTime = (System.DateTime)reader["breakend"];
                else
                    tp.SelectedTime = (System.DateTime)reader["breakstart"];
            }
            reader.Close();
            connection.Close();
        }

        protected void BreakTimePicker1_TimeChanged(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            eWorld.UI.TimePicker tp = sender as eWorld.UI.TimePicker;
            string ctrlName = ((Control)sender).ID;
            ctrlName = ctrlName.Substring(15);
            int ctrln, id;

            ctrln = int.Parse(ctrlName.ToString());
            if (int.Parse(ctrlName.ToString()) % 2 == 0)
                id = int.Parse(ctrlName.ToString()) / 2;
            else
                id = (int.Parse(ctrlName.ToString()) + 1) / 2;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            if (int.Parse(ctrlName.ToString()) % 2 == 0) //shift end
            {
                SqlCommand cmd = new SqlCommand("update shiftbreaks set breakend='" + tp.PostedTime + "' where id=" + id.ToString(), connection);
                cmd.ExecuteNonQuery();
            }
            else  //shift start
            {
                SqlCommand cmd = new SqlCommand("update shiftbreaks set breakstart='" + tp.PostedTime + "' where id=" + id.ToString(), connection);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }

        protected void BreakCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            CheckBox cb = sender as CheckBox;
            string ctrlName = ((Control)sender).ID;
            ctrlName = ctrlName.Substring(13);
            int enabled, ctrln;

            ctrln = int.Parse(ctrlName.ToString());

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            if (cb.Checked == true)
                enabled = 1;
            else
                enabled = 0;
            SqlCommand cmd = new SqlCommand("update shiftbreaks set enabled=" + enabled.ToString() + " where id=" + ctrln.ToString(), connection);
            cmd.ExecuteNonQuery();
            connection.Close();

            if (ctrln == 1)
            {
                BreakTimePicker1.Enabled = cb.Checked;
                BreakTimePicker2.Enabled = cb.Checked;
            }
            else if (ctrln == 2)
            {
                BreakTimePicker3.Enabled = cb.Checked;
                BreakTimePicker4.Enabled = cb.Checked;
            }
            else if (ctrln == 3)
            {
                BreakTimePicker5.Enabled = cb.Checked;
                BreakTimePicker6.Enabled = cb.Checked;
            }
            else if (ctrln == 4)
            {
                BreakTimePicker7.Enabled = cb.Checked;
                BreakTimePicker8.Enabled = cb.Checked;
            }
            else if (ctrln == 5)
            {
                BreakTimePicker9.Enabled = cb.Checked;
                BreakTimePicker10.Enabled = cb.Checked;
            }
            else if (ctrln == 6)
            {
                BreakTimePicker11.Enabled = cb.Checked;
                BreakTimePicker12.Enabled = cb.Checked;
            }
            else if (ctrln == 7)
            {
                BreakTimePicker13.Enabled = cb.Checked;
                BreakTimePicker14.Enabled = cb.Checked;
            }
            else if (ctrln == 8)
            {
                BreakTimePicker15.Enabled = cb.Checked;
                BreakTimePicker16.Enabled = cb.Checked;
            }
            else if (ctrln == 9)
            {
                BreakTimePicker17.Enabled = cb.Checked;
                BreakTimePicker18.Enabled = cb.Checked;
            }
            else if (ctrln == 10)
            {
                BreakTimePicker19.Enabled = cb.Checked;
                BreakTimePicker20.Enabled = cb.Checked;
            }
            else if (ctrln == 11)
            {
                BreakTimePicker21.Enabled = cb.Checked;
                BreakTimePicker22.Enabled = cb.Checked;
            }
            else if (ctrln == 12)
            {
                BreakTimePicker23.Enabled = cb.Checked;
                BreakTimePicker24.Enabled = cb.Checked;
            }
        }

        protected void BreakCheckBox1_Init(object sender, EventArgs e)
        {
            int ctrln;
            CheckBox cb = sender as CheckBox;

            string ctrlName = ((Control)sender).ID;
            ctrlName = ctrlName.Substring(13);
            ctrln = int.Parse(ctrlName.ToString());

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select Enabled from shiftbreaks where id=" + ctrln.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader["enabled"].ToString() == "0")
                    cb.Checked = false;
                else
                    cb.Checked = true;
            }
            reader.Close();
            connection.Close();
            //if (SecurityAccess != 1)
            //return;
            if (ctrln == 1)
            {
                BreakTimePicker1.Enabled = cb.Checked;
                BreakTimePicker2.Enabled = cb.Checked;
            }
            else if (ctrln == 2)
            {
                BreakTimePicker3.Enabled = cb.Checked;
                BreakTimePicker4.Enabled = cb.Checked;
            }
            else if (ctrln == 3)
            {
                BreakTimePicker5.Enabled = cb.Checked;
                BreakTimePicker6.Enabled = cb.Checked;
            }
            else if (ctrln == 4)
            {
                BreakTimePicker7.Enabled = cb.Checked;
                BreakTimePicker8.Enabled = cb.Checked;
            }
            else if (ctrln == 5)
            {
                BreakTimePicker9.Enabled = cb.Checked;
                BreakTimePicker10.Enabled = cb.Checked;
            }
            else if (ctrln == 6)
            {
                BreakTimePicker11.Enabled = cb.Checked;
                BreakTimePicker12.Enabled = cb.Checked;
            }
            else if (ctrln == 7)
            {
                BreakTimePicker13.Enabled = cb.Checked;
                BreakTimePicker14.Enabled = cb.Checked;
            }
            else if (ctrln == 8)
            {
                BreakTimePicker15.Enabled = cb.Checked;
                BreakTimePicker16.Enabled = cb.Checked;
            }
            else if (ctrln == 9)
            {
                BreakTimePicker17.Enabled = cb.Checked;
                BreakTimePicker18.Enabled = cb.Checked;
            }
            else if (ctrln == 10)
            {
                BreakTimePicker19.Enabled = cb.Checked;
                BreakTimePicker20.Enabled = cb.Checked;
            }
            else if (ctrln == 11)
            {
                BreakTimePicker21.Enabled = cb.Checked;
                BreakTimePicker22.Enabled = cb.Checked;
            }
            else if (ctrln == 12)
            {
                BreakTimePicker23.Enabled = cb.Checked;
                BreakTimePicker24.Enabled = cb.Checked;
            }
        }
    }
}