using Mighty;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebSort.Model
{
    public class Graders
    {
        public Graders()
        {
            EditsList = new List<Edit>();
        }

        [Key]
        public int GraderID { get; set; }

        public string GraderDescription { get; set; }
        public int StationID { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public static IEnumerable<Graders> GetAll()
        {
            MightyOrm<Graders> db = new MightyOrm<Graders>(Global.MightyConString, "Graders", "GraderID");
            return db.All();
        }

        public static int Insert(Graders grader)
        {
            MightyOrm<Graders> db = new MightyOrm<Graders>(Global.MightyConString, "Graders", "GraderID");
            db.Insert(grader);

            return grader.GraderID;
        }

        public static int Save(Graders grader)
        {
            MightyOrm<Graders> db = new MightyOrm<Graders>(Global.MightyConString, "Graders", "GraderID");
            db.Save(grader);

            return grader.GraderID;
        }
    }
}