using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;

using WebSort.Model;

namespace WebSort
{
    public partial class Sorts : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            CurrentUser = Global.GetSecurity("Sorts", User.Identity.Name);

            Global.GetOnlineSetup();

            Label LabelScreenStatus = (Label)Master.FindControl("LabelScreenStatus");
            if (CurrentUser.Access == 0)
                LabelScreenStatus.Text = "READ ONLY";
            else if (CurrentUser.Access == 1)
                LabelScreenStatus.Text = "";
            else if (CurrentUser.Access == 2)
            {
                LabelScreenStatus.Text = "ACCESS DENIED";
                Response.Redirect("websort.aspx");
            }

            Session["recipemode"] = "0";
        }

        [WebMethod]
        public static string AddNewSort(int ID)
        {
            using (SqlConnection cn = new SqlConnection(Global.ConnectionString))
            {
                cn.Open();
                // Cascade rows down one SortID after for SortID's greater than/equal to selected value (Sorts, SortProducts, SortProductLengths, SortLengths).
                //Insert "blank" row, Delete any Sortid's over 75
                using (SqlCommand cmd = new SqlCommand("dbo.SortsInsertRow", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("RecipeID", Recipe.GetEditingRecipe().RecipeID);
                    cmd.Parameters.AddWithValue("index", ID);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return $"Successfully Added New Sort at {ID}";
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        return ex.ToString();
                    }
                }
            }
        }

        [WebMethod]
        public static string MoveSort(int from, int to)
        {
            using (SqlConnection cn = new SqlConnection(Global.ConnectionString))
            {
                cn.Open();

                // Cascade rows down one SortID after for SortID's greater than/equal to selected value (Sorts, SortProducts, SortProductLengths, SortLengths).
                // Change SortID from selected value to selected value. Revert cascade step
                using (SqlCommand cmd = new SqlCommand("dbo.SortsMoveRow", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("RecipeID", Recipe.GetEditingRecipe().RecipeID);
                    cmd.Parameters.AddWithValue("SortIDFrom", from);
                    cmd.Parameters.AddWithValue("SortIDTo", to);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return $"Successfully Moved Sort From {from} - To {to}";
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        return ex.ToString();
                    }
                }
            }
        }

        [WebMethod]
        public static string GetGradeMatrix()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();

            try
            {
                List<GradeMatrix> grades = GradeMatrix.GetData();
                return s.Serialize(grades);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                return ex.ToString();
            }
        }

        [WebMethod]
        public static string SaveGradeMatrix(GradeMatrix[] changes)
        {
            SaveResponse response = new SaveResponse("GradeMatrix");
            if (changes?.Length > 0)
            {
                int RecipeID = Recipe.GetEditingRecipe().RecipeID;
                using SqlConnection con = new SqlConnection(Global.ConnectionString);
                con.Open();

                foreach (GradeMatrix change in changes)
                {
                    try
                    {
                        if (!GradeMatrix.SaveData(change, con, RecipeID))
                        {
                            response.Bad("PLC Timeout");
                            return SaveResponse.Serialize(response);
                        }
                        response.AddEdits(change.EditsList);
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        response.Bad($"Error saving PLCGradeID: {change.PLCGradeID}");
                        return SaveResponse.Serialize(response);
                    }
                }
            }

            response.Good("Save Successful");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string GetRecipes()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            try
            {
                return s.Serialize(Recipe.GetAllData());
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                return ex.ToString();
            }
        }

        [WebMethod]
        public static string ChangeEditingRecipe(Recipe recipe)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            try
            {
                return s.Serialize(Recipe.ChangeEditing(recipe));
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                return ex.ToString();
            }
        }

        [WebMethod]
        public static string SaveNewRecipe(string NewName, Recipe recipe)
        {
            SaveResponse response = new SaveResponse();
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                try
                {
                    if (recipe?.RecipeID != 0 && recipe?.RecipeID != null)
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE Recipes SET RecipeLabel = @RecipeLabel WHERE RecipeID = @RecipeID", con))
                        {
                            cmd.Parameters.AddWithValue("@RecipeLabel", NewName);
                            cmd.Parameters.AddWithValue("@RecipeID", recipe.RecipeID);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand($"exec createRecipe '{NewName}'", con))
                            cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad($"Error saving recipe \"{recipe.RecipeLabel}\"");
                    return SaveResponse.Serialize(response);
                }
            }

            response.Good("Save successful");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string DeleteRecipe(Recipe recipe)
        {
            SaveResponse response = new SaveResponse("Recipes");

            if (recipe?.RecipeID != 0 && recipe?.RecipeID != null)
            {
                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                {
                    con.Open();

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand($"EXEC deleteRecipe {recipe.RecipeID}", con))
                            cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        response.Bad($"Error deleting recipe \"{recipe.RecipeLabel}\"");
                    }
                }
            }

            new Audit(recipe.RecipeID.ToString(), "RecipeID", "Products", true, false).InsertSimpleAudit();

            response.Good($"Deleted Recipe {recipe.RecipeLabel}");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string CopyRecipe(Recipe recipe)
        {
            SaveResponse response = new SaveResponse();
            if (recipe?.RecipeID == 0 || recipe?.RecipeID == null)
            {
                response.Bad("Invalid Recipe");
                return SaveResponse.Serialize(response);
            }

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                try
                {
                    using (SqlCommand command = new SqlCommand($"exec copyRecipe {recipe.RecipeID}", con))
                        command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad($"Error copying recipe \"{recipe.RecipeLabel}\"");
                    return SaveResponse.Serialize(response);
                }
            }

            response.Good($"Copied recipe \"{recipe.RecipeLabel}\" successfully");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string ActivateRecipe(Recipe recipe, bool full, bool reset)
        {
            int EditingRecipeID = Recipe.GetEditingRecipe().RecipeID;
            SaveResponse response = new SaveResponse("Recipes");

            //send sorts to PLC
            if (Global.OnlineSetup)
            {
                using SqlConnection con = new SqlConnection(Global.ConnectionString);
                con.Open();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("update websortsetup set activeproductlistzero=1", con))
                        cmd.ExecuteNonQuery();

                    if (full)
                    {
                        using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1", con))
                            cmd.ExecuteNonQuery();

                        using (SqlCommand cmd = new SqlCommand("select * from bins where binstatus=1", con))
                        using (SqlDataReader ReaderBins = cmd.ExecuteReader())
                        {
                            if (ReaderBins.HasRows)
                            {
                                while (ReaderBins.Read())
                                {
                                    Bin bin = new Bin(ReaderBins);

                                    Map map = new Map();
                                    try
                                    {
                                        Bin.GetProductLengthMap(con, map, bin.BinID);
                                    }
                                    catch (Exception ex)
                                    {
                                        Global.LogError(ex);
                                        response.Bad("PLC Timeout");
                                        return SaveResponse.Serialize(response);
                                        throw;
                                    }

                                    if (!Bin.DataRequestInsert(con, bin, map, bin.BinStatus, CommSettings: false, Ack: false))
                                    {
                                        response.Bad("PLC Timeout");
                                        return SaveResponse.Serialize(response);
                                    }

                                    using (SqlCommand cmdstatus = new SqlCommand($"update bins set binstatus = 2,bins.binstatuslabel=binstatus.binstatuslabel from binstatus where binstatus.binstatus=2 and binid={bin.BinID}", con))
                                        cmdstatus.ExecuteNonQuery();
                                }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1 where (datarequests & 1)=1", con))
                            cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2", con))
                        cmd.ExecuteNonQuery();

                    using (SqlCommand cmd = new SqlCommand("select * from sorts with(NOLOCK) where recipeid=" + recipe.RecipeID, con))
                    using (SqlDataReader readerSorts = cmd.ExecuteReader())
                    {
                        while (readerSorts.Read())
                        {
                            Map map = new Map();
                            Sort sort = new Sort(readerSorts);

                            try
                            {
                                Map.GetProductLengthMapDBSort(con, sort.SortID, map, recipe.RecipeID);
                            }
                            catch (Exception ex)
                            {
                                Global.LogError(ex);
                                response.Bad("PLC Timeout");
                                return SaveResponse.Serialize(response);
                                throw;
                            }

                            if (!Sort.DataRequestInsert(con, sort, map, false, false))
                            {
                                response.Bad("PLC Timeout");
                                return SaveResponse.Serialize(response);
                            }
                        }
                    }

                    //send grade map to PLC
                    using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 8", con))
                        cmd.ExecuteNonQuery();

                    using (SqlCommand cmd = new SqlCommand($"select * from gradematrix with(NOLOCK) where recipeid={recipe.RecipeID}", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            using (SqlCommand cmdInner = new SqlCommand(Grade.DataRequestsGradeSql, con))
                            {
                                cmdInner.Parameters.AddWithValue("@GradeID", Global.GetValue<int>(reader, "plcgradeID"));
                                cmdInner.Parameters.AddWithValue("@GradeIDX", Global.GetValue<int>(reader, "websortgradeID"));
                                cmdInner.Parameters.AddWithValue("@GradeStamps", 0);
                                cmdInner.Parameters.AddWithValue("@Write", 1);
                                cmdInner.Parameters.AddWithValue("@Processed", 0);

                                using (SqlDataReader readerInner = cmdInner.ExecuteReader())
                                {
                                    if (readerInner.HasRows)
                                    {
                                        while (readerInner.Read())
                                        {
                                            if (!Raptor.MessageAckConfirm("datarequestsgrade", Global.GetValue<int>(readerInner, "id")))
                                            {
                                                response.Bad("PLC Timeout");
                                                return SaveResponse.Serialize(response);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad($"Failed to activate recipe \"{recipe.RecipeLabel}\"");
                    return SaveResponse.Serialize(response);
                }

                try
                {
                    Recipe.ChangeActive(recipe);

                    using (SqlCommand cmd = new SqlCommand($"updatechangeactiverecipe {EditingRecipeID}, '{reset}'", con))
                        cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad($"Failed to activate recipe \"{recipe.RecipeLabel}\"");
                    return SaveResponse.Serialize(response);
                }

                response.Good($"Activated recipe \"{recipe.RecipeLabel}\"");
                return SaveResponse.Serialize(response);
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                {
                    con.Open();

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand($"updatechangeactiverecipe {recipe.RecipeID}, '{reset}'", con))
                            cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        response.Bad($"Failed to activate recipe \"{recipe.RecipeLabel}\"");
                        return SaveResponse.Serialize(response);
                    }
                }

                try
                {
                    Recipe.ChangeActive(recipe);
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad($"Failed to activate recipe \"{recipe.RecipeLabel}\"");
                    return SaveResponse.Serialize(response);
                }

                List<Edit> edits = new List<Edit>
                {
                    new Edit { EditedCol = "Active", EditedVal = "True", Key = recipe.RecipeID, Previous = "False" }
                };

                response.AddEdits(edits);
                response.Good($"Activated recipe \"{recipe.RecipeLabel}\"");
                return SaveResponse.Serialize(response);
            }
        }

        [WebMethod]
        public static string GetGrades()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Grade.GetAllData());
        }

        [WebMethod]
        public static string GetSecurity()
        {
            return CurrentUser.Access.ToString();
        }

        #region Table

        [WebMethod]
        public static string GetData()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Sort> Sorts = new List<Sort>();

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();
                using SqlCommand cmd = new SqlCommand("SELECT * FROM Sorts WHERE RecipeID = (SELECT RecipeID FROM Recipes WHERE Editing=1)", con);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Sorts = Sort.PopulateSortList(reader);
                }
            }
            return serializer.Serialize(Sorts);
        }

        [WebMethod]
        public static string GetProductList(string SortID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductLengths PL = new ProductLengths();

            string RecipeID = string.Empty;

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT RecipeID FROM Recipes WHERE Editing = 1", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            RecipeID = reader["RecipeID"] != null ? reader["RecipeID"].ToString() : "0";
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(ProductLengths.SortProdMapSql, con))
                {
                    cmd.Parameters.AddWithValue("@RecipeID", RecipeID);
                    cmd.Parameters.AddWithValue("@SortID", SortID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            PL.PopulateProductList(reader);
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(ProductLengths.SortLengthMapSql, con))
                {
                    cmd.Parameters.AddWithValue("@RecipeID", RecipeID);
                    cmd.Parameters.AddWithValue("@SortID", SortID);

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
        public static string Save(Sort[] Changed, bool ActiveRecipe)
        {
            SaveResponse response = new SaveResponse("Sorts");

            if (CurrentUser.Access != 1)
            {
                response.Bad("Unauthorized!");
                return SaveResponse.Serialize(response);
            }

            uint OldStamps = 0, Sprays = 0, SpraysPLC = 0;

            string RecipeID = "0", ProductString = "", LengthString = "";
            string Update;

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                // RecipeID
                using (SqlCommand cmd = new SqlCommand("SELECT RecipeID FROM Recipes WHERE Editing=1", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            RecipeID = reader["RecipeID"].ToString();
                        }
                    }
                }

                if (Changed.Length > 0)
                {
                    foreach (Sort Item in Changed)
                    {
                        uint LengthMap = 0;
                        uint[] ProductMap = new uint[30];
                        uint[] ProductMapOld = new uint[30];

                        // Length and Product Map
                        if (Item.ProdLen != null)  // Products changed
                        {
                            // Only Selected Products and Lengths
                            GetSelectedProductMap(ref ProductString, ref LengthString, Item, ref LengthMap, ProductMap);

                            // Product Map Old
                            GetProductMapOld(con, Item, ProductMapOld, RecipeID);
                        }
                        else // No products changed, ProductMap = ProductMapOld
                        {
                            GetDBProductMap(ref ProductString, ref LengthString, con, Item, ref LengthMap, ProductMap, ProductMapOld, RecipeID);
                        }

                        Item.SortStamps = Stamp.GetStampsBitMap(Item.SelectedStamps);

                        foreach (Edit Edit in Item.EditsList)
                        {
                            // Invalid package size
                            if (Edit.EditedCol == "Active" && Item.SortSize <= 0 && Item.Active)
                            {
                                response.Bad($"Invalid package size entered for Sort: {Item.SortID}");
                                return SaveResponse.Serialize(response);
                            }

                            // General update statement
                            if (Edit.EditedCol != "Products" && Edit.EditedCol != "SortStamps")
                            {
                                Update = "UPDATE Sorts SET " + Edit.EditedCol + "=@Value WHERE RecipeID=" + RecipeID + " AND SortID=@ID";
                                using SqlCommand cmd = new SqlCommand(Update, con);
                                cmd.Parameters.AddWithValue("@Value", Edit.EditedVal);
                                cmd.Parameters.AddWithValue("@ID", Item.SortID);
                                cmd.ExecuteNonQuery();
                            }
                            if (Edit.EditedCol == "SortStamps")
                            {
                                using (SqlCommand cmd2 = new SqlCommand("SELECT SortStamps FROM Sorts WHERE SortID=@SortID AND RecipeID=@RecipeID", con))
                                {
                                    cmd2.Parameters.AddWithValue("@SortID", Item.SortID);
                                    cmd2.Parameters.AddWithValue("@RecipeID", RecipeID);
                                    using SqlDataReader reader = cmd2.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        OldStamps = Global.GetValue<uint>(reader, "SortStamps");
                                    }
                                }

                                using SqlCommand cmd = new SqlCommand("UPDATE Sorts SET SortStamps=@SortStamps WHERE RecipeID=@RecipeID AND SortID=@ID", con);
                                cmd.Parameters.AddWithValue("@SortStamps", Item.SortStamps);
                                cmd.Parameters.AddWithValue("@RecipeID", RecipeID);
                                cmd.Parameters.AddWithValue("@ID", Item.SortID);
                                cmd.ExecuteNonQuery();
                            }

                            // Order Count -> 0
                            if (Edit.EditedCol == "OrderCount" && Edit.EditedVal == "0")
                            {
                                using (SqlCommand cmd = new SqlCommand("DELETE FROM OrderManagementAnticipation where sortid=@ID", con))
                                {
                                    cmd.Parameters.AddWithValue("@ID", Item.SortID);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            SpraysPLC |= Sprays;

                            if (Global.OnlineSetup && ActiveRecipe)
                            {
                                int ProductMapCount = GetDataRequestsSortColumns(con);
                                if (Item.Active)
                                {
                                    using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2", con))
                                        cmd.ExecuteNonQuery();

                                    using (SqlCommand cmd = new SqlCommand(Sort.DataRequestsSortSQL, con))
                                    {
                                        cmd.Parameters.AddWithValue("@SortID", Item.SortID);
                                        cmd.Parameters.AddWithValue("@SortLabel", Item.SortLabel);
                                        cmd.Parameters.AddWithValue("@SortSize", Item.SortSize);
                                        cmd.Parameters.AddWithValue("@PkgsPerSort", Item.PkgsPerSort);
                                        cmd.Parameters.AddWithValue("@OrderCount", Item.OrderCount);
                                        cmd.Parameters.AddWithValue("@SecProdID", Item.SecProdID);
                                        cmd.Parameters.AddWithValue("@SecSize", Item.SecSize);
                                        cmd.Parameters.AddWithValue("@LengthMap", Convert.ToInt32(LengthMap));
                                        cmd.Parameters.AddWithValue("@ProductMap0c", 0);
                                        cmd.Parameters.AddWithValue("@ProductMap1c", 0);
                                        cmd.Parameters.AddWithValue("@ProductMap2c", 0);
                                        cmd.Parameters.AddWithValue("@LengthMapc", 0);
                                        cmd.Parameters.AddWithValue("@SortStamps", Item.SortStamps);
                                        cmd.Parameters.AddWithValue("@SortSprays", (long)SpraysPLC);
                                        cmd.Parameters.AddWithValue("@Zone1", (Item.Zone1Stop * 256) + Item.Zone1Start);
                                        cmd.Parameters.AddWithValue("@Zone2", (Item.Zone2Stop * 256) + Item.Zone2Start);
                                        cmd.Parameters.AddWithValue("@TrimFlag", 1);
                                        cmd.Parameters.AddWithValue("@RW", Item.RW);
                                        cmd.Parameters.AddWithValue("@Active", Item.Active);
                                        cmd.Parameters.AddWithValue("@ProductsOnly", 2);
                                        cmd.Parameters.AddWithValue("@Write", 1);
                                        cmd.Parameters.AddWithValue("@Processed", 0);

                                        for (int p = 0; p < ProductMapCount; p++)
                                        {
                                            cmd.Parameters.AddWithValue("@ProductMap" + p.ToString(), Convert.ToInt32(ProductMap[p]));
                                            cmd.Parameters.AddWithValue("@ProductMap" + p.ToString() + "Old", Convert.ToInt32(ProductMapOld[p]));
                                        }

                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                try
                                                {
                                                    if (!Raptor.MessageAckConfirm("datarequestssort", int.Parse(reader["id"].ToString())))
                                                    {
                                                        response.Bad("PLC Timeout");
                                                        return SaveResponse.Serialize(response);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Global.LogError(ex);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //first send products only with zeroes for products and lengths
                                    using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2", con))
                                        cmd.ExecuteNonQuery();

                                    using (SqlCommand cmd = new SqlCommand(Sort.DataRequestsSortSQL, con))
                                    {
                                        cmd.Parameters.AddWithValue("@SortID", Item.SortID);
                                        cmd.Parameters.AddWithValue("@SortLabel", Item.SortLabel);
                                        cmd.Parameters.AddWithValue("@SortSize", Item.SortSize);
                                        cmd.Parameters.AddWithValue("@PkgsPerSort", Item.PkgsPerSort);
                                        cmd.Parameters.AddWithValue("@OrderCount", Item.OrderCount);
                                        cmd.Parameters.AddWithValue("@SecProdID", Item.SecProdID);
                                        cmd.Parameters.AddWithValue("@SecSize", Item.SecSize);
                                        cmd.Parameters.AddWithValue("@LengthMap", 0);
                                        cmd.Parameters.AddWithValue("@ProductMap0c", 0);
                                        cmd.Parameters.AddWithValue("@ProductMap1c", 0);
                                        cmd.Parameters.AddWithValue("@ProductMap2c", 0);
                                        cmd.Parameters.AddWithValue("@LengthMapc", 0);
                                        cmd.Parameters.AddWithValue("@SortStamps", Item.SortStamps);
                                        cmd.Parameters.AddWithValue("@SortSprays", (long)SpraysPLC);
                                        cmd.Parameters.AddWithValue("@Zone1", (Item.Zone1Stop * 256) + Item.Zone1Start);
                                        cmd.Parameters.AddWithValue("@Zone2", (Item.Zone2Stop * 256) + Item.Zone2Start);
                                        cmd.Parameters.AddWithValue("@TrimFlag", 1);
                                        cmd.Parameters.AddWithValue("@RW", Item.RW);
                                        cmd.Parameters.AddWithValue("@Active", Item.Active);
                                        cmd.Parameters.AddWithValue("@ProductsOnly", 1);
                                        cmd.Parameters.AddWithValue("@Write", 1);
                                        cmd.Parameters.AddWithValue("@Processed", 0);

                                        for (int p = 0; p < ProductMapCount; p++)
                                        {
                                            cmd.Parameters.AddWithValue("@ProductMap" + p.ToString(), 0);
                                            cmd.Parameters.AddWithValue("@ProductMap" + p.ToString() + "Old", Convert.ToInt32(ProductMapOld[p]));
                                        }

                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                try
                                                {
                                                    if (Raptor.MessageAckConfirm("datarequestssort", int.Parse(reader["id"].ToString())))
                                                    {
                                                        response.Bad("PLC Timeout");
                                                        return SaveResponse.Serialize(response);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Global.LogError(ex);
                                                }
                                            }
                                        }

                                        //then send to the sort udt only
                                        for (int j = 0; j < ProductMapCount; j++)
                                        {
                                            cmd.Parameters["@ProductMap" + j.ToString()].Value = ProductMap[j];
                                        }
                                        cmd.Parameters["@ProductsOnly"].Value = 0;

                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                try
                                                {
                                                    if (Raptor.MessageAckConfirm("datarequestssort", int.Parse(reader["id"].ToString())))
                                                    {
                                                        response.Bad("PLC Timeout");
                                                        return SaveResponse.Serialize(response);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Global.LogError(ex);
                                                }
                                            }
                                        }
                                    }
                                }
                            } // end if OnlineSetup and ActiveRecipe
                            using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2 where (datarequests & 2)=2", con))
                                cmd.ExecuteNonQuery();

                            UpdateSortProductsGUI(con, Item, RecipeID);

                            Edit.Key = Item.SortID;

                            if (Edit.EditedCol == "SortStamps")
                            {
                                response.AddEdits(Stamp.GetChangesFromBitmap((uint)Item.SortStamps, OldStamps, Item.SortID));
                            }
                        } // end foreach edit
                        ProductString = "";
                        LengthString = "";
                        response.AddEdits(Item.EditsList.Where(e => e.EditedCol != "SortStamps").ToList());
                    } // end foreach item
                }
            } // End connection
            response.Good("Save successful");
            return SaveResponse.Serialize(response);
        }

        private static void UpdateSortProductsGUI(SqlConnection con, Sort Item, string RecipeID)
        {
            string ProductLabel = "";

            if (Item.ProdLen == null) { return; }

            ProductLengths SelectedProductLengths = new ProductLengths()
            {
                ProductsList = Item.ProdLen.ProductsList.Where(p => p.Selected).ToList(),
                LengthsList = Item.ProdLen.LengthsList.Where(l => l.Selected).ToList()
            };

            using (SqlCommand cmd = new SqlCommand($"DELETE FROM SortProducts WHERE SortID={Item.SortID} AND RecipeID={RecipeID}", con))
                cmd.ExecuteNonQuery();
            using (SqlCommand cmd = new SqlCommand($"DELETE FROM SortLengths WHERE SortID={Item.SortID} AND RecipeID={RecipeID}", con))
                cmd.ExecuteNonQuery();
            foreach (ProductLengths.Products prod in SelectedProductLengths.ProductsList)
            {
                using (SqlCommand cmd = new SqlCommand($"INSERT INTO SortProducts SELECT {RecipeID},{Item.SortID},{prod.ID}", con))
                    cmd.ExecuteNonQuery();
            }
            foreach (ProductLengths.Lengths len in SelectedProductLengths.LengthsList)
            {
                using (SqlCommand cmd = new SqlCommand($"INSERT INTO SortLengths SELECT {RecipeID},{Item.SortID},{len.ID}", con))
                    cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = new SqlCommand($"DELETE FROM [SortProductLengths] WHERE SortID={Item.SortID} AND RecipeID = {RecipeID}", con))
                cmd.ExecuteNonQuery();

            foreach (ProductLengths.Products prod in SelectedProductLengths.ProductsList)
            {
                foreach (ProductLengths.Lengths len in SelectedProductLengths.LengthsList)
                {
                    using (SqlCommand cmd = new SqlCommand($"INSERT INTO [SortProductLengths] SELECT {RecipeID}, {Item.SortID}, {prod.ID}, {len.ID}", con))
                        cmd.ExecuteNonQuery();

                    ProductLabel += $"({prod.Label} {len.Label}),";
                }
            }

            if (!string.IsNullOrEmpty(ProductLabel))
            {
                ProductLabel = ProductLabel.Remove(ProductLabel.Length - 1, 1); // Remove trailing comma
            }

            using (SqlCommand cmd = new SqlCommand($"UPDATE Sorts SET ProductsLabel = @ProductsLabel WHERE SortID = {Item.SortID} AND RecipeID = {RecipeID}", con))
            {
                cmd.Parameters.AddWithValue("@ProductsLabel", ProductLabel);

                cmd.ExecuteNonQuery();
            }
        }

        private static int GetDataRequestsSortColumns(SqlConnection con)
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
                Sort.DataRequestsSortSQL = "INSERT INTO DataRequestsSort SELECT getdate(),";
                foreach (string col in Columns.Where(c => c != "TimeStamp" && c != "ID"))
                {
                    Sort.DataRequestsSortSQL += $"@{col},";
                }
                Sort.DataRequestsSortSQL = Sort.DataRequestsSortSQL.Remove(Sort.DataRequestsSortSQL.Length - 1, 1);
                Sort.DataRequestsSortSQL += ";SELECT id = (select max(id) FROM datarequestsSort with(NOLOCK))";

                return Columns.Count(c => c.Contains("ProductMap") && !c.EndsWith("c")) / 2;
            }
            else
            {
                return -1;
            }
        }

        private static void GetDBProductMap(ref string ProductString, ref string LengthString, SqlConnection con, Sort Item, ref uint LengthMap, uint[] ProductMap, uint[] ProductMapOld, string RecipeID)
        {
            // Product Map
            ProductLengths PL = new ProductLengths();
            using (SqlCommand cmd = new SqlCommand(ProductLengths.SortProdMapSql, con))
            {
                cmd.Parameters.AddWithValue("@RecipeID", RecipeID);
                cmd.Parameters.AddWithValue("@SortID", Item.SortID);
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
                            ProductString += P.Label + "|";
                        }
                    }
                }
            }

            // Length Map
            using (SqlCommand cmd = new SqlCommand(ProductLengths.SortLengthMapSql, con))
            {
                cmd.Parameters.AddWithValue("@RecipeID", RecipeID);
                cmd.Parameters.AddWithValue("@SortID", Item.SortID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        PL.PopulateLengthList(reader);

                        // Use only selected
                        foreach (ProductLengths.Lengths L in PL.LengthsList.Where(l => l.Selected).ToList())
                        {
                            LengthMap |= Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(L.ID)));
                            LengthString += L.Label + "|";
                        }
                    }
                }
            }
        }

        private static void GetSelectedProductMap(ref string ProductString, ref string LengthString, Sort Item, ref uint LengthMap, uint[] ProductMap)
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
                ProductString += P.Label + "|";
            }

            // Length Map
            foreach (ProductLengths.Lengths L in SelectedProductLengths.LengthsList)
            {
                LengthMap |= Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(L.ID)));
                LengthString += L.Label + "|";
            }
        }

        private static void GetProductMapOld(SqlConnection con, Sort Item, uint[] ProductMapOld, string RecipeID)
        {
            ProductLengths PL = new ProductLengths();
            using (SqlCommand cmd = new SqlCommand(ProductLengths.SortProdMapSql, con))
            {
                cmd.Parameters.AddWithValue("@RecipeID", RecipeID);
                cmd.Parameters.AddWithValue("@SortID", Item.SortID);
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