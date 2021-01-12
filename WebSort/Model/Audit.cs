using Mighty;

using System;
using System.Collections.Generic;
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
    }
}