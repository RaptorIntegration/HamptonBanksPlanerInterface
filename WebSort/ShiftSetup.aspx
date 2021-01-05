<%@ Page Language="C#" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="ShiftSetup.aspx.cs" Inherits="WebSort.ShiftSetup" MasterPageFile="~/WebSort.Master" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dx" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" namespace="eWorld.UI" tagprefix="ew" %>
<%@ Register assembly="DevExpress.Web.v10.2, Version=10.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxClasses" tagprefix="dx" %>
<%@ Register assembly="eWorld.UI" namespace="eWorld.UI" tagprefix="ew" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="0"
                AutoPostBack="True" SaveStateToCookies="True"
                OnActiveTabChanged="ASPxPageControl1_ActiveTabChanged" Width="950px"
                CssFilePath="~/App_Themes/BlackGlass/{0}/styles.css"
                CssPostfix="BlackGlass">
                <LoadingPanelImage Url="~/App_Themes/BlackGlass/Web/Loading.gif">
                </LoadingPanelImage>
                <ContentStyle>
                    <Border BorderColor="#4E4F51" BorderStyle="Solid" BorderWidth="1px" />
                </ContentStyle>
                <ActiveTabStyle Font-Bold="True">
                </ActiveTabStyle>
                <TabPages>
                    <dx:TabPage Text="Shift Setup">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <table style="border: thin solid #000000; width: 100%" bgcolor="#A4A4A4">
                                    <tr>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid;">&nbsp;</td>
                                        <td align="Right"
                                            style="border-color: #000000; border-bottom-style: solid; border-bottom-width: thin">&nbsp;</td>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid; padding-left: 65px;"
                                            align="left">
                                            <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="DAYS"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-color: #000000; border-bottom-style: solid; border-bottom-width: thin">&nbsp;</td>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid; padding-left: 30px;">
                                            <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="AFTERNOONS"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-color: #000000; border-bottom-style: solid; border-bottom-width: thin">&nbsp;</td>
                                        <td style="border-color: #000000; border-bottom-style: solid; border-bottom-width: thin; padding-left: 55px;">
                                            <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="NIGHTS"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid; padding-left: 10px;"
                                            valign="middle">
                                            <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="MONDAY"></asp:Label>
                                        </td>
                                        <td align="Right" colspan="1"
                                            style="border-width: thin; border-color: #000000; border-bottom-style: solid; padding-bottom: 5px;">
                                            <asp:Label ID="Label5" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label6" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td colspan="1"
                                            style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid;">
                                            <ew:TimePicker ID="TimePicker1" runat="server"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                AutoPostBack="True" PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker2" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right" colspan="1"
                                            style="border-width: thin; border-color: #000000; border-bottom-style: solid; padding-bottom: 5px; vertical-align: middle;"
                                            valign="middle">
                                            <asp:Label ID="Label25" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label26" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid;">
                                            <ew:TimePicker ID="TimePicker15" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker16" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox8" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right" colspan="1"
                                            style="border-width: thin; border-color: #000000; border-bottom-style: solid; padding-bottom: 5px;">
                                            <asp:Label ID="Label39" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label40" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; padding-bottom: 5px;">
                                            <ew:TimePicker ID="TimePicker29" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker30" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox15" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; border-bottom-style: solid; padding-left: 10px;">
                                            <asp:Label ID="Label7" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="TUESDAY"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label13" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label14" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker3" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker4" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label27" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label28" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker17" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker18" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox9" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label41" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label42" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="TimePicker31" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker32" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox16" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-left: 10px; border-right-style: solid;">
                                            <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="WEDNESDAY"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label15" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label16" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker5" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker6" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox3" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label29" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label30" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker19" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker20" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox10" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label43" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label44" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="TimePicker33" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker34" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox17" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; border-bottom-style: solid; padding-left: 10px">
                                            <asp:Label ID="Label9" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="THURSDAY"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label17" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label18" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker7" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker8" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox4" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label31" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label32" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker21" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker22" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox11" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label45" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label46" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="TimePicker35" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker36" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox18" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; border-bottom-style: solid; padding-left: 10px">
                                            <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="FRIDAY"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label19" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label20" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker9" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker10" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox5" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label33" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label34" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker23" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker24" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox12" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label47" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label48" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="TimePicker37" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker38" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox19" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; border-bottom-style: solid; padding-left: 10px; padding-left: 10px">
                                            <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="SATURDAY"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label21" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label22" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker11" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker12" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox6" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label35" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label36" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker25" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker26" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox13" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label49" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label50" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="TimePicker39" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker40" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox20" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; border-bottom-style: solid; padding-left: 10px">
                                            <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="SUNDAY"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label23" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label24" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker13" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker14" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox7" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label37" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label38" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="TimePicker27" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker28" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox14" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label51" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label52" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="TimePicker41" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="TimePicker42" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="TimePicker1_Init"
                                                OnTimeChanged="TimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="9:25 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="CheckBox21" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="CheckBox1_CheckedChanged"
                                                OnInit="CheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000;">&nbsp;</td>
                                        <td align="Right"
                                            style="border-width: thin; border-color: #000000; padding-top: 5px; padding-bottom: 5px;">&nbsp;</td>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; padding-left: 5px;">
                                            <asp:Button ID="ButtonFillDays" runat="server" Text="Fill Mon-&gt;Sun"
                                                OnClick="ButtonFillDays_Click" />
                                        </td>
                                        <td align="Right"
                                            style="border-width: thin; border-color: #000000; padding-top: 5px; padding-bottom: 5px;">&nbsp;</td>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; padding-left: 5px;">
                                            <asp:Button ID="ButtonFillAfternoons" runat="server" Text="Fill Mon-&gt;Sun"
                                                OnClick="ButtonFillAfternoons_Click" />
                                        </td>
                                        <td align="Right"
                                            style="border-width: thin; border-color: #000000; padding-top: 5px; padding-bottom: 5px;">&nbsp;</td>
                                        <td style="padding-top: 5px; padding-bottom: 5px; padding-left: 5px;">
                                            <asp:Button ID="ButtonFillNights" runat="server" Text="Fill Mon-&gt;Sun"
                                                OnClick="ButtonFillNights_Click" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table style="width: 100%;">
                                    <tr>
                                        <td class="style1">
                                            <asp:Button ID="ButtonManualShiftEnd" runat="server"
                                                OnClick="ButtonManualShiftEnd_Click" Text="Manual Shift End"
                                                Width="143px" />
                                            <asp:Label ID="Label53" runat="server" Font-Size="Small" ForeColor="#CC3300"
                                                Text="Warning: Ending the shift will clear the current production counts and print reports."></asp:Label>
                                        </td>
                                        <td>&nbsp;</td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="style1">
                                            <asp:CheckBox ID="CheckBoxAutoIncrement" runat="server"
                                                Text="Automatically Increment Shifts" AutoPostBack="True"
                                                OnCheckedChanged="CheckBoxAutoIncrement_CheckedChanged" Font-Size="Small" ForeColor="Black" />
                                        </td>
                                        <td>&nbsp;</td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </table>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                    <dx:TabPage Text="Shift Breaks">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <table style="border: thin solid #000000; width: 100%" bgcolor="#A4A4A4">
                                    <tr>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid;">&nbsp;</td>
                                        <td align="Right"
                                            style="border-color: #000000; border-bottom-style: solid; border-bottom-width: thin">&nbsp;</td>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid; padding-left: 65px;"
                                            align="left">
                                            <asp:Label ID="Label54" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="DAYS"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-color: #000000; border-bottom-style: solid; border-bottom-width: thin">&nbsp;</td>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid; padding-left: 30px;">
                                            <asp:Label ID="Label55" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="AFTERNOONS"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-color: #000000; border-bottom-style: solid; border-bottom-width: thin">&nbsp;</td>
                                        <td style="border-color: #000000; border-bottom-style: solid; border-bottom-width: thin; padding-left: 55px;">
                                            <asp:Label ID="Label56" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="NIGHTS"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid; padding-left: 10px;"
                                            valign="middle">
                                            <asp:Label ID="Label57" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="BREAK #1"></asp:Label>
                                        </td>
                                        <td align="Right" colspan="1"
                                            style="border-width: thin; border-color: #000000; border-bottom-style: solid; padding-bottom: 5px;">
                                            <asp:Label ID="Label58" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label59" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td colspan="1"
                                            style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid;">
                                            <ew:TimePicker ID="BreakTimePicker1" runat="server"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                AutoPostBack="True" PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker2" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False" OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged"
                                                RoundUpMinutes="False" Width="100px" PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox1" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right" colspan="1"
                                            style="border-width: thin; border-color: #000000; border-bottom-style: solid; padding-bottom: 5px; vertical-align: middle;"
                                            valign="middle">
                                            <asp:Label ID="Label60" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label61" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; border-right-style: solid;">
                                            <ew:TimePicker ID="BreakTimePicker9" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker10" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox5" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right" colspan="1"
                                            style="border-width: thin; border-color: #000000; border-bottom-style: solid; padding-bottom: 5px;">
                                            <asp:Label ID="Label62" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label63" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-width: thin; border-color: #000000; border-bottom-style: solid; padding-bottom: 5px;">
                                            <ew:TimePicker ID="BreakTimePicker17" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker18" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox9" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; border-bottom-style: solid; padding-left: 10px;">
                                            <asp:Label ID="Label64" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="BREAK #2"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label65" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label66" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="BreakTimePicker3" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False" OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged"
                                                RoundUpMinutes="False" Width="100px" PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker4" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False" OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged"
                                                RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox2" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label67" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label68" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="BreakTimePicker11" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker12" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox6" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label69" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label70" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="BreakTimePicker19" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker20" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox10" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-left: 10px; border-right-style: solid;">
                                            <asp:Label ID="Label71" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="BREAK #3"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label72" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label73" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="BreakTimePicker5" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker6" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox3" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label74" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label75" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="BreakTimePicker13" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker14" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox7" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label76" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label77" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="BreakTimePicker21" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker22" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox11" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; border-bottom-style: solid; padding-left: 10px">
                                            <asp:Label ID="Label9a" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Text="BREAK #4"></asp:Label>
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label17a" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label18a" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="BreakTimePicker7" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker8" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox4" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label31a" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label32a" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-right-style: solid; border-bottom-style: solid; border-width: thin; border-color: #000000">
                                            <ew:TimePicker ID="BreakTimePicker15" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker16" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox8" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                        <td align="Right"
                                            style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <asp:Label ID="Label45a" runat="server" Font-Size="Small" Text="Start:"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label46a" runat="server" Font-Size="Small" Text="End:"></asp:Label>
                                        </td>
                                        <td style="border-bottom-style: solid; border-width: thin; border-color: #000000; padding-bottom: 5px">
                                            <ew:TimePicker ID="BreakTimePicker23" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <br />
                                            <ew:TimePicker ID="BreakTimePicker24" runat="server" AutoPostBack="True"
                                                DisabledEntryDisplayAction="OnButtonClick" DisableTextBoxEntry="False"
                                                OnInit="BreakTimePicker1_Init"
                                                OnTimeChanged="BreakTimePicker1_TimeChanged" RoundUpMinutes="False" Width="100px"
                                                PostedTime="10:06 PM">
                                                <ButtonStyle Height="20px" />
                                            </ew:TimePicker>
                                            <asp:CheckBox ID="BreakCheckBox12" runat="server" AutoPostBack="True" Checked="True"
                                                Font-Size="Small" OnCheckedChanged="BreakCheckBox1_CheckedChanged"
                                                OnInit="BreakCheckBox1_Init" Text="Enabled" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000;">&nbsp;</td>
                                        <td align="Right"
                                            style="border-width: thin; border-color: #000000; padding-top: 5px; padding-bottom: 5px;">&nbsp;</td>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; padding-left: 5px;">&nbsp;</td>
                                        <td align="Right"
                                            style="border-width: thin; border-color: #000000; padding-top: 5px; padding-bottom: 5px;">&nbsp;</td>
                                        <td style="border-right-style: solid; border-width: thin; border-color: #000000; padding-left: 5px;">&nbsp;</td>
                                        <td align="Right"
                                            style="border-width: thin; border-color: #000000; padding-top: 5px; padding-bottom: 5px;">&nbsp;</td>
                                        <td style="padding-top: 5px; padding-bottom: 5px; padding-left: 5px;">&nbsp;</td>
                                    </tr>
                                </table>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                    <dx:TabPage Text="Shift Log">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="LabelReportLog0" runat="server" Font-Size="Small"
                                            ForeColor="Black" Text="RaptorShiftMaster Service:"></asp:Label>
                                        &nbsp;&nbsp;
                                        <asp:Label ID="LabelServiceStatus" runat="server" Font-Size="Small"
                                            ForeColor="Black"></asp:Label>
                                        <br />
                                        <asp:Button ID="ButtonStart" runat="server" OnClick="ButtonStart_Click"
                                            Text="Start" Width="70px" />
                                        <asp:Button ID="ButtonStop" runat="server" OnClick="ButtonStop_Click"
                                            Text="Stop" Width="70px" />
                                        <br />
                                        <asp:Timer ID="Timer3" runat="server" Enabled="False" Interval="1500"
                                            OnTick="Timer3_Tick">
                                        </asp:Timer>
                                        <asp:Label ID="LabelReportLog" runat="server" Font-Size="Small"
                                            ForeColor="Black" Text="Auto Reporting Log:"></asp:Label>
                                        <br />
                                        <asp:ListBox ID="ListBox1" runat="server" DataSourceID="SqlDataSourceLog"
                                            DataTextField="Text" DataValueField="Text" Rows="50"></asp:ListBox>
                                        <asp:SqlDataSource ID="SqlDataSourceLog" runat="server"
                                            ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                                            SelectCommand="SELECT 'text'=convert(varchar(175),convert(varchar,timestamp,9) + '     ---        ' + text) FROM [RaptorShiftMasterLog] where id&gt;(select max(id)-50 from RaptorShiftMasterLog) order by id desc"></asp:SqlDataSource>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="Timer3" EventName="Tick" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                </TabPages>
            </dx:ASPxPageControl>
        </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>