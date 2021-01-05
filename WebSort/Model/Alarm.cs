﻿using Mighty;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSort.Model
{
    public class Alarm
    {
        [DatabasePrimaryKey]
        public int ShiftIndex { get; set; }

        [DatabasePrimaryKey]
        public int RunIndex { get; set; }

        [DatabasePrimaryKey]
        public int AlarmID { get; set; }

        [DatabasePrimaryKey]
        public DateTime StartTime { get; set; }

        public string StartTimeString => StartTime.ToString("MMM dd hh:mm tt");

        public DateTime StopTime { get; set; }
        public string StopTimeString => StopTime.ToString("MMM dd hh:mm tt");
        public bool DownTime { get; set; }
        public string Duration { get; set; }
        public int Data { get; set; }

        public const string CurrentAlarmsSQL = @"
            SELECT alarms.alarmid,
                   starttime
            FROM   alarms
            WHERE  stoptime IS NULL
            UNION
            SELECT alarmsprevious.alarmid,
                   starttime
            FROM   alarmsprevious
            WHERE  stoptime IS NULL
            ORDER  BY starttime DESC,
                      alarmid ASC";

        public static IEnumerable<Alarm> GetAll()
        {
            MightyOrm<Alarm> db = new MightyOrm<Alarm>(Global.MightyConString, "Alarms", "AlarmID");
            return db.All();
        }

        public static IEnumerable<Alarm> GetAllHistory()
        {
            MightyOrm<Alarm> db = new MightyOrm<Alarm>(Global.MightyConString, "Alarms", "AlarmID");
            return db.AllWithParams("WHERE StopTime IS NOT NULL");
        }

        public static IEnumerable<Alarm> GetAllCurrent()
        {
            MightyOrm<Alarm> db = new MightyOrm<Alarm>(Global.MightyConString, "Alarms", "AlarmID");
            return db.Query(CurrentAlarmsSQL);
        }

        public class DisplayLog
        {
            public int ID { get; set; }
            public DateTime TimeStamp { get; set; }
            public string TimeString => TimeStamp.ToString("MMM dd hh:mm:ss tt");
            public string Text { get; set; }

            public static IEnumerable<DisplayLog> GetTop50()
            {
                MightyOrm<DisplayLog> db = new MightyOrm<DisplayLog>(Global.MightyConString, "RaptorDisplayLog", "ID");
                IEnumerable<DisplayLog> ret = db.Query("SELECT TOP 50 id, Timestamp, Text FROM [RaptorDisplayLog] ORDER BY id DESC");
                return ret;
            }
        }
    }
}