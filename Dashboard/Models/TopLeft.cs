using System.Collections.Generic;

namespace DashboardCore.Models
{
    public class TopLeft
    {
        public int TargetVPH { get; set; }
        public List<Rate> Rates { get; set; }

        public TopLeft()
        {
            Rates = new List<Rate>();
        }
    }
}