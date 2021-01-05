using Mighty;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class Boards : BasePage
    {
        private Thread simThread = null;
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Boards", User.Identity.Name);
            
            Label LabelScreenStatus = (Label)Master.FindControl("LabelScreenStatus");
            if (CurrentUser.Access == 0)
            {
                LabelScreenStatus.Text = "READ ONLY";
                LabelScreenStatus.Attributes.Add("class", "severity-2");
            }
            else if (CurrentUser.Access == 1)
                LabelScreenStatus.Text = "";
            else if (CurrentUser.Access == 2)
            {
                LabelScreenStatus.Text = "ACCESS DENIED";
                LabelScreenStatus.Attributes.Add("class", "severity-2");
                Response.Redirect("websort.aspx");
            }

            if (IsPostBack)
                return;

            int clpieces, clvolume, cllugfill/*,cluptime*/;

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                SqlCommand command = new SqlCommand("Select TargetMode from WEBSortSetup", con);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader["TargetMode"].ToString() == "Shift")
                {
                    SqlCommand command1 = new SqlCommand("Select * from shifts where shiftindex=(select max(shiftindex) from shifts)", con);
                    SqlDataReader reader1 = command1.ExecuteReader();
                    reader1.Read();
                    clpieces = int.Parse(reader1["targetpiecesperhour"].ToString());
                    clvolume = int.Parse(reader1["targetvolumeperhour"].ToString());
                    cllugfill = int.Parse(reader1["targetlugfill"].ToString());
                    reader1.Close();
                }
                else
                {
                    SqlCommand command1 = new SqlCommand("Select * from runs where runindex=(select max(runindex) from runs)", con);
                    SqlDataReader reader1 = command1.ExecuteReader();
                    reader1.Read();
                    clpieces = int.Parse(reader1["targetpiecesperhour"].ToString());
                    clvolume = int.Parse(reader1["targetvolumeperhour"].ToString());
                    cllugfill = int.Parse(reader1["targetlugfill"].ToString());
                    reader1.Close();
                }

                reader.Close();
            }
        }

        [WebMethod]
        public static string GetBoards()
        {
            JavaScriptSerializer S = new JavaScriptSerializer();

            return S.Serialize(Board.GetAll());
        }

        [WebMethod]
        public static string GetProdStats()
        {
            JavaScriptSerializer S = new JavaScriptSerializer();
            MightyOrm mighty = new MightyOrm(Global.MightyConString);
            try
            {
                float TrimLossFactor = mighty.Query("SELECT TrimLossFactor FROM WEBSortSetup WHERE TrimLossFactor > 0")
                    .First()
                    .TrimLossFactor / 100.0f;

                ProdStats Data = new ProdStats();
                Data.GetData(TrimLossFactor);

                return S.Serialize(Data);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                throw ex;
            }
        }

        [WebMethod]
        public static string GetRejects()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(BoardReject.GetAll());
        }

        [WebMethod]
        public static string SaveRejects(BoardReject rejects)
        {
            SaveResponse response = new SaveResponse();
            try
            {
                BoardReject.Save(rejects);
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error saving Rejects");
                return SaveResponse.Serialize(response);
            }

            response.Good("Reject saved");
            return SaveResponse.Serialize(response);
        }

        #region Charts

        [WebMethod]
        public static string GetCharts()
        {
            BoardChart Data = new BoardChart();
            JavaScriptSerializer S = new JavaScriptSerializer();

            string ShiftIndex = "";

            Raptor cs = new Raptor();
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT CONVERT(decimal(18,2),VolumePerHour) as VPH,CONVERT(decimal(18,2),PiecesPerHour) as PPH,CONVERT(decimal(18,2),LugFill) as LF, ShiftIndex FROM TargetSummary order by TimeSegment", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Data.VPH.Add((decimal)reader["VPH"]);
                            Data.PPH.Add((decimal)reader["PPH"]);
                            Data.LF.Add((decimal)reader["LF"]);
                            ShiftIndex = reader["ShiftIndex"].ToString();
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand("Select * from shifts where shiftindex=" + ShiftIndex, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Data.TVPH = Math.Round(Convert.ToDecimal(reader["TargetVolumePerHour"].ToString()), 2);
                            Data.TPPH = Math.Round(Convert.ToDecimal(reader["TargetPiecesPerHour"].ToString()), 2);
                            Data.TLF = Math.Round(Convert.ToDecimal(reader["TargetLugFill"].ToString()), 2);
                        }
                    }
                }
            } // End conenction

            return S.Serialize(Data);
        }

        #endregion Charts
    }
}