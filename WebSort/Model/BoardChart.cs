using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSort.Model
{
    public class BoardChart
    {
        public BoardChart()
        {
            VPH = new List<decimal>();
            PPH = new List<decimal>();
            LF = new List<decimal>();
        }

        public List<decimal> LF { get; set; }
        public List<decimal> PPH { get; set; }
        public decimal TLF { get; set; }
        public decimal TPPH { get; set; }
        public decimal TVPH { get; set; }
        public List<decimal> VPH { get; set; }
    }
}