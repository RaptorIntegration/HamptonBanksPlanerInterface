using System.Collections.Generic;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using WebSort;
using WebSort.Model;

namespace Tests
{
    [TestClass]
    public class BinTests
    {
        public static SqlConnection con { get; set; }

        public BinTests()
        {
            con = new SqlConnection(Global.ConnectionString);
            con.Open();
        }

        [TestMethod, TestCategory("Get")]
        public void InitFromDb()
        {
            List<Bin> bins = new List<Bin>();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bins", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        bins.Add(new Bin(reader));
                    }
                }
            }

            Assert.IsTrue(bins.Count > 0);
        }

        [TestMethod, TestCategory("Get")]
        public void GetData()
        {
            List<Bin> bins = new List<Bin>();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bins", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    bins = Bin.PopulateBinList(reader);
                }
            }

            Assert.IsTrue(bins.Count > 0);
        }

        [TestMethod, TestCategory("Update")]
        public void UpdateLabels()
        {
            IEnumerable<Edit> edit = new List<Edit>(){
                new Edit()
                {
                    EditedVal = ";",
                    Previous = ","
                }
            };

            Bin.UpdateLabels(edit, con);

            using SqlCommand cmd = new SqlCommand("SELECT * FROM Bins WHERE ProductsLabel LIKE '%;%'", con);
            using SqlDataReader reader = cmd.ExecuteReader();
            Assert.IsTrue(reader.HasRows);

            edit = new List<Edit>(){
                new Edit()
                {
                    EditedVal = ",",
                    Previous = ";"
                }
            };
            Bin.UpdateLabels(edit, con);
        }

        [TestMethod, TestCategory("DataRequest")]
        public void GetDataRequestsSortColumns()
        {
            Assert.IsTrue(Bin.GetDataRequestsBinColumns(con) > 0);
            Assert.IsTrue(!string.IsNullOrEmpty(Bin.DataRequestBinSQL));
        }

        [TestMethod, TestCategory("DataRequest")]
        public void DataRequestInsert()
        {
            List<Bin> bins = new List<Bin>();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bins", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        bins.Add(new Bin(reader));
                    }
                }
            }

            Assert.IsTrue(bins.Count > 0);

            Map map = new Map();
            Map.GetDBProductMapBin(con, bins[0], map);

            Assert.IsTrue(Bin.DataRequestInsert(con, bins[0], map, bins[0].BinStatus, Ack: false));

            using (SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM DataRequestsBin WHERE BinID={bins[0].BinID} AND Processed = 0", con))
                Assert.IsTrue((int)cmd.ExecuteScalar() > 0);

            using (SqlCommand cmd = new SqlCommand($"DELETE FROM DataRequestsBin WHERE Processed = 0 AND BinID={bins[0].BinID}", con))
                Assert.IsTrue(cmd.ExecuteNonQuery() > 0);
        }
    }
}