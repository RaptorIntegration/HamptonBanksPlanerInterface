using Mighty;

using System.Collections.Generic;

namespace WebSort.Model
{
    public class AlarmSetting
    {
        [DatabasePrimaryKey]
        public int AlarmID { get; set; }

        public bool Active { get; set; }
        public short Priority { get; set; }
        public string AlarmText { get; set; }
        public string DisplayText { get; set; }
        public short Severity { get; set; }
        public bool Downtime { get; set; }
        public bool DataRequired { get; set; }
        public int Data { get; set; }
        public bool SorterEligible { get; set; }

        [DatabaseIgnore]
        public List<Edit> EditsList { get; set; }

        public AlarmSetting()
        {
            EditsList = new List<Edit>();
        }

        public static IEnumerable<AlarmSetting> GetAll()
        {
            MightyOrm<AlarmSetting> db = new MightyOrm<AlarmSetting>(Global.MightyConString, "AlarmSettings", "AlarmID");
            return db.All();
        }

        public AlarmSetting Save()
        {
            MightyOrm<AlarmSetting> db = new MightyOrm<AlarmSetting>(Global.MightyConString, "AlarmSettings", "AlarmID");
            db.Update(this);

            return this;
        }
    }
}