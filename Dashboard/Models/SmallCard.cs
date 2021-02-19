namespace Dashboard.Models
{
    public class SmallCard
    {
        public int AlarmID { get; set; }
        public string Alarm { get; set; }
        public string Severity { get; set; }
        public int Reman { get; set; }
        public float RemanExcess { get; set; }
        public int SlashCount { get; set; }
        public float SlashExcess { get; set; }
        public string TrimLoss { get; set; }
        public float TrimLossExcess { get; set; }
        public string LugFill { get; set; }
        public float LugFillTarget { get; set; }
        public int CurrentPPH { get; set; }
        public int CurrentVPH { get; set; }
    }
}