using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

using DashboardCore.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DashboardCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
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
                string CS = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
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
                string CS = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
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
        public IActionResult GetTableData()
        {
            try
            {
                string CS = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
                List<Bins> bins = new List<Bins>();

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    using SqlCommand cmd = new SqlCommand("SELECT BinID, BinLabel, BinStatus.Color,BinPercent FROM Bins, BinStatus WHERE Bins.BinStatus = BinStatus.BinStatus ORDER BY BinID", con);
                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            bins.Add(new Bins
                            {
                                BinID = GetValue<int>(reader, "BinID"),
                                Label = reader["BinLabel"].ToString(),
                                Status = reader["Color"].ToString(),
                                Percent = Convert.ToInt32(reader["BinPercent"].ToString())
                            });
                        }
                    }
                }
                return Ok(bins);
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
                string CS = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
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
        public IActionResult GetProductMix()
        {
            try
            {
                string CS = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
                List<ProductMix> mix = new List<ProductMix>();
                long PieceCount = 0;

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT CurrentPieces FROM CurrentState", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                PieceCount = GetValue<long>(reader, "CurrentPieces");
                            }
                        }
                    }

                    const string sql = @"
                        SELECT distinct Products.ProdLabel + ' ' + Grades.GradeLabel As 'Label', Total
                        FROM (SELECT ProdID, SUM(BoardCount) as Total
                            FROM ProductionBoards
                            WHERE Sorted = 1
                            GROUP BY ProdID) a, Products, Grades WITH(NOLOCK)
                        WHERE Products.ProdID = a.ProdID
                        and Grades.GradeID = Products.GradeID
                        ORDER BY Total DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                long total = GetValue<int>(reader, "Total");
                                float percent = (total == 0 || PieceCount == 0) ? (float)0 : ((float)total / (float)PieceCount);
                                mix.Add(new ProductMix
                                {
                                    Label = GetValue<string>(reader, "Label"),
                                    Percent = percent
                                });
                            }
                        }
                    }
                }
                return Ok(mix);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Error();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}