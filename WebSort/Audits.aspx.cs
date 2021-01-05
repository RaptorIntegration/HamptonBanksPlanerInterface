using System;
using System.Web.Script.Serialization;
using System.Web.Services;
using WebSort.Model;

namespace WebSort
{
    public partial class Audits : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static string GetAudit()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(Audit.GetAll());
        }
    }
}