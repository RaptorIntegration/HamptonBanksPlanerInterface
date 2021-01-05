using System;
using System.Data.SqlClient;
using System.Web.Security;

namespace WebSort
{
    public partial class Login : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                // check to see if login is valid
                using (SqlCommand cmd = new SqlCommand("SELECT UserID FROM Users WHERE username=@Username and password=@Password", con))
                {
                    cmd.Parameters.AddWithValue("@Username", DropDownListUserName.Text);
                    cmd.Parameters.AddWithValue("@Password", TextBoxPassword.Text);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {                                
                                FormsAuthentication.RedirectFromLoginPage(DropDownListUserName.Text, true);
                            }
                        }
                        else
                        {
                            LabelCredentialsWarning.Visible = true;
                        }
                    }
                }                   
            }
        }
    }
}