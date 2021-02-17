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
        public static string GetStamps()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            List<object> ret = new List<object>();

            try
            {
                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                {
                    con.Open();

                    using SqlCommand cmd = new SqlCommand("SELECT * FROM Stamps", con);
                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ret.Add(new {
                                ID = Global.GetValue<int>(reader, "StampID"),
                                Label = Global.GetValue<string>(reader, "StampDescription")
                            });
                        }
                    }
                }
                return s.Serialize(ret);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                return ex.ToString();
            }
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
                                    Bin bin = new Bin(ReaderBins)
                                    {
                                        BinStatus = 2,
                                        BinStampsLabel = "Full"
                                    };

                                    Map map = new Map();
                                    try
                                    {
                                        Map.GetProductLengthMapDBBin(con, map, bin);
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
                                Map.GetProductLengthMapDBSort(con, map, sort, recipe.RecipeID);
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
                            using SqlCommand cmdInner = new SqlCommand(Grade.DataRequestsGradeSql, con);
                            cmdInner.Parameters.AddWithValue("@GradeID", Global.GetValue<int>(reader, "plcgradeID"));
                            cmdInner.Parameters.AddWithValue("@GradeIDX", Global.GetValue<int>(reader, "websortgradeID"));
                            cmdInner.Parameters.AddWithValue("@GradeStamps", Global.GetValue<int>(reader, "GradeStamps"));
                            cmdInner.Parameters.AddWithValue("@Write", 1);
                            cmdInner.Parameters.AddWithValue("@Processed", 0);

                            using SqlDataReader readerInner = cmdInner.ExecuteReader();
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

                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        PL.PopulateProductList(reader);
                    }
                }

                using (SqlCommand cmd = new SqlCommand(ProductLengths.SortLengthMapSql, con))
                {
                    cmd.Parameters.AddWithValue("@RecipeID", RecipeID);
                    cmd.Parameters.AddWithValue("@SortID", SortID);

                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        PL.PopulateLengthList(reader);
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

            string Update;

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                Recipe EditingRecipe = Recipe.GetEditingRecipe();

                if (Changed.Length > 0)
                {
                    foreach (Sort Item in Changed)
                    {
                        Map map = new Map();

                        // Length and Product Map
                        if (Item.ProdLen != null)  // Products changed
                        {
                            // Only Selected Products and Lengths
                            Map.GetSelectedProductMapSort(Item, map);

                            // Product Map Old
                            Map.GetProductMapOldSort(con, Item, map, EditingRecipe.RecipeID);
                        }
                        else // No products changed, ProductMap = ProductMapOld
                        {
                            Map.GetDBProductMapSort(con, Item, map, EditingRecipe.RecipeID);
                        }

                        foreach (Edit Edit in Item.EditsList)
                        {
                            // Invalid package size
                            if (Edit.EditedCol == "Active" && (Item.SortSize <= 0 || Item.SortSize > 255) && Item.Active)
                            {
                                response.Bad($"Invalid package size entered for Sort: {Item.SortID} (cannot be zero and must be 255 or less)");
                                return SaveResponse.Serialize(response);
                            }
                            // Invalid orders size
                            if (Edit.EditedCol == "OrderCount" && Item.OrderCount > 255)
                            {
                                response.Bad($"Invalid order size entered for Sort: {Item.SortID} (must be 255 or less)");
                                return SaveResponse.Serialize(response);
                            }
                            // Invalid zone sizes
                            if (Edit.EditedCol == "Zone1Start" && Item.Zone1Start > 255)
                            {
                                response.Bad($"Invalid zone 1 start entered for Sort: {Item.SortID} (must be 255 or less)");
                                return SaveResponse.Serialize(response);
                            }
                            if (Edit.EditedCol == "Zone1Stop" && Item.Zone1Start > 255)
                            {
                                response.Bad($"Invalid zone 1 stop entered for Sort: {Item.SortID} (must be 255 or less)");
                                return SaveResponse.Serialize(response);
                            }
                            if (Edit.EditedCol == "Zone2Start" && Item.Zone1Start > 255)
                            {
                                response.Bad($"Invalid zone 2 start entered for Sort: {Item.SortID} (must be 255 or less)");
                                return SaveResponse.Serialize(response);
                            }
                            if (Edit.EditedCol == "Zone2Stop" && Item.Zone1Start > 255)
                            {
                                response.Bad($"Invalid zone 2 stop entered for Sort: {Item.SortID} (must be 255 or less)");
                                return SaveResponse.Serialize(response);
                            }

                            // General update statement
                            if (Edit.EditedCol != "Products" && Edit.EditedCol != "SortSprays")
                            {
                                Update = "UPDATE Sorts SET " + Edit.EditedCol + "=@Value WHERE RecipeID=" + EditingRecipe.RecipeID + " AND SortID=@ID";
                                using SqlCommand cmd = new SqlCommand(Update, con);
                                cmd.Parameters.AddWithValue("@Value", Edit.EditedVal);
                                cmd.Parameters.AddWithValue("@ID", Item.SortID);
                                cmd.ExecuteNonQuery();
                            }
                            if (Edit.EditedCol == "SortSprays")
                            {
                                Update = "UPDATE Sorts SET SortSprays=@Value WHERE RecipeID=" + EditingRecipe.RecipeID + " AND SortID=@ID";
                                using SqlCommand cmd = new SqlCommand(Update, con);
                                cmd.Parameters.AddWithValue("@Value", Convert.ToInt32(Item.SortSprays));
                                cmd.Parameters.AddWithValue("@ID", Item.SortID);
                                cmd.ExecuteNonQuery();

                                Edit.EditedCol = "Premium Stamp";
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

                            if (Global.OnlineSetup && ActiveRecipe)
                            {
                                if (Item.Active)
                                {
                                    try
                                    {
                                        if (!Sort.DataRequestInsert(con, Item, map))
                                        {
                                            response.Bad("PLC Timeout");
                                            return SaveResponse.Serialize(response);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Global.LogError(ex);
                                        response.Bad("Error Saving");
                                        return SaveResponse.Serialize(response);
                                    }
                                }
                                else
                                {
                                    // First zero out product/length map
                                    try
                                    {
                                        if (!Sort.DataRequestInsert(con, Item, map, ZeroOut: true))
                                        {
                                            response.Bad("PLC Timeout");
                                            return SaveResponse.Serialize(response);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Global.LogError(ex);
                                        response.Bad("Error Saving");
                                        return SaveResponse.Serialize(response);
                                    }

                                    // Then send new product/length map
                                    try
                                    {
                                        if (!Sort.DataRequestInsert(con, Item, map, ProductsOnlyZero: true))
                                        {
                                            response.Bad("PLC Timeout");
                                            return SaveResponse.Serialize(response);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Global.LogError(ex);
                                        response.Bad("Error Saving");
                                        return SaveResponse.Serialize(response);
                                    }
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2 where (datarequests & 2)=2", con))
                                cmd.ExecuteNonQuery();

                            //send Cut In Two Overrides to PLC
                            using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2097152", con))
                                cmd.ExecuteNonQuery();

                            if (Edit.EditedCol == "BinID")
                            {
                                Edit.EditedCol = "CN2 Override";
                            }

                            UpdateSortProductsGUI(con, Item, EditingRecipe.RecipeID);

                            Edit.Key = Item.SortID;
                        } // end foreach edit

                        response.AddEdits(Item.EditsList.Where(e => e.EditedCol != "SortStamps").ToList());
                    } // end foreach item
                }
            } // End connection
            response.Good("Save successful");
            return SaveResponse.Serialize(response);
        }

        private static void UpdateSortProductsGUI(SqlConnection con, Sort Item, int RecipeID)
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

        #endregion Table
    }
}