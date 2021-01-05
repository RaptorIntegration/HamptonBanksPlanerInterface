using System;
using System.Web.Script.Serialization;
using System.Web.Services;
using WebSort.Model;

namespace WebSort
{
    public partial class ErrorLogs : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.Name != "Raptor")
            {
                Response.Redirect("websort.aspx");
            }
        }

        [WebMethod]
        public static string GetLogs()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(ErrorLog.GetAll());
        }

        [WebMethod]
        public static string Flush()
        {
            SaveResponse response = new SaveResponse();

            try
            {
                ErrorLog.Flush();
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                response.Bad("Error Flushing");
                return SaveResponse.Serialize(response);
            }

            response.Good("Flushed!");
            return SaveResponse.Serialize(response);
        }
    }
}