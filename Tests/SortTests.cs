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
        [TestMethod, TestCategory("Get")]
        public void InitFromDb()
        {
            List<Sort> sorts = new List<Sort>();
            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

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
            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

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

            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();
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
            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

            Assert.IsTrue(Sort.GetDataRequestsSortColumns(con) > 0);
            Assert.IsTrue(!string.IsNullOrEmpty(Sort.DataRequestsSortSQL));
        }
    }
}