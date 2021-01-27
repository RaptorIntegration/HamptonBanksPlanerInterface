using System.Collections.Generic;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using WebSort;
using WebSort.Model;

namespace Tests
{
    [TestClass]
    public class SortTests
    {
        public static SqlConnection con { get; set; }

        public SortTests()
        {
            con = new SqlConnection(Global.ConnectionString);
            con.Open();
        }

        [TestMethod, TestCategory("Get")]
        public void InitFromDb()
        {
            List<Sort> sorts = new List<Sort>();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Sorts", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sorts.Add(new Sort(reader));
                    }
                }
            }

            Assert.IsTrue(sorts.Count > 0);
        }

        [TestMethod, TestCategory("Get")]
        public void GetData()
        {
            List<Sort> sorts = new List<Sort>();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Sorts", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    sorts = Sort.PopulateSortList(reader);
                }
            }

            Assert.IsTrue(sorts.Count > 0);
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

            Sort.UpdateLabels(edit, con);

            using SqlCommand cmd = new SqlCommand("SELECT * FROM SORTS WHERE ProductsLabel LIKE '%;%'", con);
            using SqlDataReader reader = cmd.ExecuteReader();
            Assert.IsTrue(reader.HasRows);

            edit = new List<Edit>(){
                new Edit()
                {
                    EditedVal = ",",
                    Previous = ";"
                }
            };
            Sort.UpdateLabels(edit, con);
        }

        [TestMethod, TestCategory("DataRequest")]
        public void GetDataRequestsSortColumns()
        {
            Assert.IsTrue(Sort.GetDataRequestsSortColumns(con) > 0);
            Assert.IsTrue(!string.IsNullOrEmpty(Sort.DataRequestsSortSQL));
        }

        [TestMethod, TestCategory("DataRequest")]
        public void DataRequestInsert()
        {
            List<Sort> sorts = new List<Sort>();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Sorts", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sorts.Add(new Sort(reader));
                    }
                }
            }

            Assert.IsTrue(sorts.Count > 0);

            Map map = new Map();
            Map.GetDBProductMapSort(con, sorts[0], map, Recipe.GetEditingRecipe().RecipeID);

            Assert.IsTrue(Sort.DataRequestInsert(con, sorts[0], map, Ack: false));

            using (SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM DataRequestsSort WHERE SortID={sorts[0].SortID} AND Processed = 0", con))
                Assert.IsTrue((int)cmd.ExecuteScalar() > 0);

            using (SqlCommand cmd = new SqlCommand($"DELETE FROM DataRequestsSort WHERE Processed = 0 AND SortID={sorts[0].SortID}", con))
                Assert.IsTrue(cmd.ExecuteNonQuery() > 0);

            Assert.IsTrue(Sort.DataRequestInsert(con, sorts[0], map, Ack: false, ZeroOut: true));

            using (SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM DataRequestsSort WHERE SortID={sorts[0].SortID} AND Processed = 0 AND LengthMap = 0", con))
                Assert.IsTrue((int)cmd.ExecuteScalar() > 0);

            using (SqlCommand cmd = new SqlCommand($"DELETE FROM DataRequestsSort WHERE Processed = 0 AND SortID={sorts[0].SortID}", con))
                Assert.IsTrue(cmd.ExecuteNonQuery() > 0);

            Assert.IsTrue(Sort.DataRequestInsert(con, sorts[0], map, Ack: false, ProductsOnlyZero: true));

            using (SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM DataRequestsSort WHERE SortID={sorts[0].SortID} AND Processed = 0 AND ProductsOnly = 0", con))
                Assert.IsTrue((int)cmd.ExecuteScalar() > 0);

            using (SqlCommand cmd = new SqlCommand($"DELETE FROM DataRequestsSort WHERE Processed = 0 AND SortID={sorts[0].SortID}", con))
                Assert.IsTrue(cmd.ExecuteNonQuery() > 0);
        }
    }
}