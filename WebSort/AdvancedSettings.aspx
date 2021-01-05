<%@ Page Language="C#" MasterPageFile="~/WebSort.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="AdvancedSettings.aspx.cs" Inherits="WebSort.AdvancedSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="border: thin solid #000000; width: 100%;">
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server"
                    Text="Setup Mode:" Font-Bold="True"></asp:Label>
            </td>
            <td>
                <asp:RadioButton ID="RadioButtonOnline" runat="server" 
                    GroupName="1" Text="Online"
                    OnCheckedChanged="RadioButtonOnline_CheckedChanged" />
                <asp:RadioButton ID="RadioButtonOffline" runat="server" 
                    GroupName="1" Text="Offline"
                    OnCheckedChanged="RadioButtonOffline_CheckedChanged" />
            </td>
            <td>
                <asp:Label ID="Label2" runat="server" class="severity-2"
                    Text="Warning: any changes made in offline mode will not be reflected in the PLC"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <br />
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label4" runat="server"  class="severity-2"
                    Text="Warning: All current sorting parameters in the PLC will be overwritten!"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <contenttemplate>
                        <asp:Button ID="ButtonSendBins" runat="server" Text="Send All Bay &amp; Sort Data"
                            CssClass="btn-raptor" OnClick="ButtonSendBins_Click" />
                        <asp:Button ID="ButtonSendProducts" runat="server" Text="Send All Product Data"
                            CssClass="btn-raptor" OnClick="ButtonSendProducts_Click" />
                        <asp:Button ID="ButtonSendDrives" runat="server"
                            OnClick="ButtonSendDrives_Click" Text="Send All Drive Data" CssClass="btn-raptor" />
                        <asp:Button ID="ButtonSendTiming" runat="server"
                            OnClick="ButtonSendTiming_Click" Text="Send All Timing Data"
                            CssClass="btn-raptor" />
                        <br />
                        <asp:Label ID="LabelTimeout" runat="server" 
                            ForeColor="#CC3300" Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server"
                            AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                Writing Data, Please Wait...
                    <br />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </contenttemplate>
                </asp:UpdatePanel>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <br />
    <br />
    <table style="border: thin solid #000000; width: 100%;">
        <tr>
            <td>
                <asp:Label ID="Label6" runat="server"  
                    Text="To recover loss of WEBSort data:"
                    Font-Bold="True"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label9" runat="server"  class="severity-2"
                    Text="Warning: All current sorting parameters in WEBSort database will be overwritten!"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <contenttemplate>
                        <asp:Button ID="ButtonReadBins" runat="server" Text="Read All Bay &amp; Sort Data"
                            CssClass="btn-raptor" OnClick="ButtonReadBins_Click" />
                        <asp:Button ID="ButtonReadProducts" runat="server" Text="Read All Product Data"
                            CssClass="btn-raptor" OnClick="ButtonReadProducts_Click" />
                        <asp:Button ID="ButtonReadDrives" runat="server"
                            OnClick="ButtonReadDrives_Click" Text="Read All Drive Data"
                            CssClass="btn-raptor" />
                        <asp:Button ID="ButtonReadTiming" runat="server"
                            OnClick="ButtonReadTiming_Click" Text="Read All Timing Data"
                            CssClass="btn-raptor" />
                        <asp:Button ID="ButtonReadPLCProduction"
                            runat="server" OnClick="ButtonReadPLCProduction_Click"
                            Text="Read Production Data" CssClass="btn-raptor" />
                        <br />
                        <asp:Label ID="LabelTimeout1" runat="server" 
                            ForeColor="#CC3300" Text="Timeout Communicating with PLC"
                            Visible="False"></asp:Label>
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server"
                            AssociatedUpdatePanelID="UpdatePanel2">
                            <ProgressTemplate>
                                Reading Data, Please Wait...
                    <br />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </contenttemplate>
                </asp:UpdatePanel>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <br />
    <br />

    <table style="border: thin solid #000000; width: 100%;">
        <tr>
            <td>
                <asp:Label ID="Label5" runat="server"  
                    Text="Backup the WEBSort database:" Font-Bold="True"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="LabelBackup0" runat="server"  >Enter a Password to associate with the Backup File:</asp:Label>
                <asp:TextBox ID="TextBoxPassword" runat="server"
                    TextMode="Password">
                </asp:TextBox>
                <br />
                <asp:Button ID="ButtonBackup" runat="server" Text="Backup Database"
                   CssClass="btn-raptor" OnClick="ButtonBackup_Click" />
                <asp:Label ID="LabelBackup" runat="server"  ></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <br />
                <asp:Label ID="Label7" runat="server"  
                    Text="Restore the WEBSort database:" Font-Bold="True"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="ButtonReplace" runat="server" Text="Restore Database"
                    CssClass="btn-raptor" OnClick="ButtonReplace_Click" />
                <asp:Label ID="Label8" runat="server"  ForeColor="#CC3300"
                    Text="Database restoring must be performed outside of this user interface. Please launch the WEBSort Database Restore program."
                    Visible="False"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:SqlDataSource ID="SqlDataSourceBackups" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DatabaseBackupHistory] ORDER BY [ID] DESC"></asp:SqlDataSource>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <br />
</asp:Content>