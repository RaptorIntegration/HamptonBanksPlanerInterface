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

                using (SqlCommand cmd = new SqlCommand("SELECT BinID, BinStatus, BinPercent FROM Bins", con))
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

            const string sql = "SELECT [BinID], [BinLabel], [BinStatus], [BinStatusLabel], [BinSize], [BinCount], BinPercent, " +
                               "[SortID], [ProductsLabel] FROM [Bins] with(NOLOCK)";

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Sorts = Bin.PopulateBinList(reader);
                    }
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

            int Sprays = 0, StampsPLC = 0, SpraysPLC = 0;
            int StatusOriginal = 0;

            bool succeeded = true;

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
                        int LengthMap = 0;
                        uint[] ProductMap = new uint[30];
                        uint[] ProductMapOld = new uint[30];

                        // Length and Product Map
                        if (Item.ProdLen != null)  // Products changed
                        {
                            // Only Selected Products and Lengths
                            GetSelectedProductMap(Item, ref LengthMap, ProductMap);

                            // Product Map Old
                            GetProductMapOld(con, Item, ProductMapOld);
                        }
                        else // No products changed, ProductMap = ProductMapOld
                        {
                            GetDBProductMap(con, Item, ref LengthMap, ProductMap, ProductMapOld);
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

                                string SqlCommandString = "EXECUTE UpdateBinData @FrameStart, @BayNum, @Name, @PkgSize, @Count, @RdmWidthFlag, @Status, @Stamps, @Sprays, @TrimFlag, @SortXRef";
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
                                    cmd.Parameters.AddWithValue("@RdmWidthFlag", 0);
                                    cmd.Parameters.AddWithValue("@Status", Item.BinStatus);
                                    cmd.Parameters.AddWithValue("@Stamps", 0);
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
                                            Tag MyTag = new Tag("Program:SorterTrays.Tray[" + Item.BinID.ToString() + "].ResetActive");
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
                            if (Edit.EditedCol != "Products" && Edit.EditedCol != "BinStatus")
                            {
                                Update = "UPDATE Bins SET " + Edit.EditedCol + "=@Value WHERE BinID=@BinID; UPDATE Bins SET BinPercent=(SELECT COALESCE(BinCount*100 / NULLIF(BinSize,0), 0) FROM Bins WHERE BinID=@BinID) WHERE BinID = @BinID";
                                using (SqlCommand cmd = new SqlCommand(Update, con))
                                {
                                    cmd.Parameters.AddWithValue("@Value", Edit.EditedVal);
                                    cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // BinStamps
                            using (SqlCommand cmd = new SqlCommand("select BinStamps from Bins where BinID=" + Item.BinID.ToString(), con))
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        StampsPLC = Convert.ToInt32(reader["BinStamps"].ToString());
                                    }
                                }
                            }

                            SpraysPLC |= Sprays;

                            using (SqlCommand cmd = new SqlCommand("UPDATE RaptorCommSettings SET DataRequests = DataRequests | 1", con))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            int ProductMapCount = GetDataRequestsBinColumns(con);

                            using (SqlCommand cmd = new SqlCommand(Bin.DataRequestBinSQL, con))
                            {
                                cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                                cmd.Parameters.AddWithValue("@BinLabel", Item.BinLabel);
                                cmd.Parameters.AddWithValue("@BinStatus", Item.BinStatus);
                                cmd.Parameters.AddWithValue("@BinSize", Item.BinSize);
                                cmd.Parameters.AddWithValue("@BinCount", Item.BinCount);
                                for (int i = 0; i < ProductMapCount; i++)
                                {
                                    cmd.Parameters.AddWithValue($"@ProductMap{i}", ProductMap[i].ToString());
                                    cmd.Parameters.AddWithValue($"@ProductMap{i}Old", ProductMap[i].ToString());
                                }
                                cmd.Parameters.AddWithValue("@LengthMap", LengthMap);
                                cmd.Parameters.AddWithValue("@BinStamps", 0);
                                cmd.Parameters.AddWithValue("@BinSprays", Sprays);
                                cmd.Parameters.AddWithValue("@SortID", Item.SortID);
                                cmd.Parameters.AddWithValue("@TrimFlag", 0);
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
                                        cmd.Parameters[$"@ProductMap{i}Old"].Value = ProductMapOld[i].ToString();
                                    }
                                }

                                if (Item.BinLabel.Length > 20)
                                {
                                    cmd.Parameters["@BinLabel"].Value = Item.BinLabel.Remove(20);
                                }

                                //don't send a message to the PLC for a virtual bay that is being set to spare or full
                                if (StatusOriginal != 5 || (StatusOriginal == 5 && Item.BinStatus != 0 && Item.BinStatus != 2 && Item.BinStatus != 1 && Item.BinStatus != 3 && Item.BinStatus != 4))
                                {
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Global.LogError(ex);
                                        response.Bad("Error saving");
                                        return SaveResponse.Serialize(response);
                                    }

                                    if (Global.OnlineSetup)
                                    {
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            reader.Read();
                                            // Make sure message is processed
                                            succeeded = Raptor.MessageAckConfirm("datarequestsbin", int.Parse(reader["id"].ToString()));
                                        }
                                        if (!succeeded)
                                        {
                                            response.Bad("PLC Timeout");
                                            return SaveResponse.Serialize(response);
                                        }
                                    }
                                }
                            } // End Command
                            using (SqlCommand cmd = new SqlCommand("UPDATE RaptorCommSettings SET datarequests = datarequests-1 WHERE (datarequests & 1)=1", con))
                            {
                                cmd.ExecuteNonQuery();
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

                            Edit.Key = Item.BinID;
                        } // end foreach edit
                        response.AddEdits(Item.EditsList);
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

        private static int GetDataRequestsBinColumns(SqlConnection con)
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
                Bin.DataRequestBinSQL = "INSERT INTO datarequestsbin SELECT getdate(),";
                foreach (string col in Columns.Where(c => c != "TimeStamp" && c != "ID"))
                {
                    Bin.DataRequestBinSQL += $"@{col},";
                }
                Bin.DataRequestBinSQL = Bin.DataRequestBinSQL.Remove(Bin.DataRequestBinSQL.Length - 1, 1);
                Bin.DataRequestBinSQL += ";SELECT id = (select max(id) FROM datarequestsbin with(NOLOCK))";

                return Columns.Count(c => c.Contains("ProductMap")) / 2;
            }
            else
            {
                return -1;
            }
        }

        private static void GetDBProductMap(SqlConnection con, Bin Item, ref int LengthMap, uint[] ProductMap, uint[] ProductMapOld)
        {
            // Product Map
            ProductLengths PL = new ProductLengths();
            using (SqlCommand cmd = new SqlCommand(ProductLengths.BinProdMapSql, con))
            {
                cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        PL.PopulateProductList(reader);

                        // Use only selected
                        foreach (ProductLengths.Products P in PL.ProductsList.Where(p => p.Selected).ToList())
                        {
                            ProductMap[P.ID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(P.ID.ToString()) - (32 * (P.ID / 32))));
                            ProductMapOld[P.ID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(P.ID.ToString()) - (32 * (P.ID / 32))));
                        }
                    }
                }
            }

            // Length Map
            using (SqlCommand cmd = new SqlCommand(ProductLengths.BinLengthMapSql, con))
            {
                cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        PL.PopulateLengthList(reader);

                        // Use only selected
                        foreach (ProductLengths.Lengths L in PL.LengthsList.Where(l => l.Selected).ToList())
                        {
                            LengthMap |= Convert.ToInt32(Math.Pow(2, Convert.ToDouble(L.ID)));
                        }
                    }
                }
            }
        }

        private static void GetSelectedProductMap(Bin Item, ref int LengthMap, uint[] ProductMap)
        {
            ProductLengths SelectedProductLengths = new ProductLengths()
            {
                ProductsList = Item.ProdLen.ProductsList.Where(p => p.Selected).ToList(),
                LengthsList = Item.ProdLen.LengthsList.Where(l => l.Selected).ToList()
            };

            // Product Map New
            foreach (ProductLengths.Products P in SelectedProductLengths.ProductsList)
            {
                ProductMap[P.ID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(P.ID.ToString()) - (32 * (P.ID / 32))));
            }

            // Length Map
            foreach (ProductLengths.Lengths L in SelectedProductLengths.LengthsList)
            {
                LengthMap |= Convert.ToInt32(Math.Pow(2, Convert.ToDouble(L.ID)));
            }
        }

        private static void GetProductMapOld(SqlConnection con, Bin Item, uint[] ProductMapOld)
        {
            ProductLengths PL = new ProductLengths();
            using (SqlCommand cmd = new SqlCommand(ProductLengths.BinProdMapSql, con))
            {
                cmd.Parameters.AddWithValue("@BinID", Item.SortID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        PL.PopulateProductList(reader);

                        // Use only selected
                        foreach (ProductLengths.Products P in PL.ProductsList.Where(p => p.Selected).ToList())
                        {
                            ProductMapOld[P.ID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(P.ID.ToString()) - (32 * (P.ID / 32))));
                        }
                    }
                }
            }
        }

        #endregion Table
    }
}