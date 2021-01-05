using System;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class NetworkSetup : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Network Setup", User.Identity.Name);
            
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
                TextBoxIP.Enabled = false;
                TextBoxSlot.Enabled = false;
                TextBoxTimeout.Enabled = false;
                ButtonSave.Enabled = false;
            }
            else
            {
                TextBoxIP.Enabled = true;
                TextBoxSlot.Enabled = true;
                TextBoxTimeout.Enabled = true;
                ButtonSave.Enabled = true;
            }
            //if (IsPostBack)
            //return;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("Select * from RaptorCommSettings,PLCLED,PLCFault,PLCKeyswitch where raptorcommsettings.plcled=plcled.plcled and raptorcommsettings.plcfault=plcfault.plcfault and raptorcommsettings.plckeyswitch=plckeyswitch.plckeyswitch", connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                try
                {
                    //TextBoxIP.Text = reader["PLCIPAddress"].ToString();
                    //TextBoxSlot.Text = reader["PLCProcessorSlot"].ToString();
                    //TextBoxTimeout.Text = reader["PLCTimeout"].ToString();
                    LabelPLCLED.Text = reader["PLCLEDDescription"].ToString();
                    LabelPLCFault.Text = reader["PLCFaultDescription"].ToString();
                    LabelPLCKeySwitch.Text = reader["PLCKeySwitchDescription"].ToString();
                }
                catch
                { }
            }

            reader.Close();
            connection.Close();

            if (IsPostBack)
                return;

            Raptor cs2 = new Raptor();
            string connectionString1 = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection1;
            connection1 = new SqlConnection(connectionString1);
            // Open the connection.
            connection1.Open();

            SqlCommand command1 = new SqlCommand("Select * from RaptorCommSettings", connection1);
            SqlDataReader reader1 = command1.ExecuteReader();
            reader1.Read();
            if (reader1.HasRows)
            {
                try
                {
                    TextBoxIP.Text = reader1["PLCIPAddress"].ToString();
                    TextBoxSlot.Text = reader1["PLCProcessorSlot"].ToString();
                    TextBoxTimeout.Text = reader1["PLCTimeout"].ToString();
                }
                catch
                { }
            }

            reader1.Close();
            connection1.Close();
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Access != 1)
                return;
            // this.Validate();
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("update RaptorCommSettings set PLCIPAddress='" + TextBoxIP.Text + "',PLCProcessorSlot=" + TextBoxSlot.Text + ",PLCTimeout=" + TextBoxTimeout.Text, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand command = new SqlCommand("Select PollCounter from RaptorCommSettings", connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            LabelPollCounter.Text = reader["pollcounter"].ToString();
            ListBox1.DataBind();
            reader.Close();
            connection.Close();
        }

        protected void Timer3_Tick(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            try
            {
                ServiceController svcController = new ServiceController("RaptorComm");
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
            if (LabelServiceStatus.Text == "Not Installed" || LabelServiceStatus.Text == "Stopped")
            {
                SqlCommand command1 = new SqlCommand("Update RaptorCommSettings set PLCLED=8,PLCFault=5,PLCKeyswitch=0", connection);
                command1.ExecuteNonQuery();
                return;
            }

            SqlCommand command = new SqlCommand("Select * from RaptorCommSettings,PLCLED,PLCFault,PLCKeyswitch where raptorcommsettings.plcled=plcled.plcled and raptorcommsettings.plcfault=plcfault.plcfault and raptorcommsettings.plckeyswitch=plckeyswitch.plckeyswitch", connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                try
                {
                    LabelPLCLED.Text = reader["PLCLEDDescription"].ToString();
                    LabelPLCFault.Text = reader["PLCFaultDescription"].ToString();
                    LabelPLCKeySwitch.Text = reader["PLCKeySwitchDescription"].ToString();
                }
                catch
                { }
            }

            reader.Close();
            connection.Close();
            Timer3.Interval = 2000;
        }

        protected void ButtonStart_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceController svcController2 = new ServiceController("RaptorComm");
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
                SqlCommand command1 = new SqlCommand("Insert into RaptorCommLog select getdate(),'" + ex.Message.Replace("'", "''") + "'", connection);
                command1.ExecuteNonQuery();
                connection.Close();
            }
        }

        protected void ButtonStop_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceController svcController1 = new ServiceController("RaptorComm");
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
                SqlCommand command1 = new SqlCommand("Insert into RaptorCommLog select getdate(),'" + ex.Message.Replace("'", "''") + "'", connection);
                command1.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}