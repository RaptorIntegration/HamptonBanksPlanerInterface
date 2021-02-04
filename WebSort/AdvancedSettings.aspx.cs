using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

using WebSort.Model;

namespace WebSort
{
    public partial class AdvancedSettings : BasePage
    {
        private static User CurrentUser;
        public string IPAddress;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = Global.GetSecurity("Advanced Settings", User.Identity.Name);

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
            if (CurrentUser.Access != 1)
            {
                RadioButtonOnline.Enabled = false;
                RadioButtonOffline.Enabled = false;
                ButtonSendBins.Enabled = false;
                ButtonSendProducts.Enabled = false;
                ButtonSendDrives.Enabled = false;
                ButtonSendTiming.Enabled = false;
                ButtonReadBins.Enabled = false;
                ButtonReadProducts.Enabled = false;
                ButtonReadDrives.Enabled = false;
                ButtonReadTiming.Enabled = false;
                ButtonBackup.Enabled = false;
                ButtonReplace.Enabled = false;
            }
            else
            {
                RadioButtonOnline.Enabled = true;
                RadioButtonOffline.Enabled = true;
                ButtonSendBins.Enabled = true;
                ButtonSendProducts.Enabled = true;
                ButtonSendDrives.Enabled = true;
                ButtonSendTiming.Enabled = true;
                ButtonReadBins.Enabled = true;
                ButtonReadProducts.Enabled = true;
                ButtonReadDrives.Enabled = true;
                ButtonReadTiming.Enabled = true;
                ButtonBackup.Enabled = true;
                ButtonReplace.Enabled = true;
            }

            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from WEBSortSetup", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                IPAddress = reader["ServerIPAddress"].ToString();
                if (reader["OnlineSetup"].ToString() == "1")
                {
                    RadioButtonOnline.Checked = true;
                    if (CurrentUser.Access == 1)
                    {
                        ButtonSendBins.Enabled = true;
                        ButtonSendProducts.Enabled = true;
                        ButtonSendDrives.Enabled = true;
                        ButtonSendTiming.Enabled = true;
                        ButtonReadBins.Enabled = true;
                        ButtonReadProducts.Enabled = true;
                        ButtonReadDrives.Enabled = true;
                        ButtonReadTiming.Enabled = true;
                    }
                }
                else
                {
                    RadioButtonOffline.Checked = true;
                    ButtonSendBins.Enabled = false;
                    ButtonSendProducts.Enabled = false;
                    ButtonSendDrives.Enabled = false;
                    ButtonSendTiming.Enabled = false;
                    ButtonReadBins.Enabled = false;
                    ButtonReadProducts.Enabled = false;
                    ButtonReadDrives.Enabled = false;
                    ButtonReadTiming.Enabled = false;
                }
            }
            reader.Close();
            connection.Close();

