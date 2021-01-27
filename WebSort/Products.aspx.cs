using Logix;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;

using WebSort.Model;

namespace WebSort
{
    public partial class WebForm2 : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) { return; }
            Global.GetOnlineSetup();
            CurrentUser = Global.GetSecurity("Product Setup", User.Identity.Name);

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
        }

        protected static void SendAttributeCountsToPLC()
        {
            string nt = "", nw = "", nl = "", np = "", nm = "", ns = "", npet = "";
            Global.MyPLC = new Controller();

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("update websortsetup set numthicks=(select max(id) from thickness where id>0) select nt=max(id),plcipaddress,plcprocessorslot,plctimeout from RaptorCommSettings,thickness where id>0 group by plcipaddress,plcprocessorslot,plctimeout", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            nt = Global.GetValue<string>(reader, "nt");

                            Global.MyPLC.IPAddress = Global.GetValue<string>(reader, "PLCIPAddress");
                            Global.MyPLC.Path = Global.GetValue<string>(reader, "PLCProcessorSlot");
                            Global.MyPLC.Timeout = Global.GetValue<int>(reader, "PLCTimeout");
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update websortsetup set numwidths=(select max(id) from width where id>0) select nw=max(id) from width where id>0", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            nw = Global.GetValue<string>(reader, "nw");
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update websortsetup set numlengths=(select max(lengthid) from lengths where lengthid>0) select nl=max(lengthid) from lengths where lengthid>0", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            nl = Global.GetValue<string>(reader, "nl");
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update websortsetup set numproducts=(select max(prodid) from products where prodid>0) select np=max(prodid) from products where prodid>0", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            np = Global.GetValue<string>(reader, "np");
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update websortsetup set nummoistures=(select max(moistureid) from moistures where moistureid>0) select nm=max(moistureid) from moistures where moistureid>0", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            nm = Global.GetValue<string>(reader, "nm");
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update websortsetup set numspecs=(select count(specid) from specs where specid>0) select ns=count(specid) from specs where specid>0", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ns = Global.GetValue<string>(reader, "ns");
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update websortsetup set numpetlengths=(select count(petlengthid) from petlengths where petlengthid>0) select npet=count(petlengthid) from petlengths where petlengthid>0", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            npet = Global.GetValue<string>(reader, "npet");
                        }
                    }
                }
            }

            if (Global.MyPLC.Connect() != ResultCode.E_SUCCESS)
            {
                Global.LogError(new Exception("Error connecting PLC"));
                return;
            }
            Tag MyTag = new Tag("_numthicks")
            {
                DataType = Tag.ATOMIC.DINT
            };
            Tag MyTag1 = new Tag("_numwidths")
            {
                DataType = Tag.ATOMIC.DINT
            };
            Tag MyTag2 = new Tag("_numlengths")
            {
                DataType = Tag.ATOMIC.DINT
            };
            Tag MyTag3 = new Tag("_numproducts")
            {
                DataType = Tag.ATOMIC.DINT
            };
            Tag MyTag4 = new Tag("_nummoists")
            {
                DataType = Tag.ATOMIC.DINT
            };
            //Tag MyTag5 = new Tag("_numproducts");
            // MyTag5.DataType = Logix.Tag.ATOMIC.DINT;
            Tag MyTag6 = new Tag("_numpet")
            {
                DataType = Tag.ATOMIC.DINT
            };

            try
            {
                MyTag.Value = nt;
                Global.MyPLC.WriteTag(MyTag);
                MyTag1.Value = nw;
                Global.MyPLC.WriteTag(MyTag1);
                MyTag2.Value = nl;
                Global.MyPLC.WriteTag(MyTag2);
                MyTag3.Value = np;
                Global.MyPLC.WriteTag(MyTag3);
                //MyTag4.Value = nm;
                //Global.MyPLC.WriteTag(MyTag4);
                //MyTag5.Value = ns;
                //Global.MyPLC.WriteTag(MyTag5);
                MyTag6.Value = npet;
                Global.MyPLC.WriteTag(MyTag6);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
            }
        }

        [WebMethod]
        public static string GetProducts()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Product
                .GetAll()
                .OrderBy(p => p.ThickNominal)
                .ThenBy(p => p.WidthNominal)
            );
        }

        [WebMethod]
        public static string GetGrades()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Grade.GetAllData());
        }

        [WebMethod]
        public static string GetLengths()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Length.GetAll());
        }

        [WebMethod]
        public static string GetPETLengths()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(PETLength.GetAll());
        }

        [WebMethod]
        public static string GetGraders()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Graders.GetAll());
        }

        [WebMethod]
        public static string GetSecurity()
        {
            return CurrentUser.Access.ToString();
        }

        [WebMethod]
        public static string GetThicknesses()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Thickness.GetAll());
        }

        [WebMethod]
        public static string GetWidths()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Width.GetAll());
        }

        [WebMethod]
        public static string GetNearSaw()
        {
            object obj = null;
            JavaScriptSerializer s = new JavaScriptSerializer();
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT NearSawOffSet FROM WebSortSetup", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        obj = new { NearSaw = Global.GetValue<int>(reader, "NearSawOffset") };
                    }
                }
            }
            return s.Serialize(obj);
        }

        [WebMethod]
        public static string DeleteProduct(Product product)
        {
            SaveResponse response = new SaveResponse("Products");
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                //erase product from PLC when deleting
                if (Global.OnlineSetup)
                {
                    try
                    {
                        Product.DataRequestInsert(con, product, ZeroOut: true);
                        SendAttributeCountsToPLC();
                        Product.Delete(product);
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        response.Bad("Error deleting product");
                        return SaveResponse.Serialize(response);
                    }
                }
                else
                {
                    try
                    {
                        Product.Delete(product);
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        response.Bad("Error deleting Product");
                        return SaveResponse.Serialize(response);
                    }
                }
            }
            new Audit(product.ProdID.ToString(), "ProdID", "Products", true, false).InsertSimpleAudit();
            response.Good($"Deleted #{product.ProdID} - {product.ProdLabel}");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string AddNewProduct(Product product)
        {
            SaveResponse response = new SaveResponse();

            if (product.GradeID > 0 && Product.Exists(product)) //GradeID = -1 -> All Grades
            {
                response.Bad("Product with the same attributes already exists");
                return SaveResponse.Serialize(response);
            }

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                try
                {
                    product.AddThicksWidths(Thickness.GetAtID(product.ThicknessID), Width.GetAtID(product.WidthID));
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error adding product");
                    return SaveResponse.Serialize(response);
                }

                if (product.GradeID > 0) //single product
                {
                    product.ProdID = Product.Insert(product);

                    if (Global.OnlineSetup)
                    {
                        try
                        {
                            if (!Product.DataRequestInsert(con, product, false))
                            {
                                response.Bad("PLC Timeout");
                                return SaveResponse.Serialize(response);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.LogError(ex);
                            response.Bad("Error saving product");
                            return SaveResponse.Serialize(response);
                        }

                        if (!SendToPLC(response, con, product))
                            return SaveResponse.Serialize(response);
                    }
                }
                else  //all grades
                {
                    foreach (Grade grade in Grade.GetAllData())
                    {
                        product.GradeID = grade.GradeID;

                        if (!Product.Exists(product))
                        {
                            product.ProdID = Product.Insert(product);

                            if (Global.OnlineSetup)
                            {
                                try
                                {
                                    if (!Product.DataRequestInsert(con, product, false))
                                    {
                                        response.Bad("PLC Timeout");
                                        return SaveResponse.Serialize(response);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Global.LogError(ex);
                                    response.Bad("Error saving product");
                                    return SaveResponse.Serialize(response);
                                }

                                if (!SendToPLC(response, con, product))
                                    return SaveResponse.Serialize(response);
                            }
                        }
                    }
                }
            }

            if (Global.OnlineSetup)
                SendAttributeCountsToPLC();

            response.Good($"Product {product.ProdLabel} Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveProducts(Product[] products)
        {
            SaveResponse response = new SaveResponse("Products");

            foreach (Product product in products)
            {
                try
                {
                    product.AddThicksWidths(Thickness.GetAtID(product.ThicknessID), Width.GetAtID(product.WidthID));
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving product");
                    return SaveResponse.Serialize(response);
                }
                if (Product.Exists(product))
                {
                    response.Bad("Product with the same attributes already exists");
                    return SaveResponse.Serialize(response);
                }

                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                {
                    con.Open();

                    try
                    {
                        product.ProdID = Product.Save(product);
                        response.AddEdits(product.EditsList);
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        response.Bad("Error saving product");
                        return SaveResponse.Serialize(response);
                    }

                    if (Global.OnlineSetup)
                    {
                        try
                        {
                            if (!Product.DataRequestInsert(con, product, false))
                            {
                                response.Bad("PLC Timeout");
                                return SaveResponse.Serialize(response);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.LogError(ex);
                            response.Bad("Error saving product");
                            return SaveResponse.Serialize(response);
                        }

                        if (!SendToPLC(response, con, product))
                            return SaveResponse.Serialize(response);
                    }
                    else
                    {
                        IEnumerable<Edit> LabelChanges = product.EditsList.Where(p => p.EditedCol == "ProductLabel");
                        Sort.UpdateLabels(LabelChanges, con);
                        Bin.UpdateLabels(LabelChanges, con);

                        LabelChanges = product.EditsList.Where(p => p.EditedCol == "GradeID");
                        foreach (Edit edit in LabelChanges)
                        {
                            edit.EditedVal = Grade.GetGradeLabel(Convert.ToInt32(edit.EditedVal));
                            edit.Previous = Grade.GetGradeLabel(Convert.ToInt32(edit.Previous));
                        }
                        Sort.UpdateLabels(LabelChanges, con);
                        Bin.UpdateLabels(LabelChanges, con);
                    }
                }

                response.AddEdits(product.EditsList);
            }

            if (Global.OnlineSetup)
                SendAttributeCountsToPLC();

            response.Good(products.Length > 0 ? "Products Saved!" : "Product Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string AddNewGrade(Grade grade)
        {
            SaveResponse response = new SaveResponse("Grades");

            try
            {
                Grade.Save(grade);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving grade");
                return SaveResponse.Serialize(response);
            }

            response.Good($"Grade {grade.GradeLabel} Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveGrades(Grade[] grades)
        {
            SaveResponse response = new SaveResponse("GradeMatrix");

            foreach (Grade grade in grades)
            {
                try
                {
                    Grade.Save(grade);

                    using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                    {
                        con.Open();

                        IEnumerable<Edit> LabelChanges = grade.EditsList.Where(p => p.EditedCol == "GradeLabel");
                        Sort.UpdateLabels(LabelChanges, con);
                        Bin.UpdateLabels(LabelChanges, con);
                    }

                    response.AddEdits(grade.EditsList);
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving grade");
                    return SaveResponse.Serialize(response);
                }
            }

            response.Good(grades.Length > 0 ? "Grades Saved!" : "Grade Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string AddNewThick(Thickness thick)
        {
            SaveResponse response = new SaveResponse();

            try
            {
                Thickness.Insert(thick);

                if (Global.OnlineSetup)
                {
                    using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                    {
                        con.Open();

                        Thickness.DataRequestInsert(con, thick);
                    }
                }
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving thickness");
                return SaveResponse.Serialize(response);
            }

            response.Good($"Thickness #{thick.ID} - {thick.Nominal} Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveThicks(Thickness[] thicks)
        {
            SaveResponse response = new SaveResponse("Thickness");

            foreach (Thickness thick in thicks)
            {
                try
                {
                    Thickness.Save(thick);
                    IEnumerable<Product> products = Product.UpdateAtThickness(thick);

                    if (Global.OnlineSetup)
                    {
                        using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                        {
                            con.Open();

                            Thickness.DataRequestInsert(con, thick);
                            foreach (Product product in products)
                            {
                                Product.DataRequestInsert(con, product, false);
                            }
                        }
                    }

                    response.AddEdits(thick.EditsList);
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving thickness");
                    return SaveResponse.Serialize(response);
                }
            }

            response.Good(thicks.Length > 0 ? "Thicknesses Saved!" : "Thickness Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string AddNewWidth(Width width)
        {
            SaveResponse response = new SaveResponse();

            try
            {
                Width.Insert(width);

                if (Global.OnlineSetup)
                {
                    using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                    {
                        con.Open();

                        Width.DataRequestInsert(con, width);
                    }
                }
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving width");
                return SaveResponse.Serialize(response);
            }

            response.Good($"Width #{width.ID} - {width.Nominal} Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveWidths(Width[] widths)
        {
            SaveResponse response = new SaveResponse("Width");

            foreach (Width width in widths)
            {
                try
                {
                    Width.Save(width);
                    IEnumerable<Product> products = Product.UpdateAtWidth(width);

                    if (Global.OnlineSetup)
                    {
                        using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                        {
                            con.Open();

                            Width.DataRequestInsert(con, width);
                            foreach (Product product in products)
                            {
                                Product.DataRequestInsert(con, product, false);
                            }
                        }
                    }

                    response.AddEdits(width.EditsList);
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving width");
                    return SaveResponse.Serialize(response);
                }
            }

            response.Good(widths.Length > 0 ? "Widths Saved!" : "Width Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string AddNewLength(Length length)
        {
            SaveResponse response = new SaveResponse("Lengths");

            try
            {
                Length.Save(length);

                if (Global.OnlineSetup)
                {
                    using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                    {
                        con.Open();

                        Length.DataRequestInsert(con, length, false);
                    }
                }
                response.AddEdits(length.EditsList);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving grade");
                return SaveResponse.Serialize(response);
            }

            response.Good($"Length {length.LengthLabel} Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveLengths(Length[] lengths)
        {
            SaveResponse response = new SaveResponse("Lengths");

            foreach (Length length in lengths)
            {
                try
                {
                    Length.Save(length);

                    if (Global.OnlineSetup)
                    {
                        using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                        {
                            con.Open();

                            Length.DataRequestInsert(con, length, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving length");
                    return SaveResponse.Serialize(response);
                }
                response.AddEdits(length.EditsList);
            }

            response.Good(lengths.Length > 1 ? "Lengths Saved!" : "Length Saved");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string AddNewPETLength(PETLength PETLength)
        {
            SaveResponse response = new SaveResponse("PETLengths");
            Length NewLength = new Length();

            try
            {
                PETLength.Save(PETLength);

                NewLength.PETLengthID = PETLength.PETLengthID;
                NewLength.PETFlag = true;
                NewLength.LengthLabel = PETLength.LengthLabel;
                Length.Save(NewLength);

                if (Global.OnlineSetup)
                {
                    using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                    {
                        con.Open();

                        PETLength.DataRequestInsert(con, PETLength, false);
                        Length.DataRequestInsert(con, NewLength, false);
                    }
                }
                response.AddEdits(PETLength.EditsList);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving PETLength");
                return SaveResponse.Serialize(response);
            }

            response.Good($"PETLength {PETLength.LengthLabel} Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SavePETLengths(PETLength[] PETLengths)
        {
            SaveResponse response = new SaveResponse("PETLengths");

            foreach (PETLength length in PETLengths)
            {
                try
                {
                    PETLength.Save(length);

                    if (Global.OnlineSetup)
                    {
                        using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                        {
                            con.Open();

                            PETLength.DataRequestInsert(con, length, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving PETlength");
                    return SaveResponse.Serialize(response);
                }
                response.AddEdits(length.EditsList);
            }

            response.Good(PETLengths.Length > 1 ? "PETLengths Saved!" : "PETLength Saved");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveGraders(Graders[] graders)
        {
            SaveResponse response = new SaveResponse("Graders");

            foreach (Graders grader in graders)
            {
                try
                {
                    Graders.Save(grader);
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    response.Bad("Error saving grades");
                    return SaveResponse.Serialize(response);
                }
                response.AddEdits(grader.EditsList);
            }

            response.Good(graders.Length > 1 ? "Graders Saved!" : "Grader Saved");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string AddNewGraders(Graders graders)
        {
            SaveResponse response = new SaveResponse("Graders");

            try
            {
                Graders.Save(graders);
                response.AddEdits(graders.EditsList);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving grade");
                return SaveResponse.Serialize(response);
            }

            response.Good($"Grader {graders.GraderDescription} Saved!");
            return SaveResponse.Serialize(response);
        }

        [WebMethod]
        public static string SaveNearSaw(int NearSawOffSet)
        {
            SaveResponse response = new SaveResponse();

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd1 = new SqlCommand($"UPDATE WebSortSetup SET NearSawOffSet = {NearSawOffSet}", con))
                    cmd1.ExecuteNonQuery();
                if (Global.OnlineSetup)
                {
                    using (SqlCommand cmdip = new SqlCommand("SELECT * FROM RaptorCommSettings", con))
                    using (SqlDataReader readerip = cmdip.ExecuteReader())
                    {
                        while (readerip.Read())
                        {
                            Global.MyPLC.IPAddress = readerip["PLCIPAddress"].ToString();
                            Global.MyPLC.Path = readerip["PLCProcessorSlot"].ToString();
                            Global.MyPLC.Timeout = int.Parse(readerip["PLCTimeout"].ToString());
                        }
                    }
                    if (Global.MyPLC.Connect() != ResultCode.E_SUCCESS)
                    {
                        response.Bad("PLC Not Connected");
                        return SaveResponse.Serialize(response);
                    }
                    Tag MyTag = new Tag("PETOffset.Saw[0]");
                    MyTag.DataType = Tag.ATOMIC.REAL;

                    try
                    {
                        MyTag.Value = NearSawOffSet;
                        Global.MyPLC.WriteTag(MyTag);
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                    }
                    Global.MyPLC.Disconnect();
                }
            }
            response.Good("Saved!");
            return SaveResponse.Serialize(response);
        }

        public static bool SendToPLC(SaveResponse response, SqlConnection con, Product product)
        {
            //send product to PLC
            using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 4", con))
                cmd.ExecuteNonQuery();

            try
            {
                using (SqlCommand cmd = new SqlCommand(Product.DataRequestSql, con))
                {
                    cmd.Parameters.AddWithValue("@ProductID", product.ProdID);
                    cmd.Parameters.AddWithValue("@Active", true);
                    cmd.Parameters.AddWithValue("@ThicknessID", product.ThicknessID);
                    cmd.Parameters.AddWithValue("@WidthID", product.WidthID);
                    cmd.Parameters.AddWithValue("@GradeID", product.GradeID);
                    cmd.Parameters.AddWithValue("@MoistureID", 0);
                    cmd.Parameters.AddWithValue("@SpecID", 0);
                    cmd.Parameters.AddWithValue("@SpecialX", 0);
                    cmd.Parameters.AddWithValue("@SpecialY", 0);
                    cmd.Parameters.AddWithValue("@Specialz", 0);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!Raptor.MessageAckConfirm("DataRequestsProduct", Global.GetValue<int>(reader, "id")))
                            {
                                response.Bad("PLC Timeout");
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving product");
                return false;
            }

            using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-4 where (datarequests & 4)=4", con))
                cmd.ExecuteNonQuery();

            return true;
        }
    }
}