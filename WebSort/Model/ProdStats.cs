using Mighty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSort.Model
{
    public class ProdStats
    {
        public IEnumerable<ProdStatsLengths> Lengths { get; set; }
        public IEnumerable<ProdStatsGrades> Grades { get; set; }
        public IEnumerable<ProdStatsProducts> Products { get; set; }

        private const string
            ProdLengthsSql = @"
                SELECT
	                Length = LengthLabel ,
	                Pieces = SUM(BoardCount),
	                Volume = CONVERT(int,SUM(BoardCount * ThickNominal * WidthNominal * LengthNominal / 144 * @TrimLossFactor))
                FROM
	                ProductionBoards,Products,Lengths
                WHERE
	                Sorted=1
	                AND ProductionBoards.ProdID=Products.ProdID
	                AND Lengths.LengthID=ProductionBoards.LengthID
                Group BY
	                LengthLabel,LengthNominal
                ORDER BY
	                Volume DESC,LengthNominal",
            ProdGradesSql = @"
                SELECT
	                Grade = GradeLabel,
	                Pieces = SUM(BoardCount),
	                Volume = CONVERT(int,SUM(BoardCount * ThickNominal * WidthNominal * LengthNominal / 144 * @TrimLossFactor))
                FROM ProductionBoards,Grades,Products,Lengths
                WHERE
	                products.prodid> 0
	                AND sorted=1
	                AND ProductionBoards.prodid=Products.ProdID
	                AND products.gradeid=grades.gradeid
	                AND lengths.lengthid=ProductionBoards.lengthid
                GROUP BY
	                gradelabel
                ORDER BY
	                Volume desc,gradelabel",
            ProdProductsSql = @"
                SELECT
	                Product = ProdLabel ,
	                ThickNominal,
	                WidthNominal,
	                Pieces = SUM(BoardCount),
	                Volume = CONVERT(int,SUM(BoardCount * ThickNominal * WidthNominal * LengthNominal / 144 * @TrimLossFactor))
                FROM
	                ProductionBoards,Products,Lengths
                WHERE
	                Products.prodid>0
	                AND sorted=1
	                AND ProductionBoards.prodid=Products.ProdID
	                AND	lengths.lengthid=ProductionBoards.lengthid
                GROUP BY
	                prodlabel,thicknominal,widthnominal
                ORDER BY
	                Volume desc,thicknominal,widthnominal";

        public void GetLengthsData(float TrimLossFactor)
        {
            MightyOrm<ProdStatsLengths> mighty = new MightyOrm<ProdStatsLengths>(Global.MightyConString);
            Lengths = mighty.QueryWithParams(ProdLengthsSql, inParams: new { TrimLossFactor });
        }

        public void GetGradesData(float TrimLossFactor)
        {
            MightyOrm<ProdStatsGrades> mighty = new MightyOrm<ProdStatsGrades>(Global.MightyConString);
            Grades = mighty.QueryWithParams(ProdGradesSql, inParams: new { TrimLossFactor });
        }

        public void GetProductsData(float TrimLossFactor)
        {
            MightyOrm<ProdStatsProducts> mighty = new MightyOrm<ProdStatsProducts>(Global.MightyConString);
            Products = mighty.QueryWithParams(ProdProductsSql, inParams: new { TrimLossFactor });
        }

        public void GetData(float TrimLossFactor)
        {
            GetLengthsData(TrimLossFactor);
            GetGradesData(TrimLossFactor);
            GetProductsData(TrimLossFactor);
        }
    }

    public class ProdStatsLengths
    {
        public string Length { get; set; }
        public int Pieces { get; set; }
        public int Volume { get; set; }
    }

    public class ProdStatsGrades
    {
        public string Grade { get; set; }
        public int Pieces { get; set; }
        public int Volume { get; set; }
    }

    public class ProdStatsProducts
    {
        public string Product { get; set; }
        public int ThickNominal { get; set; }
        public int WidthNominal { get; set; }
        public int Pieces { get; set; }
        public int Volume { get; set; }
    }
}