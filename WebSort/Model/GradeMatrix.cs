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
        public int GradeStamps { get; set; }
        public List<Edit> EditsList { get; set; }

        public GradeMatrix()
        {
            EditsList = new List<Edit>();
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
            select getdate(),@PLCGradeID,@GradeID,@Stamps,1,0;
            select id=(select max(id) from datarequestsgrade with(NOLOCK))";

        /// <summary>
        /// @Stamps, @WebSortGradeID, @PLCGradeID, @RecipeID
        /// </summary>
        public const string GradeMatrixUpdate = @"
            UPDATE GradeMatrix
            SET GradeStamps = @Stamps,websortgradeid = @WebSortGradeID
            WHERE plcgradeid = @PLCGradeID and recipeid = @RecipeID";

        #endregion SQL String

        public static List<GradeMatrix> GetData()
        {
            List<GradeMatrix> grades = new List<GradeMatrix>();

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using SqlCommand cmd = new SqlCommand(GradeMatrixSql, con);
                cmd.Parameters.AddWithValue("@RecipeID", Recipe.GetEditingRecipe().RecipeID);

                using SqlDataReader reader = cmd.ExecuteReader();
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
                            GradeStamps = Global.GetValue<int>(reader, "GradeStamps")
                        });
                    }
                }
            }
            return grades;
        }

        public static bool SaveData(GradeMatrix matrix, SqlConnection con, int RecipeID)
        {
            bool ret = false;
            if (matrix?.EditsList.Count > 0)
            {
                if (Recipe.GetOnlineRecipe().RecipeID == RecipeID && Global.OnlineSetup)
                {
                    try
                    {
                        if (!SavePLC(matrix, con))
                            return false;
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        throw;
                    }
                }

                using (SqlCommand cmd = new SqlCommand(GradeMatrixUpdate, con))
                {
                    cmd.Parameters.AddWithValue("@PLCGradeID", matrix.PLCGradeID);
                    cmd.Parameters.AddWithValue("@Stamps", matrix.GradeStamps);
                    cmd.Parameters.AddWithValue("@WebSortGradeID", matrix.WebSortGradeID);
                    cmd.Parameters.AddWithValue("@RecipeID", RecipeID);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        ret = true;
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        throw;
                    }
                }
                return ret;
            }
            else
            {
                return true;
            }
        }

        private static bool SavePLC(GradeMatrix matrix, SqlConnection con)
        {
            using (SqlCommand cmd = new SqlCommand(ComSettingsSql, con))
                cmd.ExecuteNonQuery();

            using (SqlCommand cmd = new SqlCommand(DataRequestsGradeSql, con))
            {
                cmd.Parameters.AddWithValue("@PLCGradeID", matrix.PLCGradeID);
                cmd.Parameters.AddWithValue("@Stamps", matrix.GradeStamps);
                cmd.Parameters.AddWithValue("@GradeID", matrix.WebSortGradeID);

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!Raptor.MessageAckConfirm("datarequestsgrade", Global.GetValue<int>(reader, "id")))
                        {
                            return false;
                        }
                    }
                }
            }

            using (SqlCommand cmd = new SqlCommand(ComSettingsAfterSql, con))
                cmd.ExecuteNonQuery();

            return true;
        }
    }
}