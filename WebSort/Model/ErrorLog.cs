using Mighty;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace WebSort.Model
{
    public class ErrorLog
    {
        public int ID { get; set; }

        public string MachineName { get; set; }

        public DateTime Logged { get; set; }

        public string LoggedString => Logged.ToString("MMM dd hh:mm:ss tt");

        public string Level { get; set; }

        public string Message { get; set; }

        public string Logger { get; set; }

        public string Callsite { get; set; }

        public string Exception { get; set; }

        public static IEnumerable<ErrorLog> GetAll()
        {
            MightyOrm<ErrorLog> db = new MightyOrm<ErrorLog>(Global.MightyConString, "ErrorLogs", "ID");
            return db.All().OrderByDescending(o => o.Logged);
        }

        public static void Flush()
        {
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("DELETE FROM ErrorLogs WHERE ID < (SELECT TOP 25 max(ID) - 25 FROM ErrorLogs)", con))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}