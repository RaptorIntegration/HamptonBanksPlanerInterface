namespace WebSort
{
    public abstract class BasePage : System.Web.UI.Page
    {
        public static string GetVersion()
        {
            return Global.Version;
        }
    }
}