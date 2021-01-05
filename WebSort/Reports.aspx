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
</style>
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="MainContent"  > 
    <table style="width: 100%;">
    <tr>
        <td class="style1" nowrap="nowrap" 
            style="border: 2px solid #000000; padding: 5px 10px 5px 10px; vertical-align: top; font-size: small; white-space: nowrap;font-weight: bold; border-spacing: 5px; text-indent: 0px; font-family: Arial, Helvetica, sans-serif;" 
            valign="top">
            
            
            
            <asp:Panel ID="Panel3" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                >
                <asp:TreeView ID="TreeView1" runat="server" NodeWrap="True"   
                EnableClientScript="true"   
                ontreenodepopulate="TreeView1_TreeNodePopulate" 
                ShowLines="True" onselectednodechanged="TreeView1_SelectedNodeChanged" 
                ontreenodecheckchanged="TreeView1_TreeNodeCheckChanged" 
                ShowCheckBoxes="Leaf" BorderWidth="1px" BorderStyle="None">
                    <SelectedNodeStyle ForeColor="#3333CC" BorderColor="Black" BorderStyle="Solid" 
                        BorderWidth="1px" />
                    <Nodes>
                        <asp:TreeNode Text="Reports" Expanded="true"  PopulateOnDemand="true" Value="0"  
                        SelectAction="Expand"/>
                    </Nodes>
                    <RootNodeStyle  />
                    <NodeStyle  />
                </asp:TreeView>
                <br />
                <asp:CheckBox ID="CheckBox1" runat="server" Checked="True" />
                <asp:Label ID="Label1" runat="server" Text="Checkbox indicates an automatic"></asp:Label>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp<asp:Label ID="Label2" runat="server" Text="end of shift printed report"></asp:Label>
                <br />
                <br />
                &nbsp;<asp:Button ID="ButtonPrint" runat="server" onclick="ButtonPrint_Click" 
                    Text="Print Report" Width="175px" />
                
                
                <br />
                &nbsp;<asp:Button ID="ButtonPrintNow" runat="server" onclick="ButtonPrintNow_Click" 
                    Text="Print End of Shift Reports" Width="175px" />
                
                
                <br />
                <asp:Label ID="Label3" runat="server" Text="Automatically Print To:"></asp:Label>
                <br />
                <asp:DropDownList ID="cmbPrinters" runat="server" AutoPostBack="True" 
                    onselectedindexchanged="cmbPrinters_SelectedIndexChanged" Width="218px">
                </asp:DropDownList>
                <table style="width:100%;">
                    <tr>
                        <td class="style2">
                            <asp:Label ID="Label4" runat="server" Text="Copies:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox1" runat="server" ontextchanged="TextBox1_TextChanged" 
                                Width="50px" CausesValidation="True"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            
            
            <br />
            <asp:Panel ID="Panel2" runat="server" Height="32px" BorderStyle="Solid" 
                BorderWidth="1px">
                <asp:RadioButton ID="RadioButtonCurrentShift" runat="server" Checked="True" 
                    oncheckedchanged="RadioButtonCurrentShift_CheckedChanged" 
                    Text="Current Shift" AutoPostBack="True" GroupName="1" />
                <br />
                <asp:RadioButton ID="RadioButtonSelectedShift" runat="server" 
                    oncheckedchanged="RadioButtonSelectedShift_CheckedChanged" 
                    Text="Selected Shift" AutoPostBack="True" GroupName="1" />
            </asp:Panel>
            <br />
            <asp:Panel ID="PanelShift" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                Visible="False">
                <asp:Label ID="LabelStartShift" runat="server" Text="Select Start Date:" 
                    Visible="True"></asp:Label>
                <asp:Calendar ID="CalendarShiftStart" runat="server" BackColor="#FFFFCC" 
                    BorderColor="#FFCC66" BorderWidth="1px" DayNameFormat="Shortest" 
                    Font-Names="Verdana" Font-Size="8pt" ForeColor="#663399" Height="142px" 
                    ShowGridLines="True" Width="218px" 
                    onselectionchanged="CalendarShiftStart_SelectionChanged" 
                    FirstDayOfWeek="Sunday">
                    <SelectedDayStyle BackColor="#CCCCFF" Font-Bold="True" />
                    <SelectorStyle BackColor="#FFCC66" />
                    <TodayDayStyle BackColor="#FFCC66" ForeColor="White" />
                    <OtherMonthDayStyle ForeColor="#CC9966" />
                    <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
                    <DayHeaderStyle BackColor="#FFCC66" Font-Bold="True" Height="1px" />
                    <TitleStyle BackColor="#990000" Font-Bold="True" Font-Size="9pt" 
                        ForeColor="#FFFFCC" />
                </asp:Calendar>
                
                <asp:RadioButtonList ID="RadioButtonListShiftStart" runat="server" 
                    onselectedindexchanged="RadioButtonListShiftStart_SelectedIndexChanged" 
                    Visible="False" AutoPostBack="True">
                </asp:RadioButtonList>
                
                <asp:Label ID="LabelShiftStartError" runat="server" ForeColor="Red" 
                    Text="No Shift Starts Exist on this Date" Visible="False"></asp:Label>
                <br />
                <br />
                <asp:Label ID="LabelEndShift" runat="server" Text="Select End Date:" 
                    Visible="True"></asp:Label>
                <asp:Calendar ID="CalendarShiftEnd" runat="server" BackColor="#FFFFCC" 
                    BorderColor="#FFCC66" BorderWidth="1px" DayNameFormat="Shortest" 
                    FirstDayOfWeek="Sunday" Font-Names="Verdana" Font-Size="8pt" 
                    ForeColor="#663399" Height="142px" 
                    onselectionchanged="CalendarShiftEnd_SelectionChanged" ShowGridLines="True" 
                    Width="218px">
                    <SelectedDayStyle BackColor="#CCCCFF" Font-Bold="True" />
                    <SelectorStyle BackColor="#FFCC66" />
                    <TodayDayStyle BackColor="#FFCC66" ForeColor="White" />
                    <OtherMonthDayStyle ForeColor="#CC9966" />
                    <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
                    <DayHeaderStyle BackColor="#FFCC66" Font-Bold="True" Height="1px" />
                    <TitleStyle BackColor="#990000" Font-Bold="True" Font-Size="9pt" 
                        ForeColor="#FFFFCC" />
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
                    DataValueField="RecipeID" Width="218px" AutoPostBack="True" 
                    onselectedindexchanged="DropDownListRecipes_SelectedIndexChanged" 
                    Visible="False">
                </asp:DropDownList>
                <br />
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>" 
                    SelectCommand="select recipeid=0,recipelabel='&lt;ALL&gt;',0,0,0,0,0,0
