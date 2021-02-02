using Logix;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;

using WebSort.Model;

namespace WebSort
{
    public partial class WebSortPage : BasePage
    {
        private Thread ReportThread;
        private static User CurrentUser;

        public void LoadReport()
        {
            try
            {
                //load a dummy report so that all of the Crystal runtime files are preloaded
                CrystalReportSource1.Report.FileName = Server.MapPath(".") + "\\app_data\\Reports\\template_portrait.rpt";
                CrystalReportSource1.DataBind();
                if (ReportThread != null)
                {
                    //ReportThread.Abort();
                    ReportThread = null;
                }
            }
            catch { }
        }

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (IsPostBack) { return; }

            try
            {
                Global.GetOnlineSetup();
                CurrentUser = Global.GetSecurity("Home", User.Identity.Name);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) { return; }

            //start a separate thread so the Crystal Report Engine gets preloaded.
            if (ReportThread == null)
            {
                ReportThread = new Thread(LoadReport);
                ReportThread.Start();
            }
        }

        #region Chart stuff

        [WebMethod]
        public static string SaveColours(string[] C)
        {
            ChartData Data = new ChartData();

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                for (int i = 0; i < C.Length; i++)
                {
                    using (SqlCommand cmd = new SqlCommand($"UPDATE BinStatus SET Color='{C[i].Substring(1)}' WHERE BinStatus={i}", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            } // End connection

            JavaScriptSerializer S = new JavaScriptSerializer();
            return S.Serialize(Data);
        }

        [WebMethod]
        public static string GetColours()
        {
            List<string> Json = new List<string>();
            JavaScriptSerializer S = new JavaScriptSerializer();

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Color FROM BinStatus ORDER BY BinStatus ASC", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Json.Add(reader["color"].ToString());
                        }
                    }
                }
            }

            return S.Serialize(Json);
        }

        public class Dataset
        {
            public List<int> data { get; set; }
            public List<string> backgroundColor { get; set; }
            public List<string> borderColor { get; set; }
            public int borderWidth { get; set; }

            public Dataset()
            {
                data = new List<int>();
                backgroundColor = new List<string>();
                borderColor = new List<string>();
            }
        }

        public class ChartData
        {
            public List<string> labels { get; set; }
            public List<Dataset> datasets { get; set; }

            public ChartData()
            {
                labels = new List<string>();
                datasets = new List<Dataset> { new Dataset() };
            }
        }

        [WebMethod]
        public static string GetChartData()
        {
            ChartData Data = new ChartData();
            JavaScriptSerializer S = new JavaScriptSerializer();

            Raptor cs = new Raptor();
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT BinID, BinStatus, BinPercent FROM Bins where binstatus<5", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Data.datasets[0].data.Add(Convert.ToInt32(reader["BinPercent"].ToString()));
                            Data.datasets[0].backgroundColor.Add(reader["BinStatus"].ToString());
                            Data.datasets[0].borderColor.Add(reader["BinStatus"].ToString());
                            Data.labels.Add(reader["BinID"].ToString());
                        }
                    }
                }
            }

            Data.datasets[0].borderWidth = 1;

            return S.Serialize(Data);
        }

        [WebMethod]
        public static string GetPieData()
        {
            ChartData Data = new ChartData();
            JavaScriptSerializer S = new JavaScriptSerializer();

            Raptor cs = new Raptor();
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT BinStatus, BinStatusLabel, count(*) AS Count FROM Bins WHERE BinStatus <> 5 AND BinStatus <> 4 GROUP BY BinStatus, BinStatusLabel", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Data.labels.Add(reader["BinStatusLabel"].ToString());
                            Data.datasets[0].data.Add(Convert.ToInt32(reader["Count"].ToString()));
                            Data.datasets[0].backgroundColor.Add(reader["BinStatus"].ToString());
                            Data.datasets[0].borderColor.Add(reader["BinStatus"].ToString());
                        }
                    }
                }
            }

            Data.datasets[0].borderWidth = 2;

            return S.Serialize(Data);
        }

        #endregion Chart stuff

        #region Table

        [WebMethod]
        public static string GetData()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Bin> Sorts = new List<Bin>();

            const string sql = "SELECT [BinID], [BinLabel], [BinStatus], [BinStatusLabel], [BinSize], [BinCount], BinStamps, BinPercent, " +
                               "[SortID], SecProdID, SecSize, SecCount, [ProductsLabel] FROM [Bins] with(NOLOCK)";

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();
                using SqlCommand cmd = new SqlCommand(sql, con);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Sorts = Bin.PopulateBinList(reader);
                }
            }
            return serializer.Serialize(Sorts);
        }

        [WebMethod]
        public static string GetData1()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Bin> Sorts = new List<Bin>();

            const string sql = "SELECT [BinID], [BinLabel], [BinStatus], [BinStatusLabel], [BinSize], [BinCount], BinStamps, BinPercent, " +
                               "[SortID], SecProdID, SecSize, SecCount, [ProductsLabel] FROM [Bins] with(NOLOCK) where binstatus<5";

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();
                using SqlCommand cmd = new SqlCommand(sql, con);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Sorts = Bin.PopulateBinList(reader);
                }
            }
            return serializer.Serialize(Sorts);
        }

        [WebMethod]
        public static string GetProductList(string BinID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductLengths PL = new ProductLengths();

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(ProductLengths.BinProdMapSql, con))
                {
                    cmd.Parameters.AddWithValue("@BinID", BinID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            PL.PopulateProductList(reader);
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(ProductLengths.BinLengthMapSql, con))
                {
                    cmd.Parameters.AddWithValue("@BinID", BinID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            PL.PopulateLengthList(reader);
                        }
                    }
                }
            }
            return serializer.Serialize(PL);
        }

        [WebMethod]
        public static string GetProductGrades()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(Product.GetProductGrades());
        }

        [WebMethod]
        public static string GetIncDec()
        {
            string ret = "";
            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

            using SqlCommand cmd = new SqlCommand("SELECT IncDec FROM WEBSortSetup", con);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ret = reader["IncDec"].ToString();
                }
            }
            return ret;
        }

        [WebMethod]
        public static string Save(Bin[] Changed)
        {
            SaveResponse response = new SaveResponse("Bins");

            uint OldStamps = 0;
            int StatusOriginal = 0;

            string Update;

            if (CurrentUser.Access != 1)
            {
                response.Bad("Unauthorized!");
                return SaveResponse.Serialize(response);
            }

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                if (Changed.Length > 0)
                {
                    foreach (Bin Item in Changed)
                    {
                        Map map = new Map();

                        Item.BinStamps = Stamp.GetStampsBitMap(Item.SelectedStamps);

                        // Length and Product Map
                        if (Item.ProdLen != null)  // Products changed
                        {
                            // Only Selected Products and Lengths
                            Map.GetSelectedProductMapBin(Item, map);

                            // Product Map Old
                            Map.GetProductMapOldBin(con, Item, map);
                        }
                        else // No products changed, ProductMap = ProductMapOld
                        {
                            Map.GetDBProductMapBin(con, Item, map);
                        }

                        foreach (Edit Edit in Item.EditsList)
                        {
                            // Status
                            if (Edit.EditedCol == "BinStatus")
                            {
                                StatusOriginal = Item.BinStatus;

                                //account for settings a virtual bay full - override it to be spare
                                if (StatusOriginal == 5 && Item.BinStatus == 2)
                                {
                                    Item.BinStatusLabel = "Spare";
                                    Item.BinStatus = 0;
                                }

                                string SqlCommandString = "EXECUTE UpdateBinData @FrameStart, @BayNum, @Name, @PkgSize, @Count, @SecProdID, @SecSize, @SecCount, @RdmWidthFlag, @Status, @Stamps, @Sprays, @TrimFlag, @SortXRef";
                                for (int j = 0; j < 6; j++)
                                {
                                    SqlCommandString += ",0";
                                }
                                SqlCommandString += ",0,0,2";
                                using (SqlCommand cmd = new SqlCommand(SqlCommandString, con))
                                {
                                    cmd.Parameters.AddWithValue("@FrameStart", 0);
                                    cmd.Parameters.AddWithValue("@BayNum", Item.BinID);
                                    cmd.Parameters.AddWithValue("@Name", Item.BinLabel);
                                    cmd.Parameters.AddWithValue("@PkgSize", Item.BinSize);
                                    cmd.Parameters.AddWithValue("@Count", Item.BinCount);
                                    cmd.Parameters.AddWithValue("@SecProdID", Item.BinCount);
                                    cmd.Parameters.AddWithValue("@SecSize", Item.BinCount);
                                    cmd.Parameters.AddWithValue("@SecCount", Item.BinCount);
                                    cmd.Parameters.AddWithValue("@RdmWidthFlag", 0);
                                    cmd.Parameters.AddWithValue("@Status", Item.BinStatus);
                                    cmd.Parameters.AddWithValue("@Stamps", Item.BinStamps);
                                    cmd.Parameters.AddWithValue("@Sprays", 0);
                                    cmd.Parameters.AddWithValue("@TrimFlag", 1);
                                    cmd.Parameters.AddWithValue("@SortXRef", Item.SortID);
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Global.LogError(ex);
                                        response.Bad("Error Updating Bin Data");
                                        return SaveResponse.Serialize(response);
                                    }
                                }

                                if (Item.BinStatus == 0) //Spare
                                {
                                    if (Global.OnlineSetup)
                                    {
                                        // Send bin reset message to PLC
                                        using (SqlCommand cmdip = new SqlCommand("SELECT * from RaptorCommSettings", con))
                                        {
                                            using (SqlDataReader readerip = cmdip.ExecuteReader())
                                            {
                                                readerip.Read();
                                                if (readerip.HasRows)
                                                {
                                                    Global.MyPLC.IPAddress = readerip["PLCIPAddress"].ToString();
                                                    Global.MyPLC.Path = readerip["PLCProcessorSlot"].ToString();
                                                    Global.MyPLC.Timeout = int.Parse(readerip["PLCTimeout"].ToString());
                                                }
                                                if (Global.MyPLC.Connect() != ResultCode.E_SUCCESS)
                                                {
                                                    response.Bad("PLC Timeout");
                                                    return SaveResponse.Serialize(response);
                                                }
                                            }
                                            Tag MyTag = new Tag("Program:SorterBays.Bay[" + Item.BinID.ToString() + "].ResetActive");
                                            MyTag.DataType = Tag.ATOMIC.BOOL;
                                            try
                                            {
                                                MyTag.Value = 1;
                                                Global.MyPLC.WriteTag(MyTag);
                                            }
                                            catch (Exception ex)
                                            {
                                                Global.LogError(ex);
                                                response.Bad("PLC Timeout");
                                                return SaveResponse.Serialize(response);
                                            }
                                        }
                                    }
                                    using (SqlCommand cmd = new SqlCommand("UPDATE Bins SET BinLabel = '', ProdLabel = '' WHERE BinID = @BinID", con))
                                    {
                                        cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                                    }
                                } // End if Spare
                            } // End BinStatus

                            // General update statement
                            if (Edit.EditedCol != "Products" && Edit.EditedCol != "BinStatus" && Edit.EditedCol != "BinStamps")
                            {
                                Update = "UPDATE Bins SET " + Edit.EditedCol + "=@Value WHERE BinID=@BinID; UPDATE Bins SET BinPercent=(SELECT COALESCE(BinCount*100 / NULLIF(BinSize,0), 0) FROM Bins WHERE BinID=@BinID) WHERE BinID = @BinID";
                                using (SqlCommand cmd = new SqlCommand(Update, con))
                                {
                                    cmd.Parameters.AddWithValue("@Value", Edit.EditedVal);
                                    cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            if (Edit.EditedCol == "BinStamps")
                            {
                                using (SqlCommand cmd2 = new SqlCommand("SELECT BinStamps FROM Bins WHERE BinID=@BinID", con))
                                {
                                    cmd2.Parameters.AddWithValue("@BinID", Item.BinID);
                                    using SqlDataReader reader = cmd2.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        OldStamps = Global.GetValue<uint>(reader, "BinStamps");
                                    }
                                }

                                using SqlCommand cmd = new SqlCommand("UPDATE Bins SET BinStamps=@BinStamps WHERE BinID=@BinID", con);
                                cmd.Parameters.AddWithValue("@BinStamps", Item.BinStamps);
                                cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                                cmd.ExecuteNonQuery();
                            }
                            if (Item.BinStatus != 0) //Don't write anything to the PLC, the reset active code above will trigger the PLC to clear everything out
                                if (!Bin.DataRequestInsert(con, Item, map, StatusOriginal))
                                {
                                    response.Bad("PLC Timeout");
                                    return SaveResponse.Serialize(response);
                                }

                            if (Edit.EditedCol != "BinStatus" && Item.BinStatus != 0)
                            {
                                try
                                {
                                    UpdateBinProductsGUI(con, Item);
                                }
                                catch (Exception ex)
                                {
                                    Global.LogError(ex);
                                    response.Bad("Error saving");
                                    return SaveResponse.Serialize(response);
                                }
                            }

                            if (Edit.EditedCol == "BinStamps")
                            {
                                response.AddEdits(Stamp.GetChangesFromBitmap((uint)Item.BinStamps, OldStamps, Item.BinID));
                            }

                            Edit.Key = Item.BinID;
                        } // end foreach edit
                        response.AddEdits(Item.EditsList.Where(e => e.EditedCol != "BinStamps").ToList());
                    } // end foreach item
                }
            } // End connection

            response.Good("Save successful");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveIncDec(int IncDec)
        {
            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

            try
            {
                using SqlCommand cmd = new SqlCommand("UPDATE WEBSortSetup SET IncDec=@IncDec", con);
                cmd.Parameters.AddWithValue("@IncDec", IncDec);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                return ex.ToString();
            }
            return "Success";
        }

        private static void UpdateBinProductsGUI(SqlConnection con, Bin Item)
        {
            string ProductLabel = "";

            if (Item.ProdLen == null) { return; }

            ProductLengths SelectedProductLengths = new ProductLengths()
            {
                ProductsList = Item.ProdLen.ProductsList.Where(p => p.Selected).ToList(),
                LengthsList = Item.ProdLen.LengthsList.Where(l => l.Selected).ToList()
            };

            using (SqlCommand cmd = new SqlCommand($"DELETE FROM BinProducts WHERE BinID={Item.BinID}", con))
                cmd.ExecuteNonQuery();
            using (SqlCommand cmd = new SqlCommand($"DELETE FROM BinLengths WHERE BinID={Item.BinID}", con))
                cmd.ExecuteNonQuery();
            foreach (ProductLengths.Products prod in SelectedProductLengths.ProductsList)
            {
                using (SqlCommand cmd = new SqlCommand($"INSERT INTO BinProducts SELECT {Item.BinID},{prod.ID}", con))
                    cmd.ExecuteNonQuery();
            }
            foreach (ProductLengths.Lengths len in SelectedProductLengths.LengthsList)
            {
                using (SqlCommand cmd = new SqlCommand($"INSERT INTO BinLengths SELECT {Item.BinID},{len.ID}", con))
                    cmd.ExecuteNonQuery();
            }

            // Collect board counts for all boards in BinProductLenths for given BinID and store in dictionary with key prodid-lengthid
            Dictionary<string, int> BoardCounts = new Dictionary<string, int>();
            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM BinProductLengths WHERE BinID = {Item.BinID}", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string key = $"{Global.GetValue<int>(reader, "ProdID")}-{Global.GetValue<int>(reader, "LengthID")}";
                        if (!BoardCounts.ContainsKey(key))
                        {
                            BoardCounts.Add(key, Global.GetValue<int>(reader, "BoardCount"));
                        }
                    }
                }
            }

            using (SqlCommand cmd = new SqlCommand($"DELETE FROM [BinProductLengths] WHERE BinID={Item.BinID}", con))
                cmd.ExecuteNonQuery();

            foreach (ProductLengths.Products prod in SelectedProductLengths.ProductsList)
            {
                foreach (ProductLengths.Lengths len in SelectedProductLengths.LengthsList)
                {
                    int count = BoardCounts.ContainsKey($"{prod.ID}-{len.ID}") ? BoardCounts[$"{prod.ID}-{len.ID}"] : 0;

                    using (SqlCommand cmd = new SqlCommand($"INSERT INTO [BinProductLengths] SELECT {Item.BinID}, {prod.ID}, {len.ID}, {count}", con))
                        cmd.ExecuteNonQuery();

                    ProductLabel += $"({prod.Label} {len.Label})[{count}],";
                }
            }
            if (!string.IsNullOrEmpty(ProductLabel))
            {
                ProductLabel = ProductLabel.Remove(ProductLabel.Length - 1, 1); // Remove trailing comma
            }

            using (SqlCommand cmd = new SqlCommand($"UPDATE Bins SET ProductsLabel = @ProductsLabel WHERE BinID = {Item.BinID}", con))
            {
                cmd.Parameters.AddWithValue("@ProductsLabel", ProductLabel);

                cmd.ExecuteNonQuery();
            }
        }

        #endregion Table
    }
}