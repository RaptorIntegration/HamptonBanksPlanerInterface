using Mighty;
using System;
using System.Collections.Generic;

namespace WebSort.Model
{
    public class Board
    {
        public int id { get; set; }
        public DateTime? timestamp { get; set; }
        public string TimeString => string.Format("{0:yy/MM/dd H:mm}", timestamp);
        public int? LugNum { get; set; }
        public int? TrackNum { get; set; }
        public int? BinID { get; set; }
        public string BinLabel { get; set; }
        public int? ProdID { get; set; }
        public string ProdLabel { get; set; }
        public int? LengthID { get; set; }
        public string LengthLabel { get; set; }
        public float? ThickActual { get; set; }
        public float? WidthActual { get; set; }
        public float? LengthIn { get; set; }
        public string LengthInLabel { get; set; }
        public string Saws { get; set; }
        public int? NET { get; set; }
        public string NETLabel { get; set; }
        public int? FET { get; set; }
        public string FETLabel { get; set; }
        public int? CN2 { get; set; }
        public string CN2Label { get; set; }
        public float? Fence { get; set; }
        public string FenceLabel { get; set; }
        public int? BinCount { get; set; }
        public int? Flags { get; set; }
        public int? Devices { get; set; }

        private const string BoardsSql = @"
                SELECT TOP 50
                    [id], [LugNum], [TrackNum], boards.ProdLabel,
                    [LengthID], [LengthLabel], [ThickActual], [WidthActual], [LengthInLabel], [Saws], [NET],
                    [NETLabel], [FETLabel], [CN2Label], [FenceLabel], [BinCount],
                    Flags, [timestamp], [BinLabel]
                FROM
                    [Boards] with(NOLOCK)
                ORDER BY TimeStamp DESC";

        public static IEnumerable<Board> GetAll()
        {
            MightyOrm<Board> mighty = new MightyOrm<Board>(Global.MightyConString);
            return mighty.Query(BoardsSql);
        }
    }
}