using Mighty;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;

namespace WebSort.Model
{
    public class Product
    {
        [Key]
        public int ProdID { get; set; }

        public short? Deleted { get; set; }
        public bool? Active { get; set; }
        public string ProdLabel { get; set; }
        public int? GradeID { get; set; }
        public int? MoistureID { get; set; }
        public int? SpecID { get; set; }
        public float ThickNominal { get; set; }
        public float ThickMin { get; set; }
        public float ThickMax { get; set; }
        public float WidthNominal { get; set; }
        public float WidthMin { get; set; }
        public float WidthMax { get; set; }
        public int ThicknessID { get; set; }
        public int WidthID { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public Product()
        {
            EditsList = new List<Edit>();
        }

        /// <summary>
        /// @ProductID, @Active, @ThicknessID, @WidthID, @GradeID, @MoistureID, @SpecID, @SpecialX, @SpecialY, @SpecialZ, @Write, @Processed
        /// </summary>
        public static string DataRequestSql = @"
            INSERT INTO [DataRequestsProduct]
            SELECT
                GETDATE(),
                @ProductID,
                @Active,
                @ThicknessID,
                @WidthID,
                @GradeID,
                @MoistureID,
                @SpecID,
                @SpecialX,
                @SpecialY,
                @SpecialZ,
                @Write,
                @Processed;
            SELECT ID = (SELECT MAX(ID) FROM DataRequestsProduct WITH(NOLOCK))";

        public static IEnumerable<Product> GetAll()
        {
            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");
            return db.AllWithParams("WHERE Deleted = 0");
        }

        public static IEnumerable<Product> GetAllByThick(Thickness thick)
        {
            const string pars = "Deleted=0 AND ThickNominal=@ThickNominal AND ThickMin=@ThickMin AND ThickMax=@ThickMax";

            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");

            return db.AllWithParams(
                where: pars,
                inParams: new {
                    ThickNominal = thick.Nominal,
                    ThickMin = thick.Minimum,
                    ThickMax = thick.Maximum
                });
        }

        public static IEnumerable<Product> GetAllByWidth(Width width)
        {
            const string pars = "Deleted=0 AND WidthNominal=@WidthNominal AND WidthMin=@WidthMin AND WidthMax=@WidthMax";

            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");

            return db.AllWithParams(
                where: pars,
                inParams: new {
                    WidthNominal = width.Nominal,
                    WidthMin = width.Minimum,
                    WidthMax = width.Maximum
                });
        }

        public static Product GetAtID(int ID)
        {
            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");
            return db.Single(ID);
        }

        public Product AddThicksWidths(Thickness thick, Width width)
        {
            ThickMin = thick.Minimum;
            ThickNominal = thick.Nominal;
            ThickMax = thick.Maximum;

            WidthMin = width.Minimum;
            WidthNominal = width.Nominal;
            WidthMax = width.Maximum;

            return this;
        }

        public static void Delete(Product product)
        {
            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");
            db.Delete(product);
        }

        public static int Insert(Product product)
        {
            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");
            db.Insert(product);

            return product.ProdID;
        }

        public static int Save(Product product)
        {
            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");
            db.Save(product);

            return product.ProdID;
        }

        public static IEnumerable<Product> UpdateAtThickness(Thickness thick)
        {
            const string q = "UPDATE Products SET ThickMin=@Min, ThickMax=@Max, ThickNominal=@Nom OUTPUT DELETED.* WHERE ThicknessID=@ID";
            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");
            return db.QueryWithParams(q,
                inParams: new {
                    Min = thick.Minimum,
                    Max = thick.Maximum,
                    Nom = thick.Nominal,
                    ID = thick.ID
                })
                .ToList();
        }

        public static IEnumerable<Product> UpdateAtWidth(Width width)
        {
            const string q = "UPDATE Products SET WidthMin=@Min, WidthMax=@Max, WidthNominal=@Nom OUTPUT DELETED.* WHERE WidthID=@ID";
            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");
            return db.QueryWithParams(q,
                inParams: new {
                    Min = width.Minimum,
                    Max = width.Maximum,
                    Nom = width.Nominal,
                    ID = width.ID
                })
                .ToList();
        }

        public static bool DataRequestInsert(SqlConnection con, Product product, bool CommSettings = true, bool ZeroOut = false)
        {
            if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 4", con);
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = new SqlCommand(DataRequestSql, con))
            {
                if (ZeroOut)
                {
                    cmd.Parameters.AddWithValue("@ProductID", product.ProdID);
                    cmd.Parameters.AddWithValue("@Active", false);
                    cmd.Parameters.AddWithValue("@ThicknessID", 0);
                    cmd.Parameters.AddWithValue("@WidthID", 0);
                    cmd.Parameters.AddWithValue("@GradeID", 0);
                    cmd.Parameters.AddWithValue("@MoistureID", 0);
                    cmd.Parameters.AddWithValue("@SpecID", 0);
                    cmd.Parameters.AddWithValue("@SpecialX", 0);
                    cmd.Parameters.AddWithValue("@SpecialY", 0);
                    cmd.Parameters.AddWithValue("@SpecialZ", 0);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }
                else
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
                    cmd.Parameters.AddWithValue("@SpecialZ", 0);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!Raptor.MessageAckConfirm("DataRequestsProduct", Global.GetValue<int>(reader, "id")))
                    {
                        return false;
                    }
                }
            }

            if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-4 where (datarequests & 4)=4", con);
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public static bool Exists(Product product)
        {
            const string pars = @"
                Deleted=0 AND GradeID=@GradeID
                AND ThickNominal=@ThickNominal AND ThickMin=@ThickMin AND ThickMax=@ThickMax
                AND WidthNominal=@WidthNominal AND WidthMin=@WidthMin AND WidthMax=@WidthMax";

            MightyOrm<Product> db = new MightyOrm<Product>(Global.MightyConString, "Products", "ProdID");
            IEnumerable<Product> res = db.AllWithParams(
                where: pars,
                inParams: new {
                    GradeID = product.GradeID,
                    ThickNominal = product.ThickNominal,
                    ThickMin = product.ThickMin,
                    ThickMax = product.ThickMax,
                    WidthNominal = product.WidthNominal,
                    WidthMin = product.WidthMin,
                    WidthMax = product.WidthMax
                });

            return res.Any();
        }
    }
}