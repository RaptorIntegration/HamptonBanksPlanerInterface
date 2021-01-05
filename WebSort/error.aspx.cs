using System;

namespace WebSort
{
    public partial class error : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("websort.aspx");
        }
    }
}
