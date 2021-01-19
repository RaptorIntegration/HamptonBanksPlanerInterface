using Mighty;

using System.Collections.Generic;

namespace WebSort.Model
{
    public class Grade
    {
        public int GradeID { get; set; }
        public string GradeLabel { get; set; }
        public string GradeDescription { get; set; }
        public string GradeLabelTicket { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public Grade()
        {
            EditsList = new List<Edit>();
        }

        /// <summary>
        ///  @GradeID, @GradeIDX, @GradeStamps, @Write, @Processed
        /// </summary>
        public const string DataRequestsGradeSql = @"
            INSERT INTO [DataRequestsGrade]
            SELECT
                GETDATE(),
                @GradeID,
                @GradeIDX,
                @GradeStamps,
                @Write,
                @Processed;
            SELECT id=(SELECT MAX(id) FROM DataRequestsGrade WITH(NOLOCK))";

        public static IEnumerable<Grade> GetAllData()
        {
            MightyOrm<Grade> db = new MightyOrm<Grade>(Global.MightyConString, "Grades", "GradeID");
            return db.All();
        }

        public static void Save(Grade grade)
        {
            MightyOrm<Grade> db = new MightyOrm<Grade>(Global.MightyConString, "Grades", "GradeID");
            db.Save(grade);
        }

        public static string GetGradeLabel(int ID)
        {
            MightyOrm<Grade> db = new MightyOrm<Grade>(Global.MightyConString, "Grades", "GradeID");
            return db.Single(ID).GradeLabel;
        }
    }
}