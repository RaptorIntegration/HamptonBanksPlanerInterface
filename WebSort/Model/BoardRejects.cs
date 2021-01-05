using Mighty;
using System.Collections.Generic;

namespace WebSort.Model
{
    public class BoardReject
    {
        public int RejectFlag { get; set; }
        public string RejectDescription { get; set; }
        public string Colour { get; set; }

        public static IEnumerable<BoardReject> GetAll()
        {
            MightyOrm<BoardReject> db = new MightyOrm<BoardReject>(Global.MightyConString, "BoardRejects", "RejectFlag");
            return db.All();
        }

        public static void Save(BoardReject reject)
        {
            MightyOrm<BoardReject> db = new MightyOrm<BoardReject>(Global.MightyConString, "BoardRejects", "RejectFlag");
            db.Update(reject);
        }
    }
}