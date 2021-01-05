using Mighty;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebSort.Model
{
    [Serializable]
    public class AlarmGeneralSetting
    {
        public float? DisplayTime { get; set; }

        public float? BlankTime { get; set; }

        public string DisplayIPAddress { get; set; }

        public string DisplayPortNumber { get; set; }

        public bool? MultilineDefaults { get; set; }

        public short? DisplayType { get; set; }

        public short? DisplayCount { get; set; }

        public float? DisplayTimeVorneScroll { get; set; }

        public float? DisplayTimeVorneStatic { get; set; }

        public float? DisplayTime1 { get; set; }

        public float? BlankTime1 { get; set; }

        public string DisplayPortNumber1 { get; set; }

        public float? DisplayTime2 { get; set; }

        public float? BlankTime2 { get; set; }

        public string DisplayPortNumber2 { get; set; }

        private DateTime AccidentDate { get; set; }
        public string AccidentDateString => AccidentDate.ToString("MMM dd hh:mm tt");

        public string DisplayPortNumber3 { get; set; }

        public const string UpdateSql = @"
        UPDATE [dbo].[AlarmGeneralSettings]
           SET [DisplayTime] = @DisplayTime
              ,[DisplayTime1] = @DisplayTime1
              ,[DisplayTime2] = @DisplayTime2
              ,[BlankTime] = @BlankTime
              ,[BlankTime1] = @BlankTime1
              ,[BlankTime2] = @BlankTime2
              ,[DisplayIPAddress] = @DisplayIPAddress
              ,[DisplayPortNumber] = @DisplayPortNumber
              ,[DisplayPortNumber1] = @DisplayPortNumber1
              ,[DisplayPortNumber2] = @DisplayPortNumber2
              ,[DisplayPortNumber3] = @DisplayPortNumber3
              ,[MultilineDefaults] = @MultilineDefaults
              ,[DisplayType] = @DisplayType
              ,[DisplayCount] = @DisplayCount
              ,[DisplayTimeVorneScroll] = @DisplayTimeVorneScroll
              ,[DisplayTimeVorneStatic] = @DisplayTimeVorneStatic";

        public static IEnumerable<AlarmGeneralSetting> GetAll()
        {
            MightyOrm<AlarmGeneralSetting> db = new MightyOrm<AlarmGeneralSetting>(Global.MightyConString, "AlarmGeneralSettings");
            return db.Query("SELECT * FROM AlarmGeneralSettings");
        }

        public AlarmGeneralSetting Save()
        {
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(UpdateSql, con))
                {
                    cmd.Parameters.AddWithValue("@DisplayTime", this.DisplayTime);
                    cmd.Parameters.AddWithValue("@DisplayTime1", this.DisplayTime1);
                    cmd.Parameters.AddWithValue("@DisplayTime2", this.DisplayTime2);

                    cmd.Parameters.AddWithValue("@BlankTime", this.BlankTime);
                    cmd.Parameters.AddWithValue("@BlankTime1", this.BlankTime1);
                    cmd.Parameters.AddWithValue("@BlankTime2", this.BlankTime2);

                    cmd.Parameters.AddWithValue("@DisplayPortNumber", this.DisplayPortNumber);
                    cmd.Parameters.AddWithValue("@DisplayPortNumber1", this.DisplayPortNumber1);
                    cmd.Parameters.AddWithValue("@DisplayPortNumber2", this.DisplayPortNumber2);
                    cmd.Parameters.AddWithValue("@DisplayPortNumber3", this.DisplayPortNumber3);

                    cmd.Parameters.AddWithValue("@DisplayIPAddress", this.DisplayIPAddress);

                    cmd.Parameters.AddWithValue("@MultilineDefaults", this.MultilineDefaults);
                    cmd.Parameters.AddWithValue("@DisplayType", this.DisplayType);
                    cmd.Parameters.AddWithValue("@DisplayCount", this.DisplayCount);
                    cmd.Parameters.AddWithValue("@DisplayTimeVorneScroll", this.DisplayTimeVorneScroll);
                    cmd.Parameters.AddWithValue("@DisplayTimeVorneStatic", this.DisplayTimeVorneStatic);

                    cmd.ExecuteNonQuery();
                }
            }

            return this;
        }
    }
}