union
SELECT * FROM [Recipes] ORDER BY [RecipeLabel]">
                </asp:SqlDataSource>
            </asp:Panel>
            
            <br />
            <br />
            
        </td>
        <td style="padding-left: 20px; vertical-align: top;">
       
        <asp:Panel ID="Panel1" runat="server" BackColor="White" BorderColor="Black" 
                BorderStyle="None" BorderWidth="2px">
               
                
                <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" ToolbarImagesFolderUrl="~/images/toolbar/"  
    AutoDataBind="True" EnableDatabaseLogonPrompt="False" Height="1037px" 
    ReportSourceID="CrystalReportSource1" Width="773px" 
    DisplayGroupTree="False" HasCrystalLogo="False" HasDrillUpButton="False" HasRefreshButton="True" 
    HasToggleGroupTreeButton="False" BorderStyle="None" 
        HasViewList="False" ToolbarStyle-BorderStyle="None" 
                    EnableDrillDown="False" EnableTheming="False" HasGotoPageButton="False" 
                    HasSearchButton="False" ToolbarStyle-BackColor="#A4A4A4" 
                    ToolbarStyle-BorderWidth="2px" BorderColor="Black" BorderWidth="2px" />
                <br />
            </asp:Panel>
         
        </td>
        <td>
            &nbsp;</td>
    </tr>
    <tr>
        <td class="style1">
            &nbsp;</td>
        <td>
            &nbsp;</td>
        <td>
            &nbsp;</td>
    </tr>
    <tr>
        <td class="style1">
            &nbsp;</td>
        <td>
            &nbsp;</td>
        <td>
            &nbsp;</td>
    </tr>
</table>


<CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
    <Report FileName="C:\inetpub\wwwroot\websort\app_data\reports\production summary.rpt">
    </Report>
</CR:CrystalReportSource>










</asp:Content>


