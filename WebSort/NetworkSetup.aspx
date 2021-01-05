<%@ Page Language="C#" MaintainScrollPositionOnPostback="true" MasterPageFile="~/WebSort.Master" AutoEventWireup="true" CodeBehind="NetworkSetup.aspx.cs" Inherits="WebSort.NetworkSetup" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1 {
            width: 169px;
        }

        .style2 {
            width: 259px;
        }

        .style3 {
            width: 108px;
        }

        .auto-style1 {
            width: 350px;
        }

        .auto-style2 {
            width: 394px;
        }

        .auto-style3 {
            width: 265px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="width: 100%;">
        <tr>
            <td class="auto-style3">

                <asp:Label ID="Label1" runat="server" Font-Size="Small" 
                    Text="PLC IP Address:"></asp:Label>
            </td>
            <td class="style2">
                <asp:TextBox ID="TextBoxIP" runat="server" CausesValidation="True" Width="137px"></asp:TextBox>
            </td>
            <td class="auto-style1">

                <asp:Label ID="Label6" runat="server" Font-Size="Small" 
                    Text="PLC LED State:"></asp:Label>
            </td>
            <td class="auto-style2">

                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="LabelPLCLED" runat="server" Font-Size="Small"
                            ></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="auto-style3">
                <asp:Label ID="Label2" runat="server" Font-Size="Small" 
                    Text="PLC Processor Slot:"></asp:Label>
            </td>
            <td class="style2">
                <asp:TextBox ID="TextBoxSlot" runat="server" CausesValidation="True" Width="137px"></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator2" runat="server"
                    ControlToValidate="TextBoxSlot" Display="Dynamic"
                    ErrorMessage="Entry Out of Range" MaximumValue="25" MinimumValue="0"
                    Type="Integer"></asp:RangeValidator>
            </td>
            <td class="auto-style1">

                <asp:Label ID="Label7" runat="server" Font-Size="Small" 
                    Text="PLC Fault State:"></asp:Label>
            </td>
            <td class="auto-style2">

                <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="LabelPLCFault" runat="server" Font-Size="Small"
                            ></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="auto-style3">
                <asp:Label ID="Label3" runat="server" Font-Size="Small" 
                    Text="PLC Timeout(ms):"></asp:Label>
            </td>
            <td class="style2">
                <asp:TextBox ID="TextBoxTimeout" runat="server" CausesValidation="True" Width="137px"></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator1" runat="server"
                    ControlToValidate="TextBoxTimeout" Display="Dynamic"
                    ErrorMessage="Entry Out of Range" MaximumValue="30000" MinimumValue="1000"
                    Type="Integer"></asp:RangeValidator>
            </td>
            <td class="auto-style1">

                <asp:Label ID="Label8" runat="server" Font-Size="Small" 
                    Text="PLC Keyswitch:"></asp:Label>
            </td>
            <td class="auto-style2">

                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="LabelPLCKeySwitch" runat="server" Font-Size="Small"
                            ></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="auto-style3">&nbsp;</td>
            <td class="style2">
                <asp:Button ID="ButtonSave" runat="server" OnClick="ButtonSave_Click"
                    Text="Save" CssClass="btn-raptor" UseSubmitBehavior="False" />
            </td>
            <td class="auto-style1">&nbsp;</td>
            <td class="auto-style2">&nbsp;</td>
        </tr>

        <tr>
            <td class="auto-style3" valign="top">
                <asp:Label ID="Label5" runat="server" Font-Size="Small" 
                    Text="RaptorComm Service:"></asp:Label>
            </td>
            <td class="style2">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="LabelServiceStatus" runat="server" Font-Size="Small"
                            ></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <br />
                        <asp:Button ID="ButtonStart" runat="server" OnClick="ButtonStart_Click"
                            Text="Start" CssClass="btn-raptor" />
                        <asp:Button ID="ButtonStop" runat="server" OnClick="ButtonStop_Click"
                            Text="Stop" CssClass="btn-raptor" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="Timer3" EventName="Tick" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td class="auto-style1" valign="top">&nbsp;</td>
            <td class="auto-style2">&nbsp;</td>
        </tr>
    </table>

    <table style="width: 100%;">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="Label4" runat="server" Font-Size="Small" 
                            Text="WEBSort - PLC Communication Log:"></asp:Label>
                        <br />
                        <asp:Label ID="Label9" runat="server" Font-Size="Small" 
                            Text="Poll Counter:"></asp:Label>
                        <asp:Label ID="LabelPollCounter" runat="server" Font-Size="Small"
                             Text="0"></asp:Label>
                        <br />
                        <asp:ListBox ID="ListBox1" runat="server"
                            DataSourceID="SqlDataSourceRaptorCommLog" DataTextField="Text" style="background-color: var(--bg-primary); color: var(--text-primary); border: none; padding: 1rem; box-shadow: inset 0 0 10px 10px var(--shadow)"
                            DataValueField="text" Rows="50"></asp:ListBox>
                        <asp:SqlDataSource ID="SqlDataSourceRaptorCommLog" runat="server"
                            ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                            SelectCommand="SELECT 'text'=convert(varchar,timestamp,9) + '     ---        ' + text FROM [RaptorCommLog] where id>(select max(id)-50 from raptorcommlog) order by id desc"></asp:SqlDataSource>
                        <asp:Timer ID="Timer2" runat="server" Interval="1000" OnTick="Timer2_Tick">
                        </asp:Timer>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="Timer2" EventName="Tick" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Timer ID="Timer3" runat="server" Interval="1000" OnTick="Timer3_Tick">
                </asp:Timer>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <br />
    <br />
</asp:Content>