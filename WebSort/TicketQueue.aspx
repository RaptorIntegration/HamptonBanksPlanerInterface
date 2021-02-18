<%@ Page Language="C#" MaintainScrollPositionOnPostback="true"  EnableEventValidation="false"  ViewStateEncryptionMode="Never" StyleSheetTheme="Theme" MasterPageFile="~/WebSort.Master" AutoEventWireup="true" CodeBehind="ticketqueue.aspx.cs" Inherits="industrial.ticketqueue" %>


<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
    <ContentTemplate>
        <asp:CheckBox ID="CheckBoxAutoIncrement" runat="server" AutoPostBack="True" visible ="false"
            Font-Bold="True" Font-Size="X-Large" ForeColor="Black" 
            oncheckedchanged="CheckBoxAutoIncrement_CheckedChanged" 
            Text="Automatic Printing" />
        <br />
        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="GridViewPreviousData" runat="server" AllowSorting="True" 
                    AutoGenerateColumns="False" CaptionAlign="Left" 
                    DataKeyNames="Packagenumber" 
                    DataSourceID="SqlDataSource2" 
                    onrowcancelingedit="GridViewPreviousData_RowCancelingEdit" 
                    onrowcommand="GridViewPreviousData_RowCommand" 
                    onrowediting="GridViewPreviousData_RowEditing" 
                    onrowupdated="GridViewPreviousData_RowUpdated" 
                    onrowupdating="GridViewPreviousData_RowUpdating" SkinID="gridviewSkin" Width="1124px">
                    <RowStyle Font-Size="Large" Wrap="True" />
                    <Columns>
                        <asp:ButtonField ButtonType="Button" CommandName="Print" Text="Print">
                            <ControlStyle Font-Bold="True" Font-Size="Large" Height="60px" Width="200px" />
                        </asp:ButtonField>
                        <asp:ButtonField ButtonType="Button" CommandName="Remove" Text="Delete">
                            <ControlStyle Font-Bold="True" Font-Size="Large" Height="60px" Width="125px" />
                        </asp:ButtonField>
                        
                        
                        <asp:BoundField DataField="Packagenumber" HeaderText="Pack #" 
                            ReadOnly="True" SortExpression="Packagenumber" />
                        
                        <asp:BoundField DataField="TimeStampReset" HeaderText="Time" 
                            SortExpression="TimeStampReset" />
                        <asp:BoundField DataField="PackageLabel" HeaderText="Package Label" 
                            SortExpression="PackageLabel" Visible="true" />
                        <asp:BoundField DataField="PackageCount" HeaderText="Count" 
                            SortExpression="PackageCount" />                        
                        
                        
                        <asp:BoundField DataField="ticketprinted" HeaderText="ticketprinted" 
                            SortExpression="ticketprinted" Visible="false" />
                    </Columns>
                    <PagerStyle BackColor="#FFCC66" Font-Bold="False" Font-Italic="False" 
                        Font-Size="Small" ForeColor="#333333" HorizontalAlign="Center" />
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                    <EditRowStyle Font-Size="Medium" />
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Timer2" EventName="Tick" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
            ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>" 
            SelectCommand="selectTicketQueue" SelectCommandType="StoredProcedure">
        </asp:SqlDataSource>
    </ContentTemplate>
    </asp:UpdatePanel>

    
    <asp:Timer ID="Timer2" runat="server" Interval="30000" ontick="Timer2_Tick">
    </asp:Timer>

</asp:Content>