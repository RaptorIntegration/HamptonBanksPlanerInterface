<%@ Page Language="C#" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" MasterPageFile="~/WebSort.Master" CodeBehind="GraderBoardTest.aspx.cs" Inherits="WebSort.GraderBoardTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .style1 {
            width: 145px;
        }

        .style2 {
            width: 172px;
        }

        .style4 {
            height: 16px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table style="width: 120%;">
                <tr>
                    <td class="style1" valign="top">

                        <asp:Label ID="Label1" runat="server" Font-Size="Small" ForeColor="Black"
                            Text="Choose Test Grader(s):"></asp:Label>
                    </td>
                    <td class="style2" valign="top">
                        <asp:CheckBoxList ID="CheckBoxList1" runat="server" AppendDataBoundItems="True"
                            DataSourceID="SqlDataSourceGraders" DataTextField="GraderDescription"
                            DataValueField="GraderID" Font-Size="Small" ForeColor="Black"
                            OnSelectedIndexChanged="CheckBoxList1_SelectedIndexChanged" Width="182px"
                            RepeatColumns="2">
                            <asp:ListItem Value="0">All</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                    <td valign="top" colspan="2">&nbsp;&nbsp;</td>
                    <td align="left" class="style1" valign="top">
                        <asp:Label ID="Label2" runat="server" Font-Size="Small" ForeColor="Black"
                            Text="Choose Test Grade(s):"></asp:Label>
                    </td>
                    <td valign="top">
                        <asp:CheckBoxList ID="CheckBoxList2" runat="server" AppendDataBoundItems="True"
                            DataSourceID="SqlDataSourceGrades" DataTextField="gradelabel"
                            DataValueField="plcgradeid" Font-Size="Small" ForeColor="Black" Height="24px"
                            RepeatColumns="2" Width="182px">
                            <asp:ListItem Value="0">All</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="style1" valign="top">&nbsp;&nbsp;</td>
                    <td class="style2">&nbsp;</td>
                    <td colspan="2">&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="style1" valign="top">
                        <asp:Label ID="Label4" runat="server" Font-Size="Small" ForeColor="Black"
                            Text="Choose Sample Size:"></asp:Label>
                    </td>
                    <td class="style2" valign="top">
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TextBox ID="TextBoxSampleSize" runat="server"
                                    OnTextChanged="TextBoxSampleSize_TextChanged" Width="70px">1</asp:TextBox>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td valign="top" colspan="2">&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label5" runat="server" Font-Size="Small" ForeColor="Black"
                            Text="Choose Interval:"></asp:Label>
                    </td>
                    <td class="style2">
                        <asp:TextBox ID="TextBoxInterval" runat="server" Width="70px">1</asp:TextBox>
                    </td>
                    <td colspan="2">&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label3" runat="server" Font-Size="Small" ForeColor="Black"
                            Text="Choose Test Bay:"></asp:Label>
                    </td>
                    <td class="style2">
                        <asp:DropDownList ID="DropDownListBay" runat="server"
                            DataSourceID="SqlDataSourceBays" DataTextField="BinID"
                            DataValueField="BinID" Width="70px">
                        </asp:DropDownList>
                        <asp:Label ID="LabelBayError0" runat="server" Font-Size="Small" ForeColor="Red"
                            Text="Choose a Bay" Visible="False"></asp:Label>
                    </td>
                    <td colspan="2">&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label6" runat="server" Font-Size="Small" ForeColor="Black"
                            Text="Choose Options:"></asp:Label>
                    </td>
                    <td class="style2">
                        <asp:CheckBox ID="CheckBoxStamp" runat="server" Checked="True"
                            Font-Size="Small" ForeColor="Black" Text="Stamp" />
                        <asp:CheckBox ID="CheckBoxTrim" runat="server" Checked="True" Font-Size="Small"
                            ForeColor="Black" Text="Trim" />
                    </td>
                    <td colspan="2">&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="style1">&nbsp;&nbsp;</td>
                    <td class="style2">&nbsp;</td>
                    <td colspan="2">&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="style1">&nbsp;</td>
                    <td colspan="3">
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="ButtonTest" runat="server" OnClick="ButtonTest_Click"
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
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="style1"></td>
                    <td class="style4" colspan="4">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="LabelStatus" runat="server"
                                    Font-Size="Medium" ForeColor="Red" Text="Grader Board Test In Progress"
                                    Visible="False"></asp:Label>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ButtonTest" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="Timer3" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td></td>
                </tr>

                <tr>
                    <td class="style1">&nbsp;</td>
                    <td colspan="2">&nbsp;</td>
                    <td colspan="2">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:SqlDataSource ID="SqlDataSourceGrades" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>" SelectCommand="select gradelabel,plcgradeid from grades,gradematrix where recipeid=(select recipeid from recipes where online=1)
and grades.gradeid=gradematrix.websortgradeid and WEBSortGradeID&gt;0
order by gradelabel"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceGraders" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
        SelectCommand="SELECT * FROM [Graders]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceBays" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
        SelectCommand="SELECT [BinID] FROM [Bins] WHERE [BinStatus] = 0
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