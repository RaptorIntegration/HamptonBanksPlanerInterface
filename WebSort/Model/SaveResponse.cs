using System.Collections.Generic;
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
                Audit.InsertAudit(edit, TableKey, TableName);
            }
        }
    }
}