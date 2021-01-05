using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebSort.Model
{
    public class ProductLengths
    {
        public List<Products> ProductsList { get; set; }
        public List<Lengths> LengthsList { get; set; }

        public ProductLengths()
        {
            ProductsList = new List<Products>();
            LengthsList = new List<Lengths>();
        }

        public class Products
        {
            public int ID { get; set; }
            public string Label { get; set; }
            public bool Selected { get; set; }
        }

        public class Lengths
        {
            public int ID { get; set; }
            public string Label { get; set; }
            public bool Selected { get; set; }
        }

        public const string BinProdMapSql = @"
            SELECT products.prodid, prodlabel, 'selected' = 1, ThickNominal, WidthNominal, gradelabel
            FROM products,binproducts,grades with(NOLOCK)
            WHERE deleted = 0 AND products.prodid > 0 AND products.prodid = binproducts.prodid
            AND products.gradeid = grades.gradeid
            AND BinID = @BinID
            UNION
            SELECT products.prodid, prodlabel, 'selected' = 0, ThickNominal, WidthNominal, gradelabel
            FROM products,grades with(NOLOCK)
            WHERE deleted = 0 AND products.prodid > 0 AND products.gradeid = grades.gradeid
            AND products.prodid NOT IN (SELECT prodid FROM binproducts with(NOLOCK) WHERE BinID = @BinID)
            ORDER BY ThickNominal, WidthNominal, ProdID";
        public const string BinLengthMapSql = @"
            SELECT lengths.lengthid, lengthlabel, 'selected' = 1, lengthnominal, petflag
            FROM lengths,BinLengths with(NOLOCK)
            WHERE lengths.lengthid > 0 AND lengths.lengthid = BinLengths.LengthID AND BinID = @BinID
            UNION
            SELECT lengths.lengthid, lengthlabel, 'selected' = 0, lengthnominal, petflag
            FROM lengths with(NOLOCK)
            WHERE lengths.lengthid > 0
            AND lengths.lengthid NOT IN (SELECT lengthid FROM BinLengths with(NOLOCK) WHERE BinID = @BinID)
            ORDER BY petflag, lengthnominal";

        public const string SortProdMapSql = @"
            SELECT
                DISTINCT(products.prodid),
                prodlabel,
                'selected'=1,
                thicknominal,
                widthnominal,
                gradelabel
            FROM products,sortproducts,grades with(NOLOCK)
            WHERE deleted = 0
                AND products.prodid>0
                AND products.prodid=sortproducts.prodid
                AND products.gradeid=grades.gradeid
                AND recipeid=@RecipeID
                AND SortProducts.RecipeID = @RecipeID
                AND sortid=@SortID
            UNION
            SELECT
                products.prodid,
                prodlabel,
                'selected'=0,
                thicknominal,
                widthnominal,
                gradelabel
            FROM products,grades with(NOLOCK)
            WHERE deleted = 0
                AND products.prodid>0
                AND products.gradeid=grades.gradeid
                AND NOT EXISTS (SELECT prodid FROM sortproducts with(NOLOCK) 
                                WHERE recipeid = @RecipeID AND sortid = @SortID 
                                AND products.ProdID = SortProducts.ProdID)
            ORDER BY thicknominal,widthnominal,gradelabel";

        public const string SortLengthMapSql = @"
            SELECT
                lengths.lengthid,
                lengthlabel,
                'selected' = 1,
                lengthnominal,
                petflag
            FROM lengths,sortlengths with(NOLOCK)
            WHERE lengths.lengthid > 0
                AND lengths.lengthid = sortlengths.lengthid
                AND sortlengths.recipeid = @RecipeID
                AND sortid = @SortID
            UNION
            SELECT
                lengths.lengthid,
                lengthlabel,
                'selected' = 0,
                lengthnominal,
                petflag
            FROM lengths with(NOLOCK)
            WHERE lengths.lengthid > 0
                AND NOT EXISTS (SELECT lengthid FROM sortlengths with(NOLOCK) 
                                WHERE recipeid = @RecipeID AND sortid = @SortID 
                                AND Lengths.LengthID = SortLengths.LengthID)
            ORDER BY petflag, lengthnominal";

        public void PopulateProductList(SqlDataReader reader)
        {
            while (reader.Read())
            {
                ProductsList.Add(new Products
                {
                    ID = Global.GetValue<int>(reader, "prodid"),
                    Label = $"{reader["prodlabel"].ToString().Replace(" x ", "x")} {reader["gradelabel"]}",
                    Selected = Global.GetValue<bool>(reader, "selected")
                });
            }
        }

        public void PopulateLengthList(SqlDataReader reader)
        {
            while (reader.Read())
            {
                LengthsList.Add(new Lengths
                {
                    ID = Global.GetValue<int>(reader, "lengthid"),
                    Label = reader["lengthlabel"].ToString(),
                    Selected = Global.GetValue<bool>(reader, "selected")
                });
            }
        }
    }
}