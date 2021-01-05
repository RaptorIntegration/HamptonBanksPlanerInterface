using Mighty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSort.Model
{
    public class AlarmDefault
    {
        public int AlarmID { get; set; }
        public bool Active { get; set; }
        public string Category { get; set; }
        public string Prefix { get; set; }
        public string InfomasterPrefix { get; set; }
        public string ColumnName { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public AlarmDefault()
        {
            EditsList = new List<Edit>();
        }

        public static IEnumerable<AlarmDefault> GetAll()
        {
            MightyOrm<AlarmDefault> db = new MightyOrm<AlarmDefault>(Global.MightyConString, "AlarmDefaults", "AlarmID");
            return db.All();
        }

        public AlarmDefault Save()
        {
            MightyOrm<AlarmDefault> db = new MightyOrm<AlarmDefault>(Global.MightyConString, "AlarmDefaults", "AlarmID");
            db.Update(this);

            return this;
        }
    }
}