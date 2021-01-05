using System;
using System.Data.SqlClient;
using System.Threading;

namespace WebSort
{
    public class Raptor : BasePage
    {
        public static string GetConnectionString()
        {
            return "Server=(local)\\SQLEXPRESS;Initial Catalog=RaptorWebSort;Integrated Security=True;MultipleActiveResultSets=True;pooling=True";
        }

        public string MsgBoxInAsp(string MessageToDisplay)
        {
            return "<script language='javascript'>window.alert('" + MessageToDisplay + "')</script>";
        }

        public static bool MessageAckConfirm(string tablename, int id)
        {
            //bool retry = true;
            int counter = 0;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            while (true)
            {
                SqlCommand cmd = new SqlCommand("select processed from " + tablename + " with(NOLOCK) where id = " + id.ToString(), connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader["processed"].ToString() == "False")
                {
                    counter++;
                    Thread.Sleep(100);
                    reader.Close();
                    if (counter >= 50)  // 5 seconds
                    {
                        connection.Close();
                        return false;
                    }
                }
                else if (reader["processed"].ToString() == "True")
                {
                    reader.Close();
                    connection.Close();
                    //retry = false;
                    break;
                }
            }

            //connection.Close();
            return true;
        }
    }
}