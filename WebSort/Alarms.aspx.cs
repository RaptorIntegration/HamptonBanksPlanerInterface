using System;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class Alarms : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Alarms", User.Identity.Name);
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

            if (IsPostBack)
                return;
        }

        [WebMethod]
        public static string GetSecurity()
        {
            return CurrentUser.Access.ToString();
        }

        [WebMethod]
        public static string GetAlarms()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Alarm.GetAll());
        }

        [WebMethod]
        public static string GetAlarmSettings()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(AlarmSetting.GetAll());
        }

        [WebMethod]
        public static string GetCurrentAlarms()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Alarm.GetAllCurrent());
        }

        [WebMethod]
        public static string GetAlarmHistory()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Alarm.GetAllHistory());
        }

        [WebMethod]
        public static string GetDefaults()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(AlarmDefault.GetAll());
        }

        [WebMethod]
        public static string GetGeneral()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(AlarmGeneralSetting.GetAll());
        }

        [WebMethod]
        public static string GetDisplayLog()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Alarm.DisplayLog.GetTop50());
        }

        [WebMethod]
        public static string GetServices()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();

            ServiceController contollerMain = new ServiceController("RaptorDisplay");
            ServiceController contollerMarq = new ServiceController("RaptorDisplayViewMarq");
            ServiceController contollerMaster = new ServiceController("RaptorDisplayInfomaster");

            string[] Status = new string[3];

            try
            {
                Status[0] = contollerMain != null ? contollerMain.Status.ToString() : "Not Installed";
            }
            catch
            {
                Status[0] = "Not Installed";
            }
            try
            {
                Status[1] = contollerMarq != null ? contollerMarq.Status.ToString() : "Not Installed";
            }
            catch
            {
                Status[1] = "Not Installed";
            }
            try
            {
                Status[2] = contollerMaster != null ? contollerMaster.Status.ToString() : "Not Installed";
            }
            catch
            {
                Status[2] = "Not Installed";
            }

            return s.Serialize(Status);
        }

        [WebMethod]
        public static string SaveSettings(AlarmSetting[] settings)
        {
            SaveResponse response = new SaveResponse("AlarmSettings");

            foreach (AlarmSetting setting in settings)
            {
                try
                {
                    setting.Save();
                    response.AddEdits(setting.EditsList);
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving");
                    return SaveResponse.Serialize(response);
                }
            }

            response.Good("Alarm Properties Saved");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveDefaults(AlarmDefault[] defaults)
        {
            SaveResponse response = new SaveResponse("AlarmDefaults");

            foreach (AlarmDefault d in defaults)
            {
                try
                {
                    d.Save();
                    response.AddEdits(d.EditsList);
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving");
                    return SaveResponse.Serialize(response);
                }
            }

            response.Good("Alarm Properties Saved");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveMultiRow(bool MultipleRows)
        {
            SaveResponse response = new SaveResponse();

            try
            {
                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE AlarmGeneralSettings SET multilinedefaults=@Multi", con))
                    {
                        cmd.Parameters.AddWithValue("@Multi", MultipleRows);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving");
                return SaveResponse.Serialize(response);
            }

            response.Good("Alarm Properties Saved");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveGeneral(AlarmGeneralSetting general)
        {
            SaveResponse response = new SaveResponse("AlarmGeneralSettings");

            try
            {
                general.Save();
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving");
                return SaveResponse.Serialize(response);
            }

            response.Good("Alarm Properties Saved");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string StartService(string service)
        {
            SaveResponse response = new SaveResponse();

            try
            {
                ServiceController controller = new ServiceController(service);
                if (controller != null && controller.Status == ServiceControllerStatus.Stopped)
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception ex)
            {
                Global.LogError(ex);

                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("Insert into RaptorDisplayLog select getdate(),@message", con))
                    {
                        cmd.Parameters.AddWithValue("@message", ex.Message.ToString());
                        cmd.ExecuteNonQuery();
                    }
                }

                response.Bad($"Error starting service: {service}");
                return SaveResponse.Serialize(response);
            }

            response.Good($"Service: {service} started");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string StopService(string service)
        {
            SaveResponse response = new SaveResponse();

            try
            {
                ServiceController controller = new ServiceController(service);
                if (controller != null && controller.Status == ServiceControllerStatus.Running)
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception ex)
            {
                Global.LogError(ex);

                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("Insert into RaptorDisplayLog select getdate(),@message", con))
                    {
                        cmd.Parameters.AddWithValue("@message", ex.Message.ToString());
                        cmd.ExecuteNonQuery();
                    }
                }

                response.Bad($"Error stopping service: {service}");
                return SaveResponse.Serialize(response);
            }

            response.Good($"Service: {service} stopped");
            return SaveResponse.Serialize(response);
        }
    }
}