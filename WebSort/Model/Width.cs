using Mighty;

using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebSort.Model
{
    public class Width
    {
        [DatabasePrimaryKey]
        public int ID { get; set; }

        public string Label { get; set; }

        public float Nominal { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public Width()
        {
            EditsList = new List<Edit>();
        }

        /// <summary>
        /// @WidthID, @WidthMin, @WidthMax, @WidthNom, @Write, @Processed
        /// </summary>
        public const string DataRequestSql = @"
                INSERT INTO DataRequestsWidth
                SELECT
                    GETDATE(), @WidthID, @WidthMin, @WidthMax, @WidthNom, @Write, @Processed;
                SELECT ID = (SELECT MAX(ID) FROM DataRequestsWidth WITH(NOLOCK))";

        public static IEnumerable<Width> GetAll()
        {
            MightyOrm<Width> db = new MightyOrm<Width>(Global.MightyConString, "Width", "ID");
            return db.AllWithParams("WHERE ID > 0");
        }

        public static Width GetAtID(int ID)
        {
            MightyOrm<Width> db = new MightyOrm<Width>(Global.MightyConString, "Width", "ID");
            return db.Single(ID);
        }

        public static int Insert(Width width)
        {
            width.ID = GetLowestAvailableID();
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Width SELECT @ID,@Label, @Nom, @Min, @Max", con))
                {
                    cmd.Parameters.AddWithValue("@ID", width.ID);
                    cmd.Parameters.AddWithValue("@Label", width.Label);
                    cmd.Parameters.AddWithValue("@Nom", width.Nominal);
                    cmd.Parameters.AddWithValue("@Min", width.Minimum);
                    cmd.Parameters.AddWithValue("@Max", width.Maximum);

                    cmd.ExecuteNonQuery();
                }
            }

            return width.ID;
        }

        public static void Save(Width width)
        {
            MightyOrm<Width> db = new MightyOrm<Width>(Global.MightyConString, "Width", "ID");
            db.Save(width);
        }

        public static bool DataRequestInsert(SqlConnection con, Width width, bool CommSettings = true, bool ZeroOut = false)
        {
            //if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 32", con);
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = new SqlCommand(DataRequestSql, con))
            {
                if (ZeroOut)
                {
                    cmd.Parameters.AddWithValue("@WidthID", width.ID);
                    cmd.Parameters.AddWithValue("@WidthMin", 0);
                    cmd.Parameters.AddWithValue("@WidthMax", 0);
                    cmd.Parameters.AddWithValue("@WidthNom", 0);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@WidthID", width.ID);
                    cmd.Parameters.AddWithValue("@WidthMin", width.Minimum);
                    cmd.Parameters.AddWithValue("@WidthMax", width.Maximum);
                    cmd.Parameters.AddWithValue("@WidthNom", width.Nominal);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!Raptor.MessageAckConfirm("DataRequestsWidth", Global.GetValue<int>(reader, "id")))
                    {
                        return false;
                    }
                }
            }

            //if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-32 where (datarequests & 32)=32", con);
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

                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 t1.Id+1 ret FROM Width t1 WHERE NOT EXISTS(SELECT * FROM Width t2 WHERE t2.Id = t1.Id + 1) ORDER BY t1.Id", con))
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