            if (!IsPostBack)
                LabelBackup.Text = "";
        }

        protected void RadioButtonOnline_CheckedChanged(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            if (RadioButtonOnline.Checked == true)
            {
                Session["OnlineSetup"] = 1;
            }
            else
            {
                Session["OnlineSetup"] = 0;
            }
            SqlCommand cmd = new SqlCommand("update WEBSortSetup set OnlineSetup=" + Session["OnlineSetup"].ToString(), connection);
            cmd.ExecuteNonQuery();
            connection.Close(); ;
        }

        protected void RadioButtonOffline_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonOffline.Checked == true)
            {
                Session["OnlineSetup"] = 0;
            }
            else
            {
                Session["OnlineSetup"] = 1;
            }
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand cmd = new SqlCommand("update WEBSortSetup set OnlineSetup=" + Session["OnlineSetup"].ToString(), connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        protected void ButtonSendBins_Click(object sender, EventArgs e)
        {
            string sqltext;
            int StampsPLC, SpraysPLC;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            LabelTimeout.Visible = false;
            //determine existing product and length map per bin
            uint ProductMap0, ProductMap1, ProductMap2, ProductMap3, ProductMap4, ProductMap5, LengthMap;
            uint ProductMap0c, ProductMap1c, ProductMap2c, LengthMapc;

            //send bins
            try
            {
                SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1", connection);
                cmd01.ExecuteNonQuery();
                SqlCommand cmdb = new SqlCommand("select * from bins with(NOLOCK)", connection);
                SqlDataReader readerb = cmdb.ExecuteReader();
                while (readerb.Read())
                {
                    ProductMap0 = 0;
                    ProductMap1 = 0;
                    ProductMap2 = 0;
                    ProductMap3 = 0;
                    ProductMap4 = 0;
                    ProductMap5 = 0;
                    LengthMap = 0;
                    SqlCommand cmd0 = new SqlCommand("select * from binproducts where binID = " + readerb["binid"].ToString(), connection);
                    SqlDataReader reader0 = cmd0.ExecuteReader();
                    if (reader0.HasRows)
                        while (reader0.Read())
                        {
                            //product map per bin
                            if (int.Parse(reader0["ProdID"].ToString()) < 32)
                                ProductMap0 = ProductMap0 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString())));
                            else if (int.Parse(reader0["ProdID"].ToString()) < 64)
                                ProductMap1 = ProductMap1 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 32));
                            else if (int.Parse(reader0["ProdID"].ToString()) < 96)
                                ProductMap2 = ProductMap2 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 64));
                            else if (int.Parse(reader0["ProdID"].ToString()) < 128)
                                ProductMap3 = ProductMap3 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 96));
                            else if (int.Parse(reader0["ProdID"].ToString()) < 160)
                                ProductMap4 = ProductMap4 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 128));
                            else
                                ProductMap5 = ProductMap5 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 160));
                        }
                    reader0.Close();
                    SqlCommand cmd1 = new SqlCommand("select * from binlengths where binID = " + readerb["binid"].ToString(), connection);
                    SqlDataReader reader1 = cmd1.ExecuteReader();
                    if (reader1.HasRows)
                        while (reader1.Read())
                        {
                            //length map per bin
                            LengthMap = LengthMap | Convert.ToUInt32(Math.Pow(2, double.Parse(reader1["LengthID"].ToString())));
                        }
                    reader1.Close();
                    if (readerb["binid"].ToString() == "1")
                        sqltext = "insert into datarequestsbin select getdate()," + readerb["binid"].ToString() + ",'" + readerb["binlabel"].ToString().Replace("'", "''") + "'," + readerb["binstatus"].ToString() + "," + readerb["binsize"].ToString() + "," + readerb["bincount"].ToString() + "," + ProductMap0 + "," + ProductMap1 + "," + ProductMap2 + "," + ProductMap3 + "," + ProductMap4 + "," + ProductMap5 + "," + LengthMap + ",0,0,0,0,0,0," + readerb["binstamps"].ToString() + "," + readerb["binsprays"].ToString() + "," + readerb["sortid"].ToString() + ",'" + readerb["trimflag"].ToString() + "','" + readerb["rw"].ToString() + "',3,1,0 select id=(select max(id) from datarequestsbin with(NOLOCK))";
                    else
                        sqltext = "insert into datarequestsbin select getdate()," + readerb["binid"].ToString() + ",'" + readerb["binlabel"].ToString().Replace("'", "''") + "'," + readerb["binstatus"].ToString() + "," + readerb["binsize"].ToString() + "," + readerb["bincount"].ToString() + "," + ProductMap0 + "," + ProductMap1 + "," + ProductMap2 + "," + ProductMap3 + "," + ProductMap4 + "," + ProductMap5 + "," + LengthMap + ",0,0,0,0,0,0," + readerb["binstamps"].ToString() + "," + readerb["binsprays"].ToString() + "," + readerb["sortid"].ToString() + ",'" + readerb["trimflag"].ToString() + "','" + readerb["rw"].ToString() + "',2,1,0 select id=(select max(id) from datarequestsbin with(NOLOCK))";
                    SqlCommand cmd = new SqlCommand(sqltext, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequestsbin", int.Parse(reader["id"].ToString()));
                    reader.Close();
                    if (!succeeded)
                    {
                        LabelTimeout.Visible = true;
                        break;
                    }
                }
                SqlCommand cmd11 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1 where (datarequests & 1)=1", connection);
                cmd11.ExecuteNonQuery();
                readerb.Close();

                //send sorts
                SqlCommand cmd02 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2", connection);
                cmd02.ExecuteNonQuery();
                SqlCommand cmds = new SqlCommand("select * from sorts with(NOLOCK) where recipeid=(select recipeid from recipes where online=1)", connection);
                SqlDataReader readers = cmds.ExecuteReader();
                while (readers.Read())
                {
                    StampsPLC = 0;
                    SpraysPLC = 0;
                    ProductMap0 = 0;
                    ProductMap1 = 0;
                    ProductMap2 = 0;
                    ProductMap3 = 0;
                    ProductMap4 = 0;
                    ProductMap5 = 0;
                    LengthMap = 0;
                    ProductMap0c = 0;
                    ProductMap1c = 0;
                    ProductMap2c = 0;
                    LengthMapc = 0;
                    SqlCommand cmd0 = new SqlCommand("select * from sortproducts where sortID = " + readers["sortid"].ToString() + " and recipeid=(select recipeid from recipes where online=1)", connection);
                    SqlDataReader reader0 = cmd0.ExecuteReader();
                    if (reader0.HasRows)
                        while (reader0.Read())
                        {
                            //product map per sort
                            if (int.Parse(reader0["ProdID"].ToString()) < 32)
                                ProductMap0 = ProductMap0 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString())));
                            else if (int.Parse(reader0["ProdID"].ToString()) < 64)
                                ProductMap1 = ProductMap1 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 32));
                            else if (int.Parse(reader0["ProdID"].ToString()) < 96)
                                ProductMap2 = ProductMap2 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 64));
                            else if (int.Parse(reader0["ProdID"].ToString()) < 128)
                                ProductMap3 = ProductMap3 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 96));
                            else if (int.Parse(reader0["ProdID"].ToString()) < 160)
                                ProductMap4 = ProductMap4 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 128));
                            else
                                ProductMap5 = ProductMap5 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 160));
                        }
                    reader0.Close();
                    SqlCommand cmd1 = new SqlCommand("select * from sortlengths where sortID = " + readers["sortid"].ToString() + " and recipeid=(select recipeid from recipes where online=1)", connection);
                    SqlDataReader reader1 = cmd1.ExecuteReader();
                    if (reader1.HasRows)
                        while (reader1.Read())
                        {
                            //length map per sort
                            LengthMap = LengthMap | Convert.ToUInt32(Math.Pow(2, double.Parse(reader1["LengthID"].ToString())));
                        }
                    reader1.Close();

                    //determine whether to set the default stamp bit
                    SqlCommand cmd00 = new SqlCommand("if (select gradestamps from gradematrix where recipeid = (select recipeid from recipes where editing=1) and websortgradeid=(select min(gradeid) from products where prodid=(select min(prodid) from sortproducts where recipeid=(select recipeid from recipes where editing=1)and sortid=" + readers["sortid"].ToString() + "))) = (select sortstamps from Sorts where RecipeID=(select recipeid from recipes where editing=1) and SortID=" + readers["sortid"].ToString() + ") select defaultstamp=1 else select defaultstamp=0", connection);
                    SqlDataReader reader00 = cmd00.ExecuteReader();
                    if (reader00.HasRows)
                    {
                        reader00.Read();
                        if (reader00["defaultstamp"].ToString() == "1")
                            StampsPLC = -2147483648;
                    }
                    reader00.Close();
                    StampsPLC = StampsPLC | int.Parse(readers["sortstamps"].ToString());
                    SpraysPLC = SpraysPLC | int.Parse(readers["sortsprays"].ToString());
                    SqlCommand cmd = new SqlCommand("insert into datarequestssort select getdate()," + readers["sortid"].ToString() + ",'" + readers["sortlabel"].ToString().Replace("'", "''") + "'," + readers["sortsize"].ToString() + "," + readers["pkgspersort"].ToString() + "," + readers["ordercount"].ToString() + "," + ProductMap0 + "," + ProductMap1 + "," + ProductMap2 + "," + ProductMap3 + "," + ProductMap4 + "," + ProductMap5 + "," + LengthMap + "," + ProductMap0c + "," + ProductMap1c + "," + ProductMap2c + "," + LengthMapc + ",0,0,0,0,0,0," + StampsPLC.ToString() + "," + SpraysPLC.ToString() + "," + (Convert.ToInt32(readers["zone1stop"].ToString()) * 256 + Convert.ToInt32(readers["zone1start"].ToString())) + "," + (Convert.ToInt32(readers["zone2stop"].ToString()) * 256 + Convert.ToInt32(readers["zone2start"].ToString())) + ",'" + readers["trimflag"].ToString() + "','" + readers["rw"].ToString() + "','" + readers["active"].ToString() + "',2,1,0 select id=(select max(id) from datarequestssort with(NOLOCK))", connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequestssort", int.Parse(reader["id"].ToString()));
                    reader.Close();
                    if (!succeeded)
                    {
                        LabelTimeout.Visible = true;
                        break;
                    }
                }
                readers.Close();
                SqlCommand cmd13 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2 where (datarequests & 2)=2", connection);
                cmd13.ExecuteNonQuery();

                //send grade map to PLC
                SqlCommand cmd101 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 8", connection);
                cmd101.ExecuteNonQuery();
                SqlCommand cmdd = new SqlCommand("select * from gradematrix with(NOLOCK) where recipeid=(select recipeid from recipes where online=1)", connection);
                SqlDataReader readerd = cmdd.ExecuteReader();
                while (readerd.Read())
                {
                    SqlCommand cmd = new SqlCommand("insert into datarequestsgrade select getdate()," + readerd["plcgradeID"].ToString() + "," + readerd["websortgradeID"].ToString() + "," + readerd["GradeStamps"].ToString() + ",1,0 select id=(select max(id) from datarequestsgrade with(NOLOCK))", connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //make sure message is processed
                    bool succeeded = Raptor.MessageAckConfirm("datarequestsgrade", int.Parse(reader["id"].ToString()));
                    reader.Close();
                    if (!succeeded)
                    {
                        LabelTimeout.Visible = true;
                        break;
                    }
                }
                readerd.Close();
                SqlCommand cmd102 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-8 where (datarequests & 8)=8", connection);
                cmd102.ExecuteNonQuery();

                //send run thickness and width
                SqlCommand cmd1021 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests | 32768", connection);
                cmd1021.ExecuteNonQuery();
                //send sort edit trigger to PLC
                SqlCommand cmdsorttrigger = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 131072", connection);
                cmdsorttrigger.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
            }

            connection.Close();
        }

        protected void ButtonSendProducts_Click(object sender, EventArgs e)
        {
            LabelTimeout.Visible = false;

            int NumThicks, NumWidths, NumLengths, NumPETLengths, NumProducts;

            using SqlConnection con = new SqlConnection(Global.ConnectionString);
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT NumThicks, NumWidths, NumLengths, NumPETLengths, NumProducts FROM WEBSortSetup",con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                try
                {
                    reader.Read();
                    NumThicks = Global.GetValue<int>(reader, "NumThicks");
                    NumWidths = Global.GetValue<int>(reader, "NumWidths");
                    NumLengths = Global.GetValue<int>(reader, "NumLengths");
                    NumPETLengths = Global.GetValue<int>(reader, "NumPETLengths");
                    NumProducts = Global.GetValue<int>(reader, "NumProducts");
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    return;
                }
            }

            //send thickness, width, lengths, grade mapping, products, moistures, specs
            try
            {
                // Thickness
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 16", con))
                    cmd.ExecuteNonQuery();
                for (int i = 1; i <= NumThicks; i++)
                {
                    try
                    {
                        Thickness thick = Thickness.GetAtID(i);

                        if (thick is null)  //we must zero out unused thicknesses
                        {
                            if (!Thickness.DataRequestInsert(con, thick, false, true))
                            {
                                LabelTimeout.Visible = true;
                                return;
                            }
                        }
                        else if (!Thickness.DataRequestInsert(con, thick, false))
                        {
                            LabelTimeout.Visible = true;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-16 where (datarequests & 16)=16", con))
                    cmd.ExecuteNonQuery();

                // Width
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 32", con))
                    cmd.ExecuteNonQuery();
                for (int i = 1; i <= NumWidths; i++)
                {
                    try
                    {
                        Width width = Width.GetAtID(i);

                        if (width is null)  //we must zero out unused Widths
                        {
                            if (!Width.DataRequestInsert(con, width, false, true))
                            {
                                LabelTimeout.Visible = true;
                                return;
                            }
                        }
                        else if (!Width.DataRequestInsert(con, width, false))
                        {
                            LabelTimeout.Visible = true;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-32 where (datarequests & 32)=32", con))
                    cmd.ExecuteNonQuery();
                
                // Lengths
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 64", con))
                    cmd.ExecuteNonQuery();
                for (int i = 1; i <= NumLengths; i++)
                {
                    try
                    {
                        Length length = Length.GetAtID(i);

                        if (length is null)  //we must zero out unused Widths
                        {
                            if (!Length.DataRequestInsert(con, length, false, true))
                            {
                                LabelTimeout.Visible = true;
                                return;
                            }
                        }
                        else if (!Length.DataRequestInsert(con, length, false))
                        {
                            LabelTimeout.Visible = true;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-64 where (datarequests & 64)=64", con))
                    cmd.ExecuteNonQuery();

                // PET Length
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2048", con))
                    cmd.ExecuteNonQuery();
                for (int i = 1; i <= NumPETLengths; i++)
                {
                    try
                    {
                        PETLength length = PETLength.GetAtID(i);

                        if (length is null)  //we must zero out unused Widths
                        {
                            if (!PETLength.DataRequestInsert(con, length, false, true))
                            {
                                LabelTimeout.Visible = true;
                                return;
                            }
                        }
                        else if (!PETLength.DataRequestInsert(con, length, false))
                        {
                            LabelTimeout.Visible = true;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2048 where (datarequests & 2048)=2048", con))
                    cmd.ExecuteNonQuery();

                // Products
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 4", con))
                    cmd.ExecuteNonQuery();
                for (int i = 1; i <= NumProducts; i++)
                {
                    try
                    {
                        Product prod = Product.GetAtID(i);

                        if (prod is null)  //we must zero out unused Widths
                        {
                            if (!Product.DataRequestInsert(con, prod, false, true))
                            {
                                LabelTimeout.Visible = true;
                                return;
                            }
                        }
                        else if (!Product.DataRequestInsert(con, prod, false))
                        {
                            LabelTimeout.Visible = true;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-4 where (datarequests & 4)=4", con))
                    cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
            }
        }

        protected void ButtonSendSorts_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            LabelTimeout.Visible = false;
            //determine existing product and length map per bin
            uint ProductMap0, ProductMap1, ProductMap2, LengthMap;

            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2", connection);
            cmd01.ExecuteNonQuery();
            SqlCommand cmdb = new SqlCommand("select * from sorts with(NOLOCK) where recipeid=(select recipeid from recipes where online=1)", connection);
            SqlDataReader readerb = cmdb.ExecuteReader();
            while (readerb.Read())
            {
                ProductMap0 = 0;
                ProductMap1 = 0;
                ProductMap2 = 0;
                LengthMap = 0;
                SqlCommand cmd0 = new SqlCommand("select * from sortproducts where sortID = " + readerb["sortid"].ToString() + " and recipeid=(select recipeid from recipes where online=1)", connection);
                SqlDataReader reader0 = cmd0.ExecuteReader();
                if (reader0.HasRows)
                    while (reader0.Read())
                    {
                        //product map per sort
                        if (int.Parse(reader0["ProdID"].ToString()) < 32)
                            ProductMap0 = ProductMap0 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString())));
                        else if (int.Parse(reader0["ProdID"].ToString()) < 64)
                            ProductMap1 = ProductMap1 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 32));
                        else
                            ProductMap2 = ProductMap2 | Convert.ToUInt32(Math.Pow(2, double.Parse(reader0["ProdID"].ToString()) - 64));
                    }
                reader0.Close();
                SqlCommand cmd1 = new SqlCommand("select * from sortlengths where sortID = " + readerb["sortid"].ToString() + " and recipeid=(select recipeid from recipes where online=1)", connection);
                SqlDataReader reader1 = cmd1.ExecuteReader();
                if (reader1.HasRows)
                    while (reader1.Read())
                    {
                        //length map per sort
                        LengthMap = LengthMap | Convert.ToUInt32(Math.Pow(2, double.Parse(reader1["LengthID"].ToString())));
                    }
                reader1.Close();
                SqlCommand cmd = new SqlCommand("insert into datarequestssort select getdate()," + readerb["sortid"].ToString() + ",'" + readerb["sortlabel"].ToString() + "'," + readerb["sortsize"].ToString() + "," + readerb["pkgspersort"].ToString() + "," + readerb["ordercount"].ToString() + "," + ProductMap0 + "," + ProductMap1 + "," + ProductMap2 + "," + LengthMap + ",0,0,0," + readerb["sortstamps"].ToString() + "," + readerb["sortsprays"].ToString() + "," + (Convert.ToInt32(readerb["zone1stop"].ToString()) * 256 + Convert.ToInt32(readerb["zone1start"].ToString())) + "," + (Convert.ToInt32(readerb["zone2stop"].ToString()) * 256 + Convert.ToInt32(readerb["zone2start"].ToString())) + ",'" + readerb["trimflag"].ToString() + "','" + readerb["rw"].ToString() + "','" + readerb["active"].ToString() + "',2,1,0 select id=(select max(id) from datarequestssort with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestssort", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout.Visible = true;
                    break;
                }
            }
            SqlCommand cmd11 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2 where (datarequests & 2)=2", connection);
            cmd11.ExecuteNonQuery();

            readerb.Close();
            connection.Close();
        }

        protected void ButtonBackup_Click(object sender, EventArgs e)
        {
            if (TextBoxPassword.Text == "")
            {
                Raptor msg = new Raptor();
                ClientScript.RegisterStartupScript(this.GetType(), "ok", msg.MsgBoxInAsp("Please enter a password to associate with the backup file."));
                return;
            }
            LabelBackup.Text = "";
            string filename;
            string backupfilename;
            backupfilename = "RaptorWEBSortBackup " + System.DateTime.Now.ToString().Replace(":", ".").Replace("/", "-") + ".bak";
            filename = Server.MapPath(".") + "\\app_data\\databasebackups\\" + backupfilename;

            //filename = "c:\\raptorwebsort\\RaptorWebSortBackup " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + " " + System.DateTime.Now..Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second + ".bak";
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("BACKUP DATABASE  RaptorWebsort  TO DISK = '" + filename + "' WITH INIT;", connection);
                cmd.ExecuteNonQuery();
                SqlCommand cmd1 = new SqlCommand("insert into DatabaseBackupHistory select '" + backupfilename + "','" + TextBoxPassword.Text + "'", connection);
                cmd1.ExecuteNonQuery();
                SqlCommand cmd2 = new SqlCommand("update WEBSortSetup set MostRecentBackupDate=getdate()", connection);
                cmd2.ExecuteNonQuery();
                LabelBackup.Text = "Backup succeeded: " + filename;

                //write from the database to an xml file
                SqlCommand command = new SqlCommand("Select BackupFilename,Password from DatabaseBackupHistory", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                // create a new dataset
                System.Data.DataSet ds = new System.Data.DataSet();
                // fill dataset
                adapter.Fill(ds, "BackupHistory");
                // write dataset contents to an xml file by calling WriteXml method
                ds.WriteXml(Server.MapPath(".") + "/app_data/DatabaseBackups/BackupHistory.xml");
            }
            catch (Exception ex)
            {
                LabelBackup.Text = ex.Message;
            }
            connection.Close();
        }

        protected void ButtonReplace_Click(object sender, EventArgs e)
        {
            Response.Redirect("http://" + IPAddress + "/websortrestore/default.aspx");
        }

        protected void ButtonSendDrives_Click(object sender, EventArgs e)
        {
            int LengthID;
            int MasterLink = 0;
            bool Slave = false, Master = false, Independent = false, Lineal = false, Transverse = false, Lugged = false, Custom = false;

            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            LabelTimeout.Visible = false;

            SqlCommand cmdlength = new SqlCommand("select lengthid from lengths where lengthnominal=(select productlength*12 from drivecurrentstate)", connection);
            SqlDataReader readerlength = cmdlength.ExecuteReader();
            readerlength.Read();
            LengthID = int.Parse(readerlength["lengthid"].ToString());
            readerlength.Close();

            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1024", connection);
            cmd01.ExecuteNonQuery();
            SqlCommand cmdb = new SqlCommand("select *,speedmultiplier=length" + LengthID + "multiplier from drives,drivesettings where drives.driveid=drivesettings.driveid", connection);
            SqlDataReader readerb = cmdb.ExecuteReader();
            while (readerb.Read())
            {
                Slave = false;
                Master = false;
                Independent = false;
                Lineal = false;
                Transverse = false;
                Lugged = false;
                Custom = false;
                if (readerb["Type"].ToString() == "-1")  //stand alone
                {
                    Independent = true;
                }
                else if (readerb["Type"].ToString() == "0")  //master
                {
                    Master = true;
                }
                else  //slave
                {
                    Slave = true;
                    MasterLink = int.Parse(readerb["Type"].ToString());
                }
                if (readerb["configuration"].ToString() == "0")  //lineal
                {
                    Lineal = true;
                }
                else if (readerb["configuration"].ToString() == "1")  //transverse
                {
                    Transverse = true;
                }
                else if (readerb["configuration"].ToString() == "2") //lugged
                {
                    Lugged = true;
                }
                else
                    Custom = true;
                SqlCommand cmd = new SqlCommand("insert into datarequestsdrive select getdate()," + readerb["DriveID"].ToString() + "," + readerb["Command"].ToString() + ",0," + MasterLink + "," + readerb["maxspeed"].ToString() + "," + readerb["gearingactual"].ToString() + "," + readerb["speedmultiplier"].ToString() + ",'" + Slave + "','" + Master + "','" + Independent + "','" + Lineal + "','" + Transverse + "','" + Lugged + "','" + Custom + "',1,0  select id=(select max(id) from datarequestsdrive with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsdrive", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout.Visible = true;
                    break;
                }
            }
            SqlCommand cmd11 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024", connection);
            cmd11.ExecuteNonQuery();

            readerb.Close();
            connection.Close();
        }

        protected void ButtonSendTiming_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            LabelTimeout.Visible = false;
            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 256", connection);
            cmd01.ExecuteNonQuery();
            SqlCommand cmdb1 = new SqlCommand("select distinct plcid from timing with(NOLOCK)", connection);
            SqlDataReader readermain = cmdb1.ExecuteReader();
            while (readermain.Read())
            {
                int[] ItemArray = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int i = 0;
                SqlCommand cmdb = new SqlCommand("select * from timing where plcid=" + readermain["plcid"].ToString(), connection);
                SqlDataReader reader = cmdb.ExecuteReader();
                while (reader.Read())
                {
                    ItemArray[i] = Convert.ToInt32(reader["ItemValue"].ToString());
                    i++;
                }
                reader.Close();
                SqlCommand cmd = new SqlCommand("insert into datarequeststiming select getdate()," + readermain["plcid"].ToString() + "," + ItemArray[0] + "," + ItemArray[1] + "," + ItemArray[2] + "," + ItemArray[3] + "," + ItemArray[4] + "," + ItemArray[5] + "," + ItemArray[6] + "," + ItemArray[7] + "," + ItemArray[8] + "," + ItemArray[9] + ",1,0 select id=(select max(id) from datarequeststiming with(NOLOCK))", connection);
                SqlDataReader reader1 = cmd.ExecuteReader();
                reader1.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequeststiming", int.Parse(reader1["id"].ToString()));
                reader1.Close();
                if (!succeeded)
                {
                    LabelTimeout.Visible = true;
                    break;
                }
            }
            SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-256 where (datarequests & 256)=256", connection);
            cmd1.ExecuteNonQuery();

            readermain.Close();
            connection.Close();
        }

        protected void ButtonReadBins_Click(object sender, EventArgs e)
        {
            string sqltext;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            LabelTimeout1.Visible = false;

            //read bins
            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1", connection);
            cmd01.ExecuteNonQuery();
            SqlCommand cmdb = new SqlCommand("select * from bins with(NOLOCK)", connection);
            SqlDataReader readerb = cmdb.ExecuteReader();
            while (readerb.Read())
            {
                sqltext = "insert into datarequestsbin select getdate()," + readerb["binid"].ToString() + ",' ',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 select id=(select max(id) from datarequestsbin with(NOLOCK))";
                SqlCommand cmd = new SqlCommand(sqltext, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsbin", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    break;
                }
            }
            SqlCommand cmd11 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1 where (datarequests & 1)=1", connection);
            cmd11.ExecuteNonQuery();
            readerb.Close();

            //read sorts
            SqlCommand cmd02 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2", connection);
            cmd02.ExecuteNonQuery();
            SqlCommand cmds = new SqlCommand("select * from sorts with(NOLOCK) where recipeid=(select recipeid from recipes where online=1)", connection);
            SqlDataReader readers = cmds.ExecuteReader();
            while (readers.Read())
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestssort select getdate()," + readers["sortid"].ToString() + ",' ',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 select id=(select max(id) from datarequestssort with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestssort", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    break;
                }
            }
            readers.Close();
            SqlCommand cmd13 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2 where (datarequests & 2)=2", connection);
            cmd13.ExecuteNonQuery();

            //read grade map to PLC
            SqlCommand cmd101 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 8", connection);
            cmd101.ExecuteNonQuery();
            SqlCommand cmdd = new SqlCommand("select * from gradematrix with(NOLOCK) where recipeid=(select recipeid from recipes where online=1)", connection);
            SqlDataReader readerd = cmdd.ExecuteReader();
            while (readerd.Read())
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestsgrade select getdate()," + readerd["plcgradeID"].ToString() + ",0,0,0,0 select id=(select max(id) from datarequestsgrade with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsgrade", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    break;
                }
            }
            readerd.Close();
            SqlCommand cmd102 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-8 where (datarequests & 8)=8", connection);
            cmd102.ExecuteNonQuery();
        }

        protected void ButtonReadProducts_Click(object sender, EventArgs e)
        {
            bool succeeded;

            LabelTimeout1.Visible = false;
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand cmd0 = new SqlCommand("select * from WEBSortSetup", connection);
            SqlDataReader reader0 = cmd0.ExecuteReader();
            reader0.Read();
            int NumThicks = int.Parse(reader0["NumThicks"].ToString());
            int NumWidths = int.Parse(reader0["NumWidths"].ToString());
            int NumLengths = int.Parse(reader0["NumLengths"].ToString());
            int NumMoistures = int.Parse(reader0["NumMoistures"].ToString());
            int NumPETLengths = int.Parse(reader0["NumPETLengths"].ToString());
            int NumProducts = int.Parse(reader0["NumProducts"].ToString());
            reader0.Close();

            //read thickness, width, lengths, grade mapping, products, moistures, specs
            //thickness
            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 16", connection);
            cmd01.ExecuteNonQuery();
            for (int i = 1; i <= NumThicks; i++)
            {
                SqlCommand cmdt1 = new SqlCommand("insert into datarequeststhickness select getdate()," + i.ToString() + ",0,0,0,0,0 select id=(select max(id) from datarequeststhickness with(NOLOCK))", connection);
                SqlDataReader readert1 = cmdt1.ExecuteReader();
                readert1.Read();
                //make sure message is processed
                succeeded = Raptor.MessageAckConfirm("datarequeststhickness", int.Parse(readert1["id"].ToString()));
                readert1.Close();
                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    return;
                }
            }
            SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-16 where (datarequests & 16)=16", connection);
            cmd1.ExecuteNonQuery();

            //width
            SqlCommand cmd02 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 32", connection);
            cmd02.ExecuteNonQuery();
            for (int i = 1; i <= NumWidths; i++)
            {
                SqlCommand cmdw1 = new SqlCommand("insert into datarequestswidth select getdate()," + i.ToString() + ",0,0,0,0,0 select id=(select max(id) from datarequestswidth with(NOLOCK))", connection);
                SqlDataReader readerw1 = cmdw1.ExecuteReader();
                readerw1.Read();
                //make sure message is processed
                succeeded = Raptor.MessageAckConfirm("datarequestswidth", int.Parse(readerw1["id"].ToString()));
                readerw1.Close();
                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    return;
                }
            }
            SqlCommand cmd11 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-32 where (datarequests & 32)=32", connection);
            cmd11.ExecuteNonQuery();

            //moistures
            SqlCommand cmd05m = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 512", connection);
            cmd05m.ExecuteNonQuery();
            for (int i = 1; i <= NumMoistures; i++)
            {
                SqlCommand cmdlm = new SqlCommand("select * from moistures with(NOLOCK) where moistureid=" + i, connection);
                SqlDataReader readerlm = cmdlm.ExecuteReader();
                readerlm.Read();
                if (!readerlm.HasRows)  //we must zero out unused moistures
                {
                    SqlCommand cmdm = new SqlCommand("insert into datarequestsmoisture select getdate()," + i.ToString() + ",0,0,1,0 select id=(select max(id) from datarequestsmoisture with(NOLOCK))", connection);
                    SqlDataReader readerm = cmdm.ExecuteReader();
                    readerm.Read();
                    //make sure message is processed
                    succeeded = Raptor.MessageAckConfirm("datarequestsmoisture", int.Parse(readerm["id"].ToString()));
                    readerm.Close();

                    if (!succeeded)
                    {
                        LabelTimeout.Visible = true;
                        break;
                    }
                    readerlm.Close();
                }
                else
                {
                    SqlCommand cmdm = new SqlCommand("insert into datarequestsmoisture select getdate()," + i.ToString() + "," + readerlm["moistureMin"].ToString() + "," + readerlm["moistureMax"].ToString() + ",1,0 select id=(select max(id) from datarequestsmoisture with(NOLOCK))", connection);
                    SqlDataReader readerm = cmdm.ExecuteReader();
                    readerm.Read();
                    //make sure message is processed
                    succeeded = Raptor.MessageAckConfirm("datarequestsmoisture", int.Parse(readerm["id"].ToString()));
                    readerm.Close();

                    if (!succeeded)
                    {
                        LabelTimeout.Visible = true;
                        break;
                    }
                    readerlm.Close();
                }
            }
            SqlCommand cmd15m = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-512 where (datarequests & 512)=512", connection);
            cmd15m.ExecuteNonQuery();

            //lengths
            SqlCommand cmd05 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 64", connection);
            cmd05.ExecuteNonQuery();
            for (int i = 1; i <= NumLengths; i++)
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestslength select getdate()," + i.ToString() + ",0,0,0,0,0 select id=(select max(id) from datarequestslength with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                succeeded = Raptor.MessageAckConfirm("datarequestslength", int.Parse(reader["id"].ToString()));
                reader.Close();

                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    break;
                }
            }
            SqlCommand cmd15 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-64 where (datarequests & 64)=64", connection);
            cmd15.ExecuteNonQuery();

            //pet length data
            SqlCommand cmd05a = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 2048", connection);
            cmd05a.ExecuteNonQuery();
            for (int i = 1; i <= NumPETLengths; i++)
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestspetlength select getdate()," + i.ToString() + ",0,0,0,0,0 select id=(select max(id) from datarequestspetlength with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                succeeded = Raptor.MessageAckConfirm("datarequestspetlength", int.Parse(reader["id"].ToString()));
                reader.Close();

                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    break;
                }
            }
            SqlCommand cmd15a = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2048 where (datarequests & 2048)=2048", connection);
            cmd15a.ExecuteNonQuery();

            //products
            SqlCommand cmd03 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 4", connection);
            cmd03.ExecuteNonQuery();
            for (int i = 1; i <= NumProducts; i++)
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestsproduct select getdate()," + i.ToString() + ",'False',0,0,0,0,0,0,0,0,0,0 select id=(select max(id) from datarequestsproduct with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                succeeded = Raptor.MessageAckConfirm("datarequestsproduct", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    break;
                }
            }
            SqlCommand cmd12 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-4 where (datarequests & 4)=4", connection);
            cmd12.ExecuteNonQuery();

            connection.Close();
        }

        protected void ButtonReadDrives_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            connection.Open();
            LabelTimeout1.Visible = false;

            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 1024", connection);
            cmd01.ExecuteNonQuery();
            SqlCommand cmdb = new SqlCommand("select driveid from drivesettings", connection);
            SqlDataReader readerb = cmdb.ExecuteReader();
            while (readerb.Read())
            {
                SqlCommand cmd = new SqlCommand("insert into datarequestsdrive select getdate()," + readerb["DriveID"].ToString() + ",0,0,0,0,0,0,0,0,0,0,0,0,0,0,0  select id=(select max(id) from datarequestsdrive with(NOLOCK))", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequestsdrive", int.Parse(reader["id"].ToString()));
                reader.Close();
                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    break;
                }
            }
            SqlCommand cmd11 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024", connection);
            cmd11.ExecuteNonQuery();

            readerb.Close();
            connection.Close();
        }

        protected void ButtonReadTiming_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            LabelTimeout1.Visible = false;
            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 256", connection);
            cmd01.ExecuteNonQuery();
            SqlCommand cmdb1 = new SqlCommand("select distinct plcid from timing with(NOLOCK)", connection);
            SqlDataReader readermain = cmdb1.ExecuteReader();
            while (readermain.Read())
            {
                SqlCommand cmd = new SqlCommand("insert into datarequeststiming select getdate()," + readermain["plcid"].ToString() + ",0,0,0,0,0,0,0,0,0,0,0,0 select id=(select max(id) from datarequeststiming with(NOLOCK))", connection);
                SqlDataReader reader1 = cmd.ExecuteReader();
                reader1.Read();
                //make sure message is processed
                bool succeeded = Raptor.MessageAckConfirm("datarequeststiming", int.Parse(reader1["id"].ToString()));
                reader1.Close();
                if (!succeeded)
                {
                    LabelTimeout1.Visible = true;
                    break;
                }
            }
            SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-256 where (datarequests & 256)=256", connection);
            cmd1.ExecuteNonQuery();

            readermain.Close();
            connection.Close();
        }

        protected void ButtonReadPLCProduction_Click(object sender, EventArgs e)
        {
            Raptor cs1 = new Raptor();
            string connectionString = Global.ConnectionString;
            System.Data.SqlClient.SqlConnection connection;
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            LabelTimeout1.Visible = false;
            SqlCommand cmd01 = new SqlCommand("update RaptorCommSettings set DataRequests = DataRequests | 524288", connection);
            cmd01.ExecuteNonQuery();
            connection.Close();
        }
    }
}