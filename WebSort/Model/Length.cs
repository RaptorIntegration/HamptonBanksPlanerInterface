using Mighty;

using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebSort.Model
{
    public class Length
    {
        public int LengthID { get; set; }
        public string LengthLabel { get; set; }
        public float LengthNominal { get; set; }
        public float LengthMin { get; set; }
        public float LengthMax { get; set; }
        public bool PETFlag { get; set; }
        public int PETLengthID { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public Length()
        {
            EditsList = new List<Edit>();
        }

        /// <summary>
        /// @LengthID, @LengthMin, @LengthMax, @LengthNom, @Write, @Processed
        /// </summary>
        public static string DataRequestSql = @"
            INSERT INTO [DataRequestsLength]
            SELECT
                GETDATE(),
                @LengthID,
                @LengthMin,
                @LengthMax,
                @LengthNom,
                @Write,
                @Processed;
            SELECT ID = (SELECT MAX(ID) FROM DataRequestsLength WITH(NOLOCK))";

        public static IEnumerable<Length> GetAll()
        {
            MightyOrm<Length> db = new MightyOrm<Length>(Global.MightyConString, "Lengths", "LengthID");
            return db.AllWithParams("WHERE LengthID > 0 AND PetFlag = 0");
        }

        public static Length GetAtID(int ID)
        {
            MightyOrm<Length> db = new MightyOrm<Length>(Global.MightyConString, "Lengths", "LengthID");
            return db.Single(ID);
        }

        public static void Save(Length length)
        {
            MightyOrm<Length> db = new MightyOrm<Length>(Global.MightyConString, "Lengths", "LengthID");
            db.Save(length);
        }

        public static bool DataRequestInsert(SqlConnection con, Length length, bool CommSettings = true, bool ZeroOut = false)
        {
            if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 64", con);
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = new SqlCommand(DataRequestSql, con))
            {
                if (ZeroOut)
                {
                    cmd.Parameters.AddWithValue("@LengthID", length.LengthID);
                    cmd.Parameters.AddWithValue("@LengthMin", 0);
                    cmd.Parameters.AddWithValue("@LengthMax", 0);
                    cmd.Parameters.AddWithValue("@LengthNom", 0);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ProductID", length.LengthID);
                    cmd.Parameters.AddWithValue("@LengthMin", length.LengthMin);
                    cmd.Parameters.AddWithValue("@LengthMax", length.LengthMax);
                    cmd.Parameters.AddWithValue("@LengthNom", length.LengthNominal);
                    cmd.Parameters.AddWithValue("@Write", 1);
                    cmd.Parameters.AddWithValue("@Processed", 0);
                }

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!Raptor.MessageAckConfirm("DataRequestsLength", Global.GetValue<int>(reader, "id")))
                    {
                        return false;
                    }
                }
            }

            if (CommSettings)
            {
                using SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-64 where (datarequests & 64)=64", con);
                cmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}