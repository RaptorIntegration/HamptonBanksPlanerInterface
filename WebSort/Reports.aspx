<%@ Page Language="C#" MasterPageFile="~/WebSort.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="WebSort.Reports" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxMenu" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPopupControl" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >

    <script language='javascript'>
    function myPostBack() {
        var o = window.event.srcElement;

        if (o.tagName == "INPUT" && o.type == "checkbox") {
            __doPostBack("", "");
        }
    }
</script>
    <style type="text/css">
    .style1
    {
            width: 243px;
        }
    .style2
    {
        width: 67px;
    }
    .main-grid {
        display: grid;
        grid-template-columns: 300px 1000px auto;
        grid-gap: 5rem;
    }
    .toolbar{
        width:inherit !important;
    }
        .crystal {
            background-color: white;
            box-shadow: 0px 5px 5px 7px var(--shadow);
        }
        .crystal-container {
            display: flex;
            justify-content:center;
        }
    .filter{
        display:grid;
        grid-template-rows: repeat(auto-fill, minmax(20px, 40px));
        grid-gap: 1rem;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="MainContent"  >
    <asp:UpdatePanel ID="UpdatePanelReports" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="main-grid">
                <div>
                    <asp:TreeView ID="TreeView1" runat="server" NodeWrap="True"
                        EnableClientScript="true"
                        ontreenodepopulate="TreeView1_TreeNodePopulate"
                        ShowLines="True"
                        onselectednodechanged="TreeView1_SelectedNodeChanged"
                        ontreenodecheckchanged="TreeView1_TreeNodeCheckChanged"
                        ShowCheckBoxes="Leaf"
                        BorderStyle="None">
                        <SelectedNodeStyle ForeColor="#3333CC" BorderColor="Black"/>
                        <Nodes>
                            <asp:TreeNode Text="Reports" Expanded="true"  PopulateOnDemand="true" Value="0" SelectAction="Expand"/>
                        </Nodes>
                    </asp:TreeView>
                </div>
                <div class="crystal-container">
                    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server"
                        ToolbarImagesFolderUrl="~/images/toolbar/"
                        ToolbarStyle-CssClass="toolbar" Height="800"
                        CssClass="crystal"
                        AutoDataBind="True"
                        EnableDatabaseLogonPrompt="False"
                        ReportSourceID="CrystalReportSource1"
                        DisplayGroupTree="False"
                        HasCrystalLogo="False"
                        HasDrillUpButton="False"
                        HasRefreshButton="True"
                        HasToggleGroupTreeButton="False"
                        BorderStyle="None"
                        HasViewList="False"
                        ToolbarStyle-BorderStyle="None"
                        EnableDrillDown="False"
                        EnableTheming="False"
                        HasGotoPageButton="False"
                        HasSearchButton="False"
                        ToolbarStyle-BackColor="#A4A4A4"
                        ToolbarStyle-BorderWidth="2px"
                        BorderColor="Black"
                        BorderWidth="2px" />


                </div>
                <div class="filter">
                    <label>
                        <asp:CheckBox ID="CheckBox1" runat="server" Checked="True" />
                        Checkbox indicates an automatic end of shift printed report
                    </label>

                    <asp:Button ID="ButtonPrint" runat="server"
                        onclick="ButtonPrint_Click"
                        Text="Print Report"
                        CssClass="btn-raptor" />
                    <asp:Button ID="ButtonPrintNow" runat="server"
                        onclick="ButtonPrintNow_Click"
                        Text="Print End of Shift Reports"
                        CssClass="btn-raptor" />

                    <div>
                        <label>Automatically Print To: </label>
                        <asp:DropDownList ID="cmbPrinters" runat="server"
                            AutoPostBack="True"
                            onselectedindexchanged="cmbPrinters_SelectedIndexChanged"
                            CssClass="form-control">
                        </asp:DropDownList>
                    </div>
                    <div>
                        <label>Copies</label>
                        <asp:TextBox ID="TextBox1" runat="server"
                            ontextchanged="TextBox1_TextChanged"
                            CssClass="form-control"
                            CausesValidation="True">
                        </asp:TextBox>
                    </div>

                    <div>
                        <asp:RadioButton ID="RadioButtonCurrentShift" runat="server"
                            Checked="True"
                            oncheckedchanged="RadioButtonCurrentShift_CheckedChanged"
                            Text="Current Shift"
                            AutoPostBack="True"
                            GroupName="1" />
                        <br />
                        <asp:RadioButton ID="RadioButtonSelectedShift" runat="server"
                            oncheckedchanged="RadioButtonSelectedShift_CheckedChanged"
                            Text="Selected Shift"
                            AutoPostBack="True"
                            GroupName="1" />
                    </div>

                    <asp:Panel ID="PanelShift" runat="server" Visible="False">
                        <asp:Label ID="LabelStartShift" runat="server" Text="Select Start Date:"
                            Visible="True"></asp:Label>
                        <asp:Calendar ID="CalendarShiftStart" runat="server"
                            BackColor="#F8F8F8"
                            BorderStyle="None"
                            DayNameFormat="Shortest"
                            Font-Names="Verdana"
                            Font-Size="8pt"
                            ForeColor="#663399"
                            Height="142px"
                            ShowGridLines="True"
                            Width="218px"
                            onselectionchanged="CalendarShiftStart_SelectionChanged"
                            FirstDayOfWeek="Sunday">
                            <SelectedDayStyle BackColor="#2E3A62" Font-Bold="True" />
                            <SelectorStyle BackColor="#FFCC66" />
                            <TodayDayStyle BackColor="#4A5783" ForeColor="#F8F8F8" />
                            <OtherMonthDayStyle ForeColor="#CC9966" />
                            <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
                            <DayHeaderStyle BackColor="#28304D" Font-Bold="True" ForeColor="#F8F8F8" Height="1px" />
                            <TitleStyle BackColor="#2E3A62" Font-Bold="True" Font-Size="9pt" ForeColor="#FFFFCC" />
                        </asp:Calendar>

                        <asp:RadioButtonList ID="RadioButtonListShiftStart" runat="server"
                            onselectedindexchanged="RadioButtonListShiftStart_SelectedIndexChanged"
                            Visible="False"
                            AutoPostBack="True">
                        </asp:RadioButtonList>

                        <asp:Label ID="LabelShiftStartError" runat="server" ForeColor="Red"
                            Text="No Shift Starts Exist on this Date" Visible="False"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="LabelEndShift" runat="server" Text="Select End Date:"
                            Visible="True"></asp:Label>
                        <asp:Calendar ID="CalendarShiftEnd" runat="server"
                            BackColor="#F8F8F8"
                            BorderStyle="None"
                            DayNameFormat="Shortest"
                            Font-Names="Verdana"
                            Font-Size="8pt"
                            ForeColor="#663399"
                            Height="142px"
                            ShowGridLines="True"
                            Width="218px"
                            onselectionchanged="CalendarShiftEnd_SelectionChanged"
                            FirstDayOfWeek="Sunday">
                            <SelectedDayStyle BackColor="#2E3A62" Font-Bold="True" />
                            <SelectorStyle BackColor="#FFCC66" />
                            <TodayDayStyle BackColor="#4A5783" ForeColor="#F8F8F8" />
                            <OtherMonthDayStyle ForeColor="#CC9966" />
                            <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
                            <DayHeaderStyle BackColor="#28304D" Font-Bold="True" ForeColor="#F8F8F8" Height="1px" />
                            <TitleStyle BackColor="#2E3A62" Font-Bold="True" Font-Size="9pt" ForeColor="#FFFFCC" />
                        </asp:Calendar>

                        <asp:RadioButtonList ID="RadioButtonListShiftEnd" runat="server"
                            onselectedindexchanged="RadioButtonListShiftEnd_SelectedIndexChanged"
                            Visible="False" AutoPostBack="True">
                        </asp:RadioButtonList>
                        <asp:Label ID="LabelShiftError" runat="server" ForeColor="Red"
                            Text="No Shifts Ends Exist on this Date" Visible="False"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="LabelRecipe" runat="server" Text="Recipe" Visible="False"></asp:Label>
                        <br />
                        <asp:DropDownList ID="DropDownListRecipes" runat="server"
                            DataSourceID="SqlDataSource1" DataTextField="recipelabel"
                            DataValueField="RecipeID" class="form-control" AutoPostBack="True"
                            onselectedindexchanged="DropDownListRecipes_SelectedIndexChanged"
                            Visible="False">
                        </asp:DropDownList>
                        <br />
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                            ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                            SelectCommand="select recipeid=0,recipelabel='&lt;ALL&gt;',0,0,0,0,0,0 union SELECT * FROM [Recipes] ORDER BY [RecipeLabel]">
                        </asp:SqlDataSource>
                    </asp:Panel>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanelReports">
        <ProgressTemplate>
            <div class="overlay">
                 <div class="loader" >Loading...</div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
        <Report FileName="C:\inetpub\wwwroot\websort\app_data\reports\production summary.rpt">
        </Report>
    </CR:CrystalReportSource>

</asp:Content>
