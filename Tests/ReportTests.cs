using System;
using System.Data.SqlClient;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using WebSort;

namespace Tests
{
    [TestClass]
    public class ReportTests
    {
        public static SqlConnection Con { get; set; }

        public ReportTests()
        {
            Con = new SqlConnection(Global.ConnectionString);
            Con.Open();
        }

        public SqlDataReader TestStoredProcedure(string Procedure)
        {
            using SqlCommand cmd = new SqlCommand($"EXEC {Procedure}", Con);
            return cmd.ExecuteReader();
        }

        [TestMethod, TestCategory("All")]
        public void Reports()
        {
            using SqlCommand cmd = new SqlCommand("SELECT NAME FROM SYS.PROCEDURES WHERE NAME Like 'Rep%'", Con);
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    try
                    {
                        Assert.IsTrue(TestStoredProcedure(reader["NAME"].ToString()).HasRows);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}