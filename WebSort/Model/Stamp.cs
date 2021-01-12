using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebSort.Model
{
    public class Stamp
    {
        public Stamp()
        {
        }

        public Stamp(int id, string description, bool selected)
        {
            ID = id;
            Description = description;
            Selected = selected;
        }

        public int ID { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }

        public static List<Stamp> GetStamps()
        {
            List<Stamp> stamps = new List<Stamp>
            {
                new Stamp()
                {
                    ID = 31,
                    Description = "Default",
                    Selected = false
                }
            };

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using SqlCommand cmd = new SqlCommand("SELECT StampID, REPLACE(StampDescription, 'Stamp ', '') AS StampDescription FROM Stamps WHERE StampDescription <> 'Spare'", con);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        stamps.Add(new Stamp
                        (
                            Global.GetValue<int>(reader, "StampID"),
                            Global.GetValue<string>(reader, "StampDescription"),
                            false
                        ));
                    }
                }
            }

            return stamps;
        }

        public static uint GetStampsBitMap(List<Stamp> stamps)
        {
            uint ret = 0;
            foreach (Stamp stamp in stamps)
            {
                if (stamp.Selected)
                {
                    ret |= 1u << stamp.ID;
                }
            }
            return ret;
        }

        public static List<Stamp> GetSelectedStamps(uint GradeStamps, List<Stamp> stamps)
        {
            List<Stamp> Clone = stamps.ConvertAll(s => new Stamp(s.ID, s.Description, s.Selected));

            foreach (Stamp stamp in Clone)
            {
                stamp.Selected = (GradeStamps & (uint)Math.Pow(2, stamp.ID)) == (uint)Math.Pow(2, stamp.ID);
            }

            return Clone;
        }

        public static List<Edit> GetChangesFromBitmap(uint a, uint b, int key)
        {
            List<Stamp> temp = GetStamps();
            List<Edit> ret = new List<Edit>();

            List<Stamp> SelectedA = GetSelectedStamps(a, temp);
            List<Stamp> SelectedB = GetSelectedStamps(b, temp);

            for (int i = 0; i < temp.Count; i++)
            {
                if (SelectedA[i].Selected != SelectedB[i].Selected)
                {
                    ret.Add(new Edit()
                    {
                        EditedCol = $"Stamp {SelectedA[i].Description}",
                        EditedVal = SelectedA[i].Selected ? "Selected" : "Un-Selected",
                        Previous = SelectedB[i].Selected ? "Selected" : "Un-Selected",
                        Key = key
                    });
                }
            }

            return ret;
        }
    }
}