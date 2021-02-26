using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

using Dashboard.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private static string CS { get; set; }
        private record AutoOff(string Label, int Count);

        private const string AutoOffSql = @"
            SELECT Sorts.SortLabel, SUM(Sorts.OrderCount * Sorts.SortSize) - Bins.BinCount AS P
            FROM Sorts, Bins
            WHERE Sorts.SortID = Bins.SortID
                AND Sorts.OrderCount > 0
                AND Bins.BinStatus = 1
                And RecipeID = (select RecipeID from Recipes where Online=1)
            GROUP BY Sorts.SortLabel, Bins.BinCount";

        private const string HandPulledSql = @"
            SELECT DISTINCT BinLabel AS Label
            FROM Bins
            WHERE BinID > 38
                AND (BinLabel NOT LIKE '%4%'
                AND BinLabel NOT LIKE '%6%'
                AND BinLabel NOT LIKE '%Economy 8%')";

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            CS = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
        }

        public IActionResult Index()
        {
            return View();
        }

        public static T GetValue<T>(SqlDataReader reader, string colName)
        {
            var value = reader[colName];
            return value == DBNull.Value ? default(T) : (T)Convert.ChangeType(value, typeof(T));
        }

        [HttpPost]
        public IActionResult GetSmallCard()
        {
            try
            {
                SmallCard data = new SmallCard();

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT CurrentPieces, CurrentMessage, CurrentAlarmID, (Select Severity from alarmsettings where alarmid=(SELECT CurrentAlarmID FROM CurrentState)) AS Severity, TrimLoss, CurrentShiftLugfill FROM CurrentState", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.AlarmID = GetValue<int>(reader, "CurrentAlarmID");
                                data.Alarm = GetValue<string>(reader, "CurrentMessage");
                                data.TrimLoss = GetValue<decimal>(reader, "TrimLoss").ToString("F1");
                                data.LugFill = GetValue<decimal>(reader, "CurrentShiftLugfill").ToString("F0");

                                data.Severity = string.IsNullOrEmpty(GetValue<string>(reader, "Severity")) ? "0" : GetValue<string>(reader, "Severity");
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT TrimLossExcess FROM WEBSortSetup", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.TrimLossExcess = GetValue<float>(reader, "TrimLossExcess") < 1 ? (float)12 : GetValue<float>(reader, "TrimLossExcess");
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT TargetLugFill FROM Shifts WHERE ShiftIndex = (SELECT DISTINCT ShiftIndex FROM TargetSummary)", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.LugFillTarget = GetValue<float>(reader, "TargetLugFill") < 1 ? (float)12 : GetValue<float>(reader, "TargetLugFill");
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("select CurrentVolumePerHour, CurrentPiecesPerHour from CurrentState", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.CurrentPPH = GetValue<int>(reader, "CurrentPiecesPerHour");
                                data.CurrentVPH = GetValue<int>(reader, "CurrentVolumePerHour");
                            }
                        }
                    }
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Error();
            }
        }

        [HttpPost]
        public IActionResult GetTopLeft()
        {
            try
            {
                TopLeft data = new TopLeft();

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT TimeSegment, VolumePerHour FROM TargetSummary", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.Rates.Add(new Rate
                                {
                                    TimeSegment = GetValue<int>(reader, "TimeSegment"),
                                    VPH = GetValue<float>(reader, "VolumePerHour")
                                });
                            }
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT TargetVolumePerHour FROM Shifts WHERE ShiftIndex = (SELECT DISTINCT ShiftIndex FROM TargetSummary)", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.TargetVPH = GetValue<int>(reader, "TargetVolumePerHour");
                            }
                        }
                    }
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Error();
            }
        }

        [HttpPost]
        public IActionResult GetPie()
        {
            const string sql = @"
                SELECT
                    A.BinStatusLabel,
                    C,
                    BinStatus.Color
                FROM (
                    SELECT
                        BinStatus.BinStatusLabel,
                        COUNT(Bins.BinStatus) as C
                    FROM BinStatus
                    LEFT JOIN Bins ON Bins.BinStatus = BinStatus.BinStatus
                    GROUP BY BinStatus.BinStatusLabel
                ) A, BinStatus
                WHERE BinStatus.BinStatusLabel = A.BinStatusLabel
                ORDER BY BinStatus";
            try
            {
                List<Pie> pies = new List<Pie>();

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    using SqlCommand cmd = new SqlCommand(sql, con);
                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            pies.Add(new Pie
                            {
                                BinStatusLabel = GetValue<string>(reader, "BinStatusLabel"),
                                Count = GetValue<int>(reader, "C"),
                                Colour = GetValue<string>(reader, "Color")
                            });
                        }
                    }
                }
                return Ok(pies);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get data for Pie Chart", ex);
                return Error();
            }
        }

        [HttpPost]
        public JsonResult GetAutoOff()
        {
            List<AutoOff> res = new List<AutoOff>();
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();

                using SqlCommand cmd = new SqlCommand(AutoOffSql, con);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        res.Add(new AutoOff(reader["SortLabel"].ToString(), GetValue<int>(reader, "P")));
                    }
                }
            }
            return new JsonResult(res);
        }

        [HttpPost]
        public JsonResult GetHandPulled()
        {
            List<string> res = new List<string>();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();

                using SqlCommand cmd = new SqlCommand(HandPulledSql, con);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        res.Add(reader["Label"].ToString());
                    }
                }
            }
            return new JsonResult(res);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}