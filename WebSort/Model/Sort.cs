using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;

namespace WebSort.Model
{
    public class Sort
    {
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
        public long SortStamps { get; set; }
        public string SortStampsLabel { get; set; }
        public int SortSprays { get; set; }
        public string SortSpraysLabel { get; set; }
        public int BinID { get; set; }
        public int TrimFlag { get; set; }
        public int SecProdID { get; set; }
        public short SecSize { get; set; }
        public string ProductsLabel { get; set; }
        public List<Stamp> SelectedStamps { get; set; }
        public bool Changed { get; set; }
        public ProductLengths ProdLen { get; set; }
        public List<Edit> EditsList { get; set; }

        public Sort()
        {
            EditsList = new List<Edit>();
        }

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
            List<Stamp> stamps = Stamp.GetStamps();
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
                    SelectedStamps = Stamp.GetSelectedStamps(Global.GetValue<uint>(reader, "SortStamps"), stamps)
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
    }
}