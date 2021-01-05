using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebSort.Model;

namespace WebSort
{
    public partial class Targets : BasePage
    {
        private static User CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Targets", User.Identity.Name);
            
            Label LabelScreenStatus = (Label)Master.FindControl("LabelScreenStatus");
            if (CurrentUser.Access == 0)
                LabelScreenStatus.Text = "READ ONLY";
            else if (CurrentUser.Access == 1)
                LabelScreenStatus.Text = "";
            else if (CurrentUser.Access == 2)
            {
                LabelScreenStatus.Text = "ACCESS DENIED";
                Response.Redirect("websort.aspx");
            }

            if (IsPostBack)
                return;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("Select TargetMode,trimlossfactor=round(100-trimlossfactor,2) from WEBSortSetup", connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            TextBoxtlf.Text = reader["trimlossfactor"].ToString();
            if (reader["TargetMode"].ToString() == "Shift")
            {
                RadioButtonListTargetMode.Items[0].Selected = true;
                PanelShift.Visible = true;
                PanelRun.Visible = false;
            }
            else
            {
                RadioButtonListTargetMode.Items[1].Selected = true;
                PanelShift.Visible = false;
                PanelRun.Visible = true;
            }

            reader.Close();

            connection.Close();
        }

        protected void TreeView1_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            if (e.Node.ChildNodes.Count == 0)
            {
                switch (e.Node.Depth)
                {
                    case 0:
                        FillRecipeCategories(e.Node);
                        break;
                        //case 1:
                        //FillReportNames(e.Node);
                        //break;
                }
            }
            connection.Close();
        }

        private void FillRecipeCategories(TreeNode node)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("Select distinct RecipeLabel,recipeid From Recipes order by RecipeLabel", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            DataSet Recipes = new DataSet();
            adapter.Fill(Recipes);

            if (Recipes.Tables.Count > 0)
            {
                foreach (DataRow row in Recipes.Tables[0].Rows)
                {
                    TreeNode newNode = new
                    TreeNode(row["RecipeLabel"].ToString(), row["recipeid"].ToString());

                    newNode.PopulateOnDemand = true;
                    newNode.SelectAction = TreeNodeSelectAction.Select;
                    node.ChildNodes.Add(newNode);
                }
            }
            connection.Close();
        }

        protected void RadioButtonListTargetMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string mode;
            if (RadioButtonListTargetMode.Items[0].Selected == true)
            {
                mode = "Shift";
                PanelShift.Visible = true;
                PanelRun.Visible = false;
            }
            else
            {
                mode = "Run";
                PanelShift.Visible = false;
                PanelRun.Visible = true;
            }
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();

            SqlCommand command = new SqlCommand("update WEBSortSetup set TargetMode='" + mode + "'", connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        protected void TextBoxtlf_TextChanged(object sender, EventArgs e)
        {
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand command = new SqlCommand("update WEBSortSetup set trimlossfactor=" + (100 - Convert.ToSingle(TextBoxtlf.Text)).ToString(), connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}