using System;
using System.Data.SqlClient;

namespace WebSort
{
    public partial class WebSort : System.Web.UI.MasterPage
    {
        public static string GetVersion()
        {
            return Global.Version;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = 0", con))
                    cmd.ExecuteNonQuery();
                using (SqlCommand cmd = new SqlCommand("select * from reportheader", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Label1.Text = $"For {reader["companyname"]}, {reader["companycity"]}, {reader["companyregion"]}";
                        }
                    }
                }
            }
            UpdateHeader();
        }

        protected void UpdateHeader()
        {
            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

            try
            {
                using SqlCommand cmd = new SqlCommand("Select CurrentMessage,CurrentAlarmID,CurrentLPM,MainEncoderActual,SkipEncoderPosition from CurrentState with(NOLOCK)", con);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (int.Parse(reader["CurrentAlarmID"].ToString()) < 1000)
                        {
                            using SqlCommand cmdSeverity = new SqlCommand($"Select Severity from alarmsettings  with(NOLOCK) where alarmid={reader["CurrentAlarmID"]}", con);
                            using SqlDataReader readerSeverity = cmdSeverity.ExecuteReader();
                            if (readerSeverity.HasRows)
                            {
                                while (readerSeverity.Read())
                                {
                                    int Severity = Convert.ToInt32(readerSeverity["Severity"].ToString());
                                    string SeverityClass = Severity == 2 ? "severity-2" : Severity == 1 ? "severity-1" : "severity-0";
                                    LabelAlphaMessage.Attributes.Add("class", SeverityClass);
                                }
                            }
                        }
                        else
                        {
                            LabelAlphaMessage.Attributes.Add("class", "severity-0");
                        }
                        LabelAlphaMessage.Text = reader["CurrentMessage"].ToString();
                        LabelLPM.Text = $"LPM: {reader["CurrentLPM"]}";
                        if (int.Parse(reader["CurrentLPM"].ToString()) <= 10)
                            LabelEncoderPosition.Text = $"MAIN ENCODER: {reader["MainEncoderActual"]}";
                        else
                            LabelEncoderPosition.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
            }
        }

        protected void TimerHeader_Tick(object sender, EventArgs e)
        {
            UpdateHeader();
        }
    }
}