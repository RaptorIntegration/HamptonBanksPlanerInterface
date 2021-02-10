using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace WebSort.Model
{
    public class Bin
    {
        public Bin()
        {
            EditsList = new List<Edit>();
        }

        public Bin(SqlDataReader reader)
        {
            string[] except = new string[] { "SelectedStamps", "ProdLen", "Changed", "EditsList", "ProductMapCount", "DataRequestBinSQL" };
            foreach (PropertyInfo property in typeof(Bin).GetProperties().Where(p => !except.Contains(p.Name)))
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

        public int BinID { get; set; }
        public string BinLabel { get; set; }
        public int BinStatus { get; set; }
        public string BinStatusLabel { get; set; }
        public int BinSize { get; set; }
        public int BinCount { get; set; }
        public bool RW { get; set; }
        public int BinStamps { get; set; }
        public string BinStampsLabel { get; set; }

        /// <summary>
        /// Premium Stamp (0 - 1)
        /// </summary>
        public bool BinSprays { get; set; }

        public string BinSpraysLabel { get; set; }
        public int BinPercent { get; set; }
        public int SortID { get; set; }
        public bool TrimFlag { get; set; }
        public int SecProdID { get; set; }
        public short SecSize { get; set; }
        public short SecCount { get; set; }
        public string ProductsLabel { get; set; }
        public ProductLengths ProdLen { get; set; }
        public bool Changed { get; set; }
        public List<Edit> EditsList { get; set; }
        public static int ProductMapCount { get; set; }
        public static string DataRequestBinSQL { get; set; }

        public static List<Bin> PopulateBinList(SqlDataReader reader)
        {
            List<Bin> BinList = new List<Bin>();
            while (reader.Read())
            {
                BinList.Add(new Bin
                {
                    BinID = Global.GetValue<int>(reader, "BinID"),
                    BinLabel = Global.GetValue<string>(reader, "BinLabel"),
                    BinStatus = Global.GetValue<int>(reader, "BinStatus"),
                    BinStatusLabel = Global.GetValue<string>(reader, "BinStatusLabel"),
                    BinSize = Global.GetValue<int>(reader, "BinSize"),
                    BinCount = Global.GetValue<int>(reader, "BinCount"),
                    BinPercent = Global.GetValue<int>(reader, "BinPercent"),
                    SortID = Global.GetValue<int>(reader, "SortID"),
                    ProductsLabel = Global.GetValue<string>(reader, "ProductsLabel"),
                    SecProdID = Global.GetValue<int>(reader, "SecProdID"),
                    SecSize = Global.GetValue<short>(reader, "SecSize"),
                    SecCount = Global.GetValue<short>(reader, "SecCount"),
                    BinStamps = Global.GetValue<int>(reader, "BinStamps"),
                    BinSprays = Global.GetValue<bool>(reader, "BinSprays"),
                    Changed = false
                });
            }

            return BinList;
        }

        public static void UpdateLabels(IEnumerable<Edit> edits, SqlConnection con)
        {
            if (edits.Any())
            {
                foreach (Edit edit in edits)
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Bins SET ProductsLabel = REPLACE(ProductsLabel, @Old, @New)", con))
                    {
                        cmd.Parameters.AddWithValue("@New", edit.EditedVal);
                        cmd.Parameters.AddWithValue("@Old", edit.Previous);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static int GetDataRequestsBinColumns(SqlConnection con)
        {
            IEnumerable<string> Columns = null;

            using (SqlCommand cmd = new SqlCommand("Select TOP 1 * FROM DataRequestsBin", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    Columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                }
            }

            if (Columns?.Any() == true)
            {
                DataRequestBinSQL = "INSERT INTO datarequestsbin SELECT getdate(),";
                foreach (string col in Columns.Where(c => c != "TimeStamp" && c != "ID"))
                {
                    DataRequestBinSQL += $"@{col},";
                }
                DataRequestBinSQL = DataRequestBinSQL.Remove(DataRequestBinSQL.Length - 1, 1);
                DataRequestBinSQL += ";SELECT id = (select max(id) FROM datarequestsbin with(NOLOCK))";

                return Columns.Count(c => c.Contains("ProductMap") && !c.EndsWith("c")) / 2;
            }
            else
            {
                return -1;
            }
        }

        public static bool DataRequestInsert(SqlConnection con, Bin Item, Map map, int OriginalStatus, bool CommSettings = true, bool Ack = true)
        {
            if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("UPDATE RaptorCommSettings SET DataRequests = DataRequests | 1", con);
                cmd.ExecuteNonQuery();
            }

            if (ProductMapCount <= 0 || string.IsNullOrEmpty(DataRequestBinSQL))
            {
                ProductMapCount = GetDataRequestsBinColumns(con);
            }
            if (ProductMapCount == -1)
            {
                return false;
            }

            using (SqlCommand cmd = new SqlCommand(DataRequestBinSQL, con))
            {
                cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                cmd.Parameters.AddWithValue("@BinLabel", Item.BinLabel);
                cmd.Parameters.AddWithValue("@BinStatus", Item.BinStatus);
                cmd.Parameters.AddWithValue("@BinSize", Item.BinSize);
                cmd.Parameters.AddWithValue("@BinCount", Item.BinCount);
                for (int i = 0; i < ProductMapCount; i++)
                {
                    cmd.Parameters.AddWithValue($"@ProductMap{i}", (long)map.ProductMap[i]);
                    cmd.Parameters.AddWithValue($"@ProductMap{i}Old", (long)map.ProductMap[i]);
                }
                cmd.Parameters.AddWithValue("@LengthMap", (long)map.LengthMap);
                cmd.Parameters.AddWithValue("@BinStamps", Item.BinStamps);
                cmd.Parameters.AddWithValue("@BinSprays", Convert.ToInt32(Item.BinSprays));
                cmd.Parameters.AddWithValue("@SortID", Item.SortID);
                cmd.Parameters.AddWithValue("@SecProdID", Item.SecProdID);
                cmd.Parameters.AddWithValue("@SecSize", (Item.SecSize * Item.BinSize) / 100);
                cmd.Parameters.AddWithValue("@SecCount", Item.SecCount);
                cmd.Parameters.AddWithValue("@TrimFlag", Item.TrimFlag);
                cmd.Parameters.AddWithValue("@RW", 0);
                cmd.Parameters.AddWithValue("@ProductsOnly", 2);
                cmd.Parameters.AddWithValue("@Write", 1);
                cmd.Parameters.AddWithValue("@Processed", 0);

                // Spare
                if (Item.BinStatus == 0)
                {
                    for (int i = 0; i < ProductMapCount; i++)
                    {
                        cmd.Parameters[$"@ProductMap{i}"].Value = 0;
                    }
                    cmd.Parameters["@lengthMap"].Value = 0;
                    cmd.Parameters["@BinLabel"].Value = "";
                    cmd.Parameters["@BinSize"].Value = 0;
                    cmd.Parameters["@BinCount"].Value = 0;
                    cmd.Parameters["@SortID"].Value = 0;
                }
                // If products for this bin were edited
                if (Item.ProdLen != null)
                {
                    for (int i = 0; i < ProductMapCount; i++)
                    {
                        cmd.Parameters[$"@ProductMap{i}Old"].Value = (long)map.ProductMapOld[i];
                    }
                }

                if (Item.BinLabel.Length > 20)
                {
                    cmd.Parameters["@BinLabel"].Value = Item.BinLabel.Remove(20);
                }

                //don't send a message to the PLC for a virtual bay that is being set to spare or full
                if (OriginalStatus != 5 || (OriginalStatus == 5 && Item.BinStatus >= 5))
                {
                    try
                    {
                        using SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        if (Ack && !Raptor.MessageAckConfirm("datarequestsbin", int.Parse(reader["id"].ToString())))
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        return false;
                    }
                }
            }

            if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("UPDATE RaptorCommSettings SET datarequests = datarequests-1 WHERE (datarequests & 1)=1", con);
                cmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}