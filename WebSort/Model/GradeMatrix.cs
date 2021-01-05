using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebSort.Model
{
    public class GradeMatrix
    {
        public int PLCGradeID { get; set; }
        public string GradeLabel { get; set; }
        public int WebSortGradeID { get; set; }
        public int Default { get; set; }
        public uint GradeStamps { get; set; }
        public List<Stamp> SelectedStamps { get; set; }
        public List<Edit> EditsList { get; set; }

        public GradeMatrix()
        {
            SelectedStamps = new List<Stamp>();
            EditsList = new List<Edit>();
        }

        public class Stamp
        {
            public int ID { get; set; }
            public string Description { get; set; }
            public bool Selected { get; set; }

            public static List<Stamp> GetStamps()
            {
                List<Stamp> stamps = new List<Stamp>();

                using (SqlConnection con = new SqlConnection(Global.ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Stamps WHERE StampID > 0", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                stamps.Add(new Stamp
                                (
                                    Global.GetValue<int>(reader, "StampID"),
                                    Global.GetValue<string>(reader, "StampDescription"),
                                    false
                                ));
                            }
                        }
                    }
                }
                return stamps;
            }

            public Stamp(int id, string description, bool selected)
            {
                ID = id;
                Description = description;
                Selected = selected;
            }

            public Stamp()
            {
            }
        }

        #region SQL String

        /// <summary>
        /// @RecipeID
        /// </summary>
        public const string GradeMatrixSql = @"
            SELECT GradeMatrix.PLCGradeID, Grades.GradeLabel, websortgradeid,GradeMatrix.[Default],gradematrix.gradestamps
            FROM GradeMatrix
            INNER JOIN Grades ON GradeMatrix.WEBSortGradeID = Grades.GradeID
            WHERE GradeMatrix.RecipeID = @RecipeID
            ORDER BY GradeMatrix.PLCGradeID";

        public const string ComSettingsSql = "update RaptorCommSettings set DataRequests = DataRequests | 8";
        public const string ComSettingsAfterSql = "update RaptorCommSettings set datarequests = datarequests-8 where (datarequests & 8)=8";

        /// <summary>
        /// @PLCGradeID, @Stamps, @GradeLabel
        /// </summary>
        public const string DataRequestsGradeSql = @"
            insert into datarequestsgrade
            select getdate(),@PLCGradeID,gradeid,@Stamps,1,0
            from grades
            where gradelabel='@GradeLabel';
            select id=(select max(id) from datarequestsgrade with(NOLOCK))";

        /// <summary>
        /// @Stamps, @GradeLabel, @PLCGradeID, @RecipeID
        /// </summary>
        public const string GradeMatrixUpdate = @"
            UPDATE GradeMatrix
            SET GradeStamps = @Stamps,websortgradeid = (SELECT GradeID FROM Grades WHERE GradeLabel = @GradeLabel)
            WHERE plcgradeid = @PLCGradeID and recipeid = @RecipeID";

        #endregion SQL String

        public static List<GradeMatrix> GetData()
        {
            List<Stamp> stamps = Stamp.GetStamps();
            List<GradeMatrix> grades = new List<GradeMatrix>();

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(GradeMatrixSql, con))
                {
                    cmd.Parameters.AddWithValue("@RecipeID", Recipe.GetEditingRecipe().RecipeID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                grades.Add(new GradeMatrix
                                {
                                    PLCGradeID = Global.GetValue<int>(reader, "PLCGradeID"),
                                    GradeLabel = Global.GetValue<string>(reader, "GradeLabel"),
                                    WebSortGradeID = Global.GetValue<int>(reader, "WebSortGradeID"),
                                    Default = Global.GetValue<int>(reader, "Default"),
                                    GradeStamps = Global.GetValue<uint>(reader, "GradeStamps"),
                                    SelectedStamps = GetSelectedStamps(Global.GetValue<uint>(reader, "GradeStamps"), stamps)
                                });
                            }
                        }
                    }
                }
            }
            return grades;
        }

        public static bool SaveData(GradeMatrix matrix, SqlConnection con, int RecipeID)
        {
            if (matrix?.EditsList.Count > 0)
            {
                int Stamps = GetStampsBitMap(matrix.SelectedStamps);
                return SavePLC(matrix.PLCGradeID, Stamps, matrix.GradeLabel, con, RecipeID);
            }
            else
            {
                return true;
            }
        }

        private static bool SavePLC(int PLCGradeID, int Stamps, string GradeLabel, SqlConnection con, int RecipeID)
        {
            if (Global.OnlineSetup)
            {
                using (SqlCommand cmd = new SqlCommand(ComSettingsSql, con))
                    cmd.ExecuteNonQuery();

                using (SqlCommand cmd = new SqlCommand(DataRequestsGradeSql, con))
                {
                    cmd.Parameters.AddWithValue("@PLCGradeID", PLCGradeID);
                    cmd.Parameters.AddWithValue("@Stamps", Stamps);
                    cmd.Parameters.AddWithValue("@GradeLabel", GradeLabel);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                bool succeeded = Raptor.MessageAckConfirm("datarequestsgrade", Global.GetValue<int>(reader, "id"));
                                if (!succeeded)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(ComSettingsAfterSql, con))
                    cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = new SqlCommand(GradeMatrixUpdate, con))
            {
                cmd.Parameters.AddWithValue("@PLCGradeID", PLCGradeID);
                cmd.Parameters.AddWithValue("@Stamps", Stamps);
                cmd.Parameters.AddWithValue("@GradeLabel", GradeLabel);
                cmd.Parameters.AddWithValue("@RecipeID", RecipeID);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    return false;
                }
            }

            return true;
        }

        private static int GetStampsBitMap(List<Stamp> stamps)
        {
            int ret = 0;
            foreach (Stamp stamp in stamps)
            {
                if (stamp.Selected)
                {
                    ret |= (int)Math.Pow(2, stamp.ID);
                }
            }
            return ret;
        }

        private static List<Stamp> GetSelectedStamps(uint GradeStamps, List<Stamp> stamps)
        {
            List<Stamp> Clone = stamps.ConvertAll(s => new Stamp(s.ID, s.Description, s.Selected));

            foreach (Stamp stamp in Clone)
            {
                stamp.Selected = (GradeStamps & (int)Math.Pow(2, stamp.ID)) == (int)Math.Pow(2, stamp.ID);
            }

            return Clone;
        }
    }
}