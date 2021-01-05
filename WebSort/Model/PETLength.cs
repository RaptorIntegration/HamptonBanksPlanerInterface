using Mighty;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebSort.Model
{
    public class PETLength
    {
        public int PETLengthID { get; set; }
        public string LengthLabel { get; set; }
        public int SawIndex { get; set; }
        public float LengthNominal { get; set; }
        public float PETPosition { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public PETLength()
        {
            EditsList = new List<Edit>();
        }

        /// <summary>
        /// @PETLengthID, @SawIndex, @LengthNominal, @PETPosition, @Write, @Processed
        /// </summary>
        public static string DataRequestSql = @"
            INSERT INTO [DataRequestsPETLength]
            SELECT
                GETDATE(),
                @PETLengthID,
                @SawIndex,
                @LengthNominal,
                @PETPosition,
                @Write,
                @Processed;
            SELECT ID = (SELECT MAX(ID) FROM DataRequestsPETLength WITH(NOLOCK))";

        public static IEnumerable<PETLength> GetAll()
        {
            MightyOrm<PETLength> db = new MightyOrm<PETLength>(Global.MightyConString, "PETLengths", "PETLengthID");
            return db.AllWithParams("WHERE PETLengthID > 0");
        }

        public static void Save(PETLength length)
        {
            MightyOrm<PETLength> db = new MightyOrm<PETLength>(Global.MightyConString, "PETLengths", "PETLengthID");
            db.Save(length);
        }

        public static bool DataRequestInsert(SqlConnection con, PETLength length, bool ZeroOut)
        {
            using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2048", con))
                cmd.ExecuteNonQuery();

            using (SqlCommand cmd = new SqlCommand(DataRequestSql, con))
            {
                if (ZeroOut)
                {
                    cmd.Parameters.AddWithValue("@PETLengthID", length.PETLengthID);
                    cmd.Parameters.AddWithValue("@SawIndex", 0);
                    cmd.Parameters.AddWithValue("@LengthNominal", 0);
                    cmd.Parameters.AddWithValue("@PETPosition", 0);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@PETLengthID", length.PETLengthID);
                    cmd.Parameters.AddWithValue("@SawIndex", length.SawIndex);
                    cmd.Parameters.AddWithValue("@LengthNominal", length.LengthNominal);
                    cmd.Parameters.AddWithValue("@PETPosition", length.PETPosition);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!Raptor.MessageAckConfirm("DataRequestsPETLength", Global.GetValue<int>(reader, "id")))
                        {
                            return false;
                        }
                    }
                }
            }

            using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2048 where (datarequests & 2048)=2048", con))
                cmd.ExecuteNonQuery();

            return true;
        }
    }
}