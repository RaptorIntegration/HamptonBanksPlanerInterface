<%@ Page Language="C#" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" StyleSheetTheme="Theme" MasterPageFile="~/WebSort.Master" CodeBehind="GraderBoardTest.aspx.cs" Inherits="WebSort.GraderBoardTest" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <style type="text/css">
        .style1
        {
            width: 145px;
        }
        .style2
        {
            width: 172px;
        }
        .style4
        {
            height: 16px;
        }
        .style5
    {
            width: 228px;
        }
        .style7
    {
            width: 204px;
        }
        .style8
        {
            width: 215px;
        }
        .style9
        {
            width: 239px;
        }
        .style10
        {
            height: 16px;
            width: 239px;
        }
        .style11
        {
            width: 70px;
        }
        .style12
        {
            width: 166px;
        }
        .auto-style1 {
            width: 204px;
            height: 16px;
        }
        .auto-style3 {
            width: 103px;
        }
        .auto-style4 {
            width: 97px;
        }
        .auto-style5 {
            width: 30px;
        }
        .auto-style6 {
            height: 16px;
            width: 107px;
        }
        .auto-style8 {
            height: 16px;
            width: 126px;
        }
        .auto-style9 {
            width: 126px;
        }
        .auto-style10 {
            width: 105px;
        }
        .auto-style11 {
            height: 16px;
            width: 105px;
        }
        .auto-style17 {
            height: 16px;
            width: 39px;
        }
        .auto-style18 {
            width: 39px;
        }
        .auto-style19 {
            width: 15px;
        }
        .auto-style20 {
            width: 33px;
        }
        </style>
    
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table style="width: 100%;">
                <tr>
                    <td class="style7" valign="top">
                        <asp:Label ID="Label2" runat="server" Font-Size="Small"
                            Text="Test Grade(s):"></asp:Label>
                    </td>
                    <td class="style12" valign="top">
                        <asp:CheckBoxList ID="CheckBoxList2" runat="server" AppendDataBoundItems="True"
                            DataSourceID="SqlDataSourceGrades" DataTextField="gradelabel"
                            DataValueField="plcgradeid" Font-Size="Small" Height="24px"
                            RepeatColumns="1" Width="182px">
                            <asp:ListItem Value="0">All</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                    <td class="auto-style5" valign="top">
                        <asp:Label ID="Label7" runat="server" Font-Size="Small"
                            Text="Test Length(s):"></asp:Label>
                    </td>
                    <td align="left" class="auto-style20" valign="top">
                        <asp:CheckBoxList ID="CheckBoxList3" runat="server" AppendDataBoundItems="True"
                            DataSourceID="SqlDataSourceLengths" DataTextField="LengthLabel"
                            DataValueField="LengthID" Font-Size="Small"
                            OnSelectedIndexChanged="CheckBoxList1_SelectedIndexChanged" RepeatColumns="1">
                            <asp:ListItem Value="0">All</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                    <td valign="top" class="auto-style4">
                        <asp:Label ID="Label1" runat="server" Font-Size="Small"
                            Text="Test Grader(s):"></asp:Label>
                    </td>
                    <td class="auto-style18" valign="top">
                        <asp:CheckBoxList ID="CheckBoxList1" runat="server" AppendDataBoundItems="True"
                            DataSourceID="SqlDataSourceGraders" DataTextField="GraderDescription"
                            DataValueField="GraderID" Font-Size="Small"
                            OnSelectedIndexChanged="CheckBoxList1_SelectedIndexChanged" RepeatColumns="2" Width="190px">
                            <asp:ListItem Value="0">All</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                    <td class="auto-style5" valign="top">
                        <asp:Label ID="Label8" runat="server" Font-Size="Small" Text="Test Thickness:"></asp:Label>
                    </td>
                    <td class="auto-style10" valign="top">
                        <asp:RadioButtonList ID="RadioButtonListThickness" runat="server" DataSourceID="SqlDataSourceThickness" DataTextField="Nominal" DataValueField="id" Font-Size="Small">
                        </asp:RadioButtonList>
                    </td>
                    <td class="auto-style5" valign="top">
                        <asp:Label ID="Label9" runat="server" Font-Size="Small" Text="Test Width(s):"></asp:Label>
                    </td>
                    <td class="style9" valign="top">
                        <asp:CheckBoxList ID="CheckBoxListWidth" runat="server" AppendDataBoundItems="True" DataSourceID="SqlDataSourceWidth" DataTextField="Nominal" DataValueField="id" Font-Size="Small" Height="24px" RepeatColumns="1">
                            <asp:ListItem Value="0">All</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="style7" valign="top">&nbsp;&nbsp;</td>
                    <td class="style12">&nbsp;</td>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style20">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td class="auto-style18">&nbsp;</td>
                    <td class="auto-style5">&nbsp;</td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style9">&nbsp;</td>
                    <td class="style9">&nbsp;</td>
                </tr>
                <tr>
                    <td class="style7" valign="top">
                        <asp:Label ID="Label4" runat="server" Font-Size="Small"
                            Text="Sample Size:"></asp:Label>
                    </td>
                    <td class="style12" valign="top">
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TextBox ID="TextBoxSampleSize" runat="server"
                                    OnTextChanged="TextBoxSampleSize_TextChanged" Width="70px">1</asp:TextBox>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td valign="top" class="auto-style3">&nbsp;</td>
                    <td class="auto-style20">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td class="auto-style18">&nbsp;</td>
                    <td class="auto-style5">&nbsp;</td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style9">&nbsp;</td>
                    <td class="style9">&nbsp;</td>
                </tr>
                <tr>
                    <td class="style7">
                        <asp:Label ID="Label5" runat="server" Font-Size="Small"
                            Text="Interval:"></asp:Label>
                    </td>
                    <td class="style12">
                        <asp:TextBox ID="TextBoxInterval" runat="server" Width="70px">1</asp:TextBox>
                    </td>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td class="auto-style18">&nbsp;</td>
                    <td class="auto-style5">&nbsp;</td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style9">&nbsp;</td>
                    <td class="style9">&nbsp;</td>
                </tr>
                <tr>
                    <td class="style7">
                        <asp:Label ID="Label3" runat="server" Font-Size="Small"
                            Text="Test Bay #:"></asp:Label>
                    </td>
                    <td class="style12">
                        <asp:DropDownList ID="DropDownListBay" runat="server"
                            DataSourceID="SqlDataSourceBays" DataTextField="BinID"
                            DataValueField="BinID" Width="70px">
                        </asp:DropDownList>
                        <asp:Label ID="LabelBayError0" runat="server" Font-Size="Small" ForeColor="Red"
                            Text="Choose a Bay" Visible="False"></asp:Label>
                    </td>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style20">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td class="auto-style18">&nbsp;</td>
                    <td class="auto-style5">&nbsp;</td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style9">&nbsp;</td>
                    <td class="style9">&nbsp;</td>
                </tr>
                <tr>
                    <td class="style7">
                        <asp:Label ID="Label6" runat="server" Font-Size="Small"
                            Text="Options:"></asp:Label>
                    </td>
                    <td class="style12">
                        <asp:CheckBox ID="CheckBoxStamp" runat="server" Checked="True"
                            Font-Size="Small" Text="Stamp" />
                        <asp:CheckBox ID="CheckBoxTrim" runat="server" Checked="True" Font-Size="Small"
                            Text="Trim" />
                    </td>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style20">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td class="auto-style18">&nbsp;</td>
                    <td class="auto-style5">&nbsp;</td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style9">&nbsp;</td>
                    <td class="style9">&nbsp;</td>
                </tr>
                <tr>
                    <td class="style7">&nbsp;&nbsp;</td>
                    <td class="style12">&nbsp;</td>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style20">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td class="auto-style18">&nbsp;</td>
                    <td class="auto-style5">&nbsp;</td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style9">&nbsp;</td>
                    <td class="style9">&nbsp;</td>
                </tr>
                <tr>
                    <td class="style7">&nbsp;</td>
                    <td colspan="3">
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="ButtonTest" runat="server" OnClick="ButtonTest_Click" CssClass="btn-raptor"
                                    Text="Begin Test" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="LabelPLCTimeout" runat="server" Font-Size="Small"
                                    ForeColor="Red" Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                <br />
                                <asp:Label ID="LabelBayError" runat="server" Font-Size="Small" ForeColor="Red"
                                    Text="Chosen Bay is not spare" Visible="False"></asp:Label>

                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ButtonTest" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="Timer2" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td class="auto-style4">&nbsp;</td>
                    <td class="auto-style18">&nbsp;</td>
                    <td class="auto-style5">&nbsp;</td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style9">&nbsp;</td>
                    <td class="style9">&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style1"></td>
                    <td class="style4" colspan="4">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="LabelStatus" runat="server"
                                    Font-Size="Medium" ForeColor="Red" Text="Board Test In Progress"
                                    Visible="False"></asp:Label>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ButtonTest" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="Timer3" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td class="auto-style17"></td>
                    <td class="auto-style6"></td>
                    <td class="auto-style11"></td>
                    <td class="auto-style8"></td>
                    <td class="style10"></td>
                </tr>

                <tr>
                    <td class="style7">&nbsp;</td>
                    <td colspan="2">&nbsp;</td>
                    <td class="auto-style20">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td class="auto-style18">&nbsp;</td>
                    <td class="auto-style5">&nbsp;</td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style9">&nbsp;</td>
                    <td class="style9">&nbsp;</td>
                </tr>

            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:SqlDataSource ID="SqlDataSourceGrades" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>" SelectCommand="select gradelabel,plcgradeid from grades,gradematrix where gradematrix.recipeid=(select recipeid from recipes where online=1)
and grades.gradeid=gradematrix.websortgradeid and WEBSortGradeID>0

order by gradelabel"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceWidth" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>" SelectCommand="select * from width where id&gt;0 order by Nominal
"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceThickness" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>" SelectCommand="select * from thickness where id>0 order by Nominal
"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceGraders" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
        SelectCommand="SELECT * FROM [Graders]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceLengths" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
        SelectCommand="select lengthlabel,ROW_NUMBER()  over (order by lengthnominal) as lengthid from lengths where LengthID>0"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceBays" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
        SelectCommand="SELECT [BinID] FROM [Bins] WHERE [BinStatus] = 0 or binstatus=5
union
select binid=bayid from gradertest
order by binid"></asp:SqlDataSource>
    <asp:Timer ID="Timer2" runat="server" Interval="1000"
        OnTick="Timer2_Tick">
    </asp:Timer>
    <asp:Timer ID="Timer3" runat="server" Interval="1000"
        OnTick="Timer3_Tick" Enabled="False">
    </asp:Timer>
</asp:Content>