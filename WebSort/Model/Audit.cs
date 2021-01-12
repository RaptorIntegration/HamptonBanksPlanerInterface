using Mighty;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace WebSort.Model
{
    public class Audit
    {
        public int ID { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TimeStampString => TimeStamp.ToString("G");
        public string UserName { get; set; }
        public string TableName { get; set; }
        public string KeyName { get; set; }
        public string TableKey { get; set; }
        public string Col { get; set; }
        public string Val { get; set; }
        public string Prev { get; set; }
        public bool Deleted { get; set; }
        public bool Added { get; set; }

        public Audit()
        {
        }

        public Audit(string TableKey, string KeyName, string TableName, bool deleted, bool added)
        {
            Deleted = deleted;
            Added = added;
            this.TableName = TableName;
            this.TableKey = TableKey;
            this.KeyName = KeyName;
        }

        public static IEnumerable<Audit> GetAll()
        {
            MightyOrm<Audit> db = new MightyOrm<Audit>(Global.MightyConString, "Audit", "ID");
            return db.All().OrderByDescending(o => o.TimeStamp).Take(100);
        }

        public void InsertSimpleAudit()
        {
            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

            using (SqlCommand cmd = new SqlCommand("INSERT INTO Audit SELECT GETDATE(), @User, @KeyName, @TableName, @Key, @Col, @Val, @Prev, @Deleted, @Added", con))
            {
                cmd.Parameters.AddWithValue("@User", Global.UserName);
                cmd.Parameters.AddWithValue("@KeyName", string.IsNullOrEmpty(KeyName) ? "" : KeyName);
                cmd.Parameters.AddWithValue("@Key", string.IsNullOrEmpty(TableKey) ? "" : TableKey);
                cmd.Parameters.AddWithValue("@TableName", string.IsNullOrEmpty(TableName) ? "" : TableName);
                cmd.Parameters.AddWithValue("@Col", string.IsNullOrEmpty(Col) ? "" : Col);
                cmd.Parameters.AddWithValue("@Val", string.IsNullOrEmpty(Val) ? "" : Val);
                cmd.Parameters.AddWithValue("@Prev", string.IsNullOrEmpty(Prev) ? "" : Prev);
                cmd.Parameters.AddWithValue("@Deleted", Deleted);
                cmd.Parameters.AddWithValue("@Added", Added);

                cmd.ExecuteNonQuery();
            }

            DbCleanup(con);
        }

        private static void DbCleanup(SqlConnection con)
        {
            using SqlCommand cmd = new SqlCommand("DELETE FROM Audit WHERE ID < (SELECT TOP 1000 max(ID) - 1000 FROM Audit)", con);
            cmd.ExecuteNonQuery();
        }

        public static void InsertAudit(Edit edit, string KeyName, string TableName)
        {
            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

            using (SqlCommand cmd = new SqlCommand("INSERT INTO Audit SELECT GETDATE(), @User, @KeyName, @TableName, @Key, @Col, @Val, @Prev, @Deleted, @Added", con))
            {
                cmd.Parameters.AddWithValue("@User", Global.UserName);
                cmd.Parameters.AddWithValue("@KeyName", string.IsNullOrEmpty(KeyName) ? "" : KeyName);
                cmd.Parameters.AddWithValue("@Key", string.IsNullOrEmpty(edit.Key.ToString()) ? "" : edit.Key.ToString());
                cmd.Parameters.AddWithValue("@TableName", string.IsNullOrEmpty(TableName) ? "" : TableName);
                cmd.Parameters.AddWithValue("@Col", string.IsNullOrEmpty(edit.EditedCol) ? "" : edit.EditedCol);
                cmd.Parameters.AddWithValue("@Val", string.IsNullOrEmpty(edit.EditedVal) ? "" : edit.EditedVal);
                cmd.Parameters.AddWithValue("@Prev", string.IsNullOrEmpty(edit.Previous) ? "" : edit.Previous);
                cmd.Parameters.AddWithValue("@Deleted", false);
                cmd.Parameters.AddWithValue("@Added", false);

                cmd.ExecuteNonQuery();
            }

            DbCleanup(con);
        }
    }
}