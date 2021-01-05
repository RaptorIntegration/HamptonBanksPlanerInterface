using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace WebSort.Model
{
    public class SaveResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public List<Edit> ChangedList { get; set; }

        public string TableName { get; set; }

        public string TableKey => TableName switch
        {
            "AlarmSettings" => "AlarmID",
            "AlarmDefaults" => "AlarmID",
            "Bins" => "BinID",
            "GradeMatrix" => "PLCGradeID, RecipeID",
            "Grades" => "GradeID",
            "Length" => "LengthID",
            "Products" => "ProdID",
            "Recipes" => "RecipeID",
            "Sorts" => "SortID",
            "Thickness" => "ID",
            "Width" => "ID",
            _ => "",
        };

        public SaveResponse()
        {
            ChangedList = new List<Edit>();
        }

        public SaveResponse(string table)
        {
            TableName = table;
            ChangedList = new List<Edit>();
        }

        public void Bad(string bad)
        {
            Message = bad;
            Success = false;
        }

        public void Good(string good)
        {
            Message = good;
            Success = true;
        }

        public static string Serialize(SaveResponse response)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(response);
        }

        public void AddEdits(List<Edit> edits)
        {
            foreach (Edit edit in edits)
            {
                ChangedList.Add(edit);
                Audit(edit, TableKey, TableName);
            }
        }

        public void SimpleAudit(Audit audit)
        {
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Audit SELECT GETDATE(), @User, @KeyName, @TableName, @Key, @Col, @Val, @Prev, @Deleted, @Added", con))
                {
                    cmd.Parameters.AddWithValue("@User", Global.UserName);
                    cmd.Parameters.AddWithValue("@KeyName", string.IsNullOrEmpty(audit.KeyName) ? "" : audit.KeyName);
                    cmd.Parameters.AddWithValue("@Key", string.IsNullOrEmpty(audit.TableKey) ? "" : audit.TableKey);
                    cmd.Parameters.AddWithValue("@TableName", string.IsNullOrEmpty(audit.TableName) ? "" : audit.TableName);
                    cmd.Parameters.AddWithValue("@Col", string.IsNullOrEmpty(audit.Col) ? "" : audit.Col);
                    cmd.Parameters.AddWithValue("@Val", string.IsNullOrEmpty(audit.Val) ? "" : audit.Val);
                    cmd.Parameters.AddWithValue("@Prev", string.IsNullOrEmpty(audit.Prev) ? "" : audit.Prev);
                    cmd.Parameters.AddWithValue("@Deleted", audit.Deleted);
                    cmd.Parameters.AddWithValue("@Added", audit.Added);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Audit(Edit edit, string KeyName, string TableName)
        {
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
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
            }
        }
    }
}