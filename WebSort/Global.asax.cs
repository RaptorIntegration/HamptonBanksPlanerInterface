using Mighty;

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using WebSort.Model;

namespace WebSort
{
    public class Global : System.Web.HttpApplication
    {
        public static Logix.Controller MyPLC;
        private static NLog.Logger Logger;
        public const string MightyConString = "ProviderName=System.Data.SqlClient;Server=(local)\\SQLEXPRESS;Initial Catalog=RaptorWebSort;Integrated Security=True;MultipleActiveResultSets=True;pooling=True";
        public const string ConnectionString = "Server=(local)\\SQLEXPRESS;Initial Catalog=RaptorWebSort;Integrated Security=True;MultipleActiveResultSets=True;pooling=True";
        public static string Version { get; set; }

        public static bool OnlineSetup { get; set; }

        public static string UserName { get; set; }

        public static void GetOnlineSetup()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT OnlineSetup FROM WebSortSetup", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            OnlineSetup = GetValue<bool>(reader, "OnlineSetup");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get SecurityAccess for given Screen
        /// </summary>
        /// <param name="ScreenName">Current Screen</param>
        /// <param name="UserName">Current User</param>
        /// <returns>Current User</returns>
        public static User GetSecurity(string ScreenName, string name)
        {
            MightyOrm<User> mighty = new MightyOrm<User>(
                MightyConString,
                columns: "UserID, Access,");
            UserName = string.IsNullOrEmpty(name) ? "Operator" : name;

            string Sql = "SELECT Users.UserID, SecurityAccess AS Access FROM Users, SecurityScreenAccess " +
                $"WHERE ScreenName LIKE '%{ScreenName}%' AND Users.UserName LIKE '%{UserName}%' " +
                "AND Users.UserID = SecurityScreenAccess.UserID";
            return mighty.Query(Sql).First();
        }

        /// <summary>
        /// Safely read value from database and convert any nulls to default for type
        /// </summary>
        /// <typeparam name="T">Column Type</typeparam>
        /// <param name="reader">SqlDataReader containing the data you want to extract</param>
        /// <param name="colName">Name of database column</param>
        /// <returns>Value from database if not null, else default for type</returns>
        public static T GetValue<T>(SqlDataReader reader, string colName)
        {
            var value = reader[colName];
            return value == DBNull.Value ? default(T) : (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Log error to file in C://RaptorErrorLogs/WebSort-{ShortDate}.txt
        /// </summary>
        /// <param name="Message">Short message to include in log</param>
        /// <param name="Ex">The thrown exception</param>
        public static void LogError(Exception Ex)
        {
            Logger.Error(Ex);
        }

        private static void GetVersion()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(path);
            Version = Regex.Replace(version.ProductVersion, "([!@#$%^&*+]|.Branch|.Sha|.[a-zA-Z0-9]{32}$)", "");
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            if (!Debugger.IsAttached)
            {
                MyPLC = new Logix.Controller();
            }

            Logger = NLog.LogManager.GetCurrentClassLogger();

            GetVersion();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session.Timeout = 525600;  // don't want the session data to timeout (525600 = 1 year)
        }
    }
}