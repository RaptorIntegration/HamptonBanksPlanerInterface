using Mighty;

using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebSort.Model
{
    public class Thickness
    {
        [DatabasePrimaryKey]
        public int ID { get; set; }

        public string Label { get; set; }

        public float Nominal { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public Thickness()
        {
            EditsList = new List<Edit>();
        }

        /// <summary>
        /// @ThickNessID, @ThickMin, @ThickMax, @ThickNom,@Write,@Processed
        /// </summary>
        public const string DataRequestSql = @"
                INSERT INTO DataRequestsThickness
                SELECT
                    GETDATE(), @ThicknessID, @ThickMin, @ThickMax, @ThickNom, @Write, @Processed;
                SELECT ID = (SELECT MAX(ID) FROM DataRequestsThickness WITH(NOLOCK))";

        public static IEnumerable<Thickness> GetAll()
        {
            MightyOrm<Thickness> db = new MightyOrm<Thickness>(Global.MightyConString, "Thickness", "ID");
            return db.AllWithParams("WHERE ID > 0");
        }

        public static Thickness GetAtID(int ID)
        {
            MightyOrm<Thickness> db = new MightyOrm<Thickness>(Global.MightyConString, "Thickness", "ID");
            return db.Single(ID);
        }

        public static int Insert(Thickness thick)
        {
            thick.ID = GetLowestAvailableID();
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Thickness SELECT @ID, @Label, @Nom, @Min, @Max", con))
                {
                    cmd.Parameters.AddWithValue("@ID", thick.ID);
                    cmd.Parameters.AddWithValue("@Label", thick.Label);
                    cmd.Parameters.AddWithValue("@Nom", thick.Nominal);
                    cmd.Parameters.AddWithValue("@Min", thick.Minimum);
                    cmd.Parameters.AddWithValue("@Max", thick.Maximum);

                    cmd.ExecuteNonQuery();
                }
            }

            return thick.ID;
        }

        public static void Save(Thickness thick)
        {
            MightyOrm<Thickness> db = new MightyOrm<Thickness>(Global.MightyConString, "Thickness", "ID");
            db.Save(thick);
        }

        public static bool DataRequestInsert(SqlConnection con, Thickness thick, bool CommSettings = true, bool ZeroOut = false)
        {
            //if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 16", con);
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = new SqlCommand(DataRequestSql, con))
            {
                if (ZeroOut)
                {
                    cmd.Parameters.AddWithValue("@ThicknessID", thick.ID);
                    cmd.Parameters.AddWithValue("@ThickMin", 0);
                    cmd.Parameters.AddWithValue("@ThickMax", 0);
                    cmd.Parameters.AddWithValue("@ThickNom", 0);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ThicknessID", thick.ID);
                    cmd.Parameters.AddWithValue("@ThickMin", thick.Minimum);
                    cmd.Parameters.AddWithValue("@ThickMax", thick.Maximum);
                    cmd.Parameters.AddWithValue("@ThickNom", thick.Nominal);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!Raptor.MessageAckConfirm("DataRequestsThickness", Global.GetValue<int>(reader, "id")))
                    {
                        return false;
                    }
                }
            }

            //if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-16 where (datarequests & 16)=16", con);
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public static int GetLowestAvailableID()
        {
            int ret = 0;
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 t1.Id+1 ret FROM Thickness t1 WHERE NOT EXISTS(SELECT * FROM Thickness t2 WHERE t2.Id = t1.Id + 1) ORDER BY t1.Id", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret = Global.GetValue<int>(reader, "ret");
                    }
                }
            }
            return ret;
        }
    }
}