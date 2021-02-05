using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace WebSort.Model
{
    public class Sort
    {
        public Sort()
        {
            EditsList = new List<Edit>();
        }

        public Sort(SqlDataReader reader)
        {
            string[] except = new string[] { "SelectedStamps", "ProdLen", "Changed", "EditsList", "ProductMapCount", "DataRequestsSortSQL" };
            foreach (PropertyInfo property in typeof(Sort).GetProperties().Where(p => !except.Contains(p.Name)))
            {
                try
                {
                    var val = reader[property.Name] is DBNull ? null : reader[property.Name];

                    if (val != null)
                    {
                        try
                        {
                            property.SetValue(this, Convert.ChangeType(val, property.PropertyType));
                        }
                        catch (Exception ex)
                        {
                            Global.LogError(ex);
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    throw;
                }
            }
            Changed = false;
            EditsList = new List<Edit>();
        }

        [Key]
        public int SortID { get; set; }

        public string SortLabel { get; set; }
        public bool Active { get; set; }
        public int SortSize { get; set; }
        public int Zone1Start { get; set; }
        public int Zone1Stop { get; set; }
        public int Zone2Start { get; set; }
        public int Zone2Stop { get; set; }
        public int PkgsPerSort { get; set; }
        public bool RW { get; set; }
        public int OrderCount { get; set; }
        public int SortStamps { get; set; }
        public string SortStampsLabel { get; set; }
        public int SortSprays { get; set; }
        public string SortSpraysLabel { get; set; }
        public int BinID { get; set; }
        public int TrimFlag { get; set; }
        public int SecProdID { get; set; }
        public short SecSize { get; set; }
        public string ProductsLabel { get; set; }
        public bool Changed { get; set; }
        public ProductLengths ProdLen { get; set; }
        public List<Edit> EditsList { get; set; }

        public static int ProductMapCount { get; set; }

        public static string DataRequestsSortSQL = @"
            INSERT INTO DataRequestsSort SELECT GETDATE(),
                @SortID, @SortLabel, @SortSize, @PkgsPerSort, @OrderCount, @SecProdID, @SecSize,
                @ProductMap0, @ProductMap1, @ProductMap2, @ProductMap3, @ProductMap4, @ProductMap5,
                @LengthMap, @ProductMap0c, @ProductMap1c, @ProductMap2c, @LengthMapc,
                @ProductMap0Old, @ProductMap1Old, @ProductMap2Old, @ProductMap3Old, @ProductMap4Old, @ProductMap5Old,
                @SortStamps, @SortSprays, @Zone1, @Zone2, @TrimFlag, @RW, @Active, @ProductsOnly, @Write, @Processed;
            select id=(select max(id) from datarequestssort with(NOLOCK))";

        public static List<Sort> PopulateSortList(SqlDataReader reader)
        {
            List<Sort> SortList = new List<Sort>();
            while (reader.Read())
            {
                SortList.Add(new Sort
                {
                    SortID = Global.GetValue<int>(reader, "SortID"),
                    SortLabel = Global.GetValue<string>(reader, "SortLabel"),
                    Active = Global.GetValue<bool>(reader, "Active"),
                    SortSize = Global.GetValue<int>(reader, "SortSize"),
                    Zone1Start = Global.GetValue<int>(reader, "Zone1Start"),
                    Zone1Stop = Global.GetValue<int>(reader, "Zone1Stop"),
                    Zone2Start = Global.GetValue<int>(reader, "Zone2Start"),
                    Zone2Stop = Global.GetValue<int>(reader, "Zone2Stop"),
                    PkgsPerSort = Global.GetValue<int>(reader, "PkgsPerSort"),
                    RW = Global.GetValue<bool>(reader, "RW"),
                    OrderCount = Global.GetValue<int>(reader, "OrderCount"),
                    SecProdID = Global.GetValue<int>(reader, "SecProdID"),
                    SecSize = Global.GetValue<short>(reader, "SecSize"),
                    ProductsLabel = Global.GetValue<string>(reader, "ProductsLabel"),
                    Changed = false,
                    SortStamps = Global.GetValue<int>(reader, "SortStamps"),
                    SortStampsLabel = Global.GetValue<string>(reader, "SortSpraysLabel"),
                    SortSprays = Global.GetValue<int>(reader, "SortSprays")
                });
            }

            return SortList;
        }

        public static void UpdateLabels(IEnumerable<Edit> edits, SqlConnection con)
        {
            if (edits.Any())
            {
                foreach (Edit edit in edits)
                {
                    using SqlCommand cmd = new SqlCommand("UPDATE Sorts SET ProductsLabel = REPLACE(ProductsLabel, @Old, @New)", con);
                    cmd.Parameters.AddWithValue("@New", edit.EditedVal);
                    cmd.Parameters.AddWithValue("@Old", edit.Previous);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int GetDataRequestsSortColumns(SqlConnection con)
        {
            IEnumerable<string> Columns = null;

            using (SqlCommand cmd = new SqlCommand("Select TOP 1 * FROM DataRequestsSort", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    Columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                }
            }

            if (Columns?.Any() == true)
            {
                DataRequestsSortSQL = "INSERT INTO DataRequestsSort SELECT getdate(),";
                foreach (string col in Columns.Where(c => c != "TimeStamp" && c != "ID"))
                {
                    DataRequestsSortSQL += $"@{col},";
                }

                // Remove comma
                DataRequestsSortSQL = DataRequestsSortSQL.Remove(DataRequestsSortSQL.Length - 1, 1);
                DataRequestsSortSQL += ";SELECT id = (select max(id) FROM DataRequestsSort with(NOLOCK))";

                return Columns.Count(c => c.Contains("ProductMap") && !c.EndsWith("c")) / 2;
            }
            else
            {
                return -1;
            }
        }

        public static bool DataRequestInsert(SqlConnection con, Sort sort, Map map, bool CommSettings = true, bool Ack = true, bool ZeroOut = false, bool ProductsOnlyZero = false)
        {
            if (CommSettings)
            {
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2", con))
                    cmd.ExecuteNonQuery();
            }

            if (ProductMapCount <= 0 || string.IsNullOrEmpty(DataRequestsSortSQL))
            {
                ProductMapCount = GetDataRequestsSortColumns(con);
            }
            if (ProductMapCount == -1)
            {
                return false;
            }

            using (SqlCommand cmdRequest = new SqlCommand(DataRequestsSortSQL, con))
            {
                cmdRequest.Parameters.AddWithValue("@SortID", sort.SortID);
                cmdRequest.Parameters.AddWithValue("@SortLabel", sort.SortLabel);
                cmdRequest.Parameters.AddWithValue("@SortSize", sort.SortSize);
                cmdRequest.Parameters.AddWithValue("@PkgsPerSort", sort.PkgsPerSort);
                cmdRequest.Parameters.AddWithValue("@OrderCount", sort.OrderCount);
                cmdRequest.Parameters.AddWithValue("@SecProdID", sort.SecProdID);
                cmdRequest.Parameters.AddWithValue("@SecSize", sort.SecSize);
                cmdRequest.Parameters.AddWithValue("@LengthMap", ZeroOut ? 0 : Convert.ToInt64(map.LengthMap));
                cmdRequest.Parameters.AddWithValue("@SortStamps", sort.SortStamps);
                cmdRequest.Parameters.AddWithValue("@SortSprays", sort.SortSprays);
                cmdRequest.Parameters.AddWithValue("@Zone1", (sort.Zone1Stop * 256) + sort.Zone1Start);
                cmdRequest.Parameters.AddWithValue("@Zone2", (sort.Zone2Stop * 256) + sort.Zone2Start);
                cmdRequest.Parameters.AddWithValue("@TrimFlag", sort.TrimFlag);
                cmdRequest.Parameters.AddWithValue("@RW", sort.RW);
                cmdRequest.Parameters.AddWithValue("@Active", sort.Active);
                cmdRequest.Parameters.AddWithValue("@ProductsOnly", ProductsOnlyZero ? 0 : ZeroOut ? 1 : 2);
                cmdRequest.Parameters.AddWithValue("@Write", 1);
                cmdRequest.Parameters.AddWithValue("@Processed", 0);

                cmdRequest.Parameters.AddWithValue("@ProductMap0c", 0);
                cmdRequest.Parameters.AddWithValue("@ProductMap1c", 0);
                cmdRequest.Parameters.AddWithValue("@ProductMap2c", 0);
                cmdRequest.Parameters.AddWithValue("@LengthMapc", 0);

                if (ZeroOut)
                {
                    for (int index = 0; index < ProductMapCount; index++)
                    {
                        cmdRequest.Parameters.AddWithValue($"@ProductMap{index}", 0);
                        cmdRequest.Parameters.AddWithValue($"@ProductMap{index}Old", Convert.ToInt64(map.ProductMapOld[index]));
                    }
                }
                else
                {
                    for (int index = 0; index < ProductMapCount; index++)
                    {
                        cmdRequest.Parameters.AddWithValue($"@ProductMap{index}", Convert.ToInt64(map.ProductMap[index]));
                        cmdRequest.Parameters.AddWithValue($"@ProductMap{index}Old", Convert.ToInt64(map.ProductMapOld[index]));
                    }
                }

                try
                {
                    using SqlDataReader reader = cmdRequest.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Ack && !Raptor.MessageAckConfirm("datarequestssort", Global.GetValue<int>(reader, "id")))
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    throw;
                }
            }

            if (CommSettings)
            {
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2 where (datarequests & 2)=2", con))
                    cmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}