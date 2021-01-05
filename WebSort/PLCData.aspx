<%@ Page Language="C#" MasterPageFile="~/WebSort.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="PLCData.aspx.cs" Inherits="WebSort.PLCData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language='javascript'>
        function openwindow(pagename) {
            window.open(pagename, "PLC");
        }
        
    </script>
    <style>
        .plc tr td a, .plc tr th a{
            text-decoration: none;
        }
        .plc tr:first-child, .plc tr:first-child a {
            background-color: #1b3665;
            color: #b6b6b6;
        }
        .plc tr:first-child:hover, .plc tr:first-child a {
            background-color: #1b3665;
            color: #b6b6b6;
        }
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="">
        <tr>
            <td align="center">
                <asp:Label ID="Label1" runat="server" 
                    Text="Enter PLC Tag Name:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="TextBox1" runat="server" Width="200px"></asp:TextBox>
            </td>
            <td>
                <asp:RadioButton ID="RadioButtonTypeInt" runat="server" Checked="True"
                     GroupName="1" Text="Integer" />
                <asp:RadioButton ID="RadioButtonTypeBool" runat="server" 
                    GroupName="1" Text="Boolean" />
                <asp:RadioButton ID="RadioButtonTypeString" runat="server" 
                    GroupName="1" Text="String" />
                <asp:RadioButton ID="RadioButtonTypeReal" runat="server" 
                    GroupName="1" Text="Real" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="ButtonRead" runat="server" OnClick="ButtonRead_Click"
                    Text="Read" CssClass="btn-raptor" />
                <asp:Button ID="ButtonWrite" runat="server" OnClick="ButtonWrite_Click"
                    Text="Write" CssClass="btn-raptor" />
            </td>
            <td>
                <asp:TextBox ID="TextBoxWrite" runat="server" Width="200px"></asp:TextBox>
                <br />
                <asp:Label ID="LabelError" runat="server"></asp:Label>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="border-bottom-style: solid; border-width: thin; border-color: #000000">&nbsp;&nbsp; &nbsp;</td>
            <td style="border-bottom-style: solid; border-width: thin; border-color: #000000">&nbsp;</td>
            <td style="border-bottom-style: solid; border-width: thin; border-color: #000000">&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="vertical-align: top" align="left">
                <asp:Button ID="Button1" runat="server"
                    Text="View PLC Home Page" OnClick="Button1_Click1" CssClass="btn-raptor" />
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="vertical-align: top">&nbsp;&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="vertical-align: top">
                <asp:Label ID="Label2" runat="server" 
                    Text="PLC / WEBSort Data Type(s):"></asp:Label>
            </td>
            <td>
                <asp:CheckBoxList ID="CheckBoxList1" runat="server" 
                    RepeatColumns="4"
                    OnSelectedIndexChanged="CheckBoxList1_SelectedIndexChanged">
                    <asp:ListItem Selected="True">Lug</asp:ListItem>
                    <asp:ListItem>Bay</asp:ListItem>
                    <asp:ListItem>Sort</asp:ListItem>
                    <asp:ListItem>Alarm</asp:ListItem>
                    <asp:ListItem>Product</asp:ListItem>
                    <asp:ListItem>Grade</asp:ListItem>
                    <asp:ListItem>Grader Test</asp:ListItem>
                    <asp:ListItem>MSR Samples</asp:ListItem>
                    <asp:ListItem>Moisture</asp:ListItem>
                    <asp:ListItem>Thickness</asp:ListItem>
                    <asp:ListItem>Width</asp:ListItem>
                    <asp:ListItem>Length</asp:ListItem>
                    <asp:ListItem>PET Length</asp:ListItem>
                    <asp:ListItem>Timing</asp:ListItem>
                    <asp:ListItem>Diagnostics</asp:ListItem>
                    <asp:ListItem>Drives</asp:ListItem>
                </asp:CheckBoxList>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="vertical-align: top">&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <asp:Timer ID="Timer2" runat="server" Interval="2000" OnTick="Timer2_Tick">
    </asp:Timer>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="PanelLug" runat="server">

                <asp:Button ID="ButtonPause" runat="server" OnClick="ButtonPause_Click"
                    Text="Pause" CssClass="btn-raptor" />

                <asp:GridView ID="GridViewLug" runat="server" AllowPaging="True"
                    AllowSorting="True" 
                    AutoGenerateColumns="False" 
                    DataKeyNames="ID"
                    Caption="Lug Data Received" 
                    CaptionAlign="Left"
                    DataSourceID="SqlDataSourceLugDataReceived"
                    CssClass="table plc">
                    <RowStyle Wrap="False" />
                    <PagerStyle HorizontalAlign="Center" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="FrameStart" HeaderText="FrameStart"
                            SortExpression="FrameStart" />
                        <asp:BoundField DataField="LugNum" HeaderText="LugNum"
                            SortExpression="LugNum" />
                        <asp:BoundField DataField="TrackNum" HeaderText="TrackNum"
                            SortExpression="TrackNum" />
                        <asp:BoundField DataField="BayNum" HeaderText="BayNum"
                            SortExpression="BayNum" />
                        <asp:BoundField DataField="ProductID" HeaderText="ProductID"
                            SortExpression="ProductID" />
                        <asp:BoundField DataField="LengthID" HeaderText="LengthID"
                            SortExpression="LengthID" />
                        <asp:BoundField DataField="ThickActual" HeaderText="ThickActual"
                            SortExpression="ThickActual" />
                        <asp:BoundField DataField="WidthActual" HeaderText="WidthActual"
                            SortExpression="WidthActual" />
                        <asp:BoundField DataField="LengthIn" HeaderText="LengthIn"
                            SortExpression="LengthIn" />
                        <asp:BoundField DataField="Fence" HeaderText="Fence" SortExpression="Fence" />
                        <asp:BoundField DataField="Saws" HeaderText="Saws" SortExpression="Saws" />
                        <asp:BoundField DataField="NET" HeaderText="NET" SortExpression="NET" />
                        <asp:BoundField DataField="FET" HeaderText="FET" SortExpression="FET" />
                        <asp:BoundField DataField="CN2" HeaderText="CN2" SortExpression="CN2" />
                        <asp:BoundField DataField="BayCount" HeaderText="BayCount"
                            SortExpression="BayCount" />
                        <asp:BoundField DataField="Volume" HeaderText="Volume"
                            SortExpression="Volume" />
                        <asp:BoundField DataField="PieceCount" HeaderText="PieceCount"
                            SortExpression="PieceCount" />
                        <asp:BoundField DataField="Flags" HeaderText="Flags" SortExpression="Flags" />
                        <asp:BoundField DataField="Devices" HeaderText="Devices"
                            SortExpression="Devices" />
                        <asp:BoundField DataField="Ack" HeaderText="Ack" SortExpression="Ack" />
                        <asp:BoundField DataField="FrameEnd" HeaderText="FrameEnd"
                            SortExpression="FrameEnd" />
                    </Columns>
                    <HeaderStyle Wrap="False" Font-Bold="True" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceLugDataReceived" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataReceivedLug] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewBinReceived" runat="server" AllowPaging="True"
                    AllowSorting="True" 
                    AutoGenerateColumns="False" 
                    Caption="Bay Data Received"
                    CaptionAlign="Left" 
                    DataKeyNames="ID"
                    DataSourceID="SqlDataSourceBinDataReceived"
                    CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="FrameStart" HeaderText="FrameStart"
                            SortExpression="FrameStart" />
                        <asp:BoundField DataField="BayNum" HeaderText="BayNum"
                            SortExpression="BayNum" />
                        <asp:BoundField DataField="Count" HeaderText="Count" SortExpression="Count" />
                        <asp:BoundField DataField="SortXRef" HeaderText="SortXRef"
                            SortExpression="SortXRef" />
                        <asp:BoundField DataField="Status" HeaderText="Status"
                            SortExpression="Status" />
                        <asp:CheckBoxField DataField="RdmWidthFlag" HeaderText="RdmWidthFlag"
                            SortExpression="RdmWidthFlag" />
                        <asp:CheckBoxField DataField="TrimFlag" HeaderText="TrimFlag"
                            SortExpression="TrimFlag" />
                        <asp:CheckBoxField DataField="Ack" HeaderText="Ack" SortExpression="Ack" />
                        <asp:BoundField DataField="FrameEnd" HeaderText="FrameEnd"
                            SortExpression="FrameEnd" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceBinDataReceived" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataReceivedBin] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewBinSent" runat="server" 
                    AllowPaging="True"
                    AllowSorting="True" 
                    AutoGenerateColumns="False"
                    Caption="Bay Data Read/Write"
                    CaptionAlign="Left" 
                    DataKeyNames="ID" 
                    DataSourceID="SqlDataSourceBinDataSent"
                    CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="BinID" HeaderText="BinID" SortExpression="BinID" />
                        <asp:BoundField DataField="BinLabel" HeaderText="BinLabel"
                            SortExpression="BinLabel" />
                        <asp:BoundField DataField="BinStatus" HeaderText="BinStatus"
                            SortExpression="BinStatus" />
                        <asp:BoundField DataField="BinSize" HeaderText="BinSize"
                            SortExpression="BinSize" />
                        <asp:BoundField DataField="BinCount" HeaderText="BinCount"
                            SortExpression="BinCount" />
                        <asp:BoundField DataField="ProductMap0" HeaderText="ProductMap0"
                            SortExpression="ProductMap0" />
                        <asp:BoundField DataField="ProductMap1" HeaderText="ProductMap1"
                            SortExpression="ProductMap1" />
                        <asp:BoundField DataField="ProductMap2" HeaderText="ProductMap2"
                            SortExpression="ProductMap2" />
                        <asp:BoundField DataField="ProductMap3" HeaderText="ProductMap3"
                            SortExpression="ProductMap3" />
                        <asp:BoundField DataField="ProductMap4" HeaderText="ProductMap4"
                            SortExpression="ProductMap4" />
                        <asp:BoundField DataField="ProductMap5" HeaderText="ProductMap5"
                            SortExpression="ProductMap5" />
                        <asp:BoundField DataField="LengthMap" HeaderText="LengthMap"
                            SortExpression="LengthMap" />
                        <asp:BoundField DataField="ProductMap0Old" HeaderText="ProductMap0Old"
                            SortExpression="ProductMap0Old" />
                        <asp:BoundField DataField="ProductMap1Old" HeaderText="ProductMap1Old"
                            SortExpression="ProductMap1Old" />
                        <asp:BoundField DataField="ProductMap2Old" HeaderText="ProductMap2Old"
                            SortExpression="ProductMap2Old" />
                        <asp:BoundField DataField="ProductMap3Old" HeaderText="ProductMap3Old"
                            SortExpression="ProductMap3Old" />
                        <asp:BoundField DataField="ProductMap4Old" HeaderText="ProductMap4Old"
                            SortExpression="ProductMap4Old" />
                        <asp:BoundField DataField="ProductMap5Old" HeaderText="ProductMap5Old"
                            SortExpression="ProductMap5Old" />
                        <asp:BoundField DataField="BinStamps" HeaderText="BinStamps"
                            SortExpression="BinStamps" />
                        <asp:BoundField DataField="BinSprays" HeaderText="BinSprays"
                            SortExpression="BinSprays" />
                        <asp:BoundField DataField="SortID" HeaderText="SortID"
                            SortExpression="SortID" />
                        <asp:CheckBoxField DataField="TrimFlag" HeaderText="TrimFlag"
                            SortExpression="TrimFlag" />
                        <asp:CheckBoxField DataField="RW" HeaderText="RW" SortExpression="RW" />
                        <asp:BoundField DataField="ProductsOnly" HeaderText="ProductsOnly"
                            SortExpression="ProductsOnly" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Wrap="False" Font-Bold="True" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceBinDataSent" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsBin] ORDER BY [ID] DESC"></asp:SqlDataSource>

                <asp:GridView ID="GridViewSortSent" runat="server" AllowPaging="True"
                    AllowSorting="True" 
                    AutoGenerateColumns="False" 
                    Caption="Sort Data Read/Write"
                    CaptionAlign="Left" 
                    DataKeyNames="ID" 
                    DataSourceID="SqlDataSourceSortSent"
                    CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="SortID" HeaderText="SortID"
                            SortExpression="SortID" />
                        <asp:BoundField DataField="SortLabel" HeaderText="SortLabel"
                            SortExpression="SortLabel" />
                        <asp:BoundField DataField="SortSize" HeaderText="SortSize"
                            SortExpression="SortSize" />
                        <asp:BoundField DataField="PkgsPerSort" HeaderText="PkgsPerSort"
                            SortExpression="PkgsPerSort" />
                        <asp:BoundField DataField="OrderCount" HeaderText="OrderCount"
                            SortExpression="OrderCount" />
                        <asp:BoundField DataField="ProductMap0" HeaderText="ProductMap0"
                            SortExpression="ProductMap0" />
                        <asp:BoundField DataField="ProductMap1" HeaderText="ProductMap1"
                            SortExpression="ProductMap1" />
                        <asp:BoundField DataField="ProductMap2" HeaderText="ProductMap2"
                            SortExpression="ProductMap2" />
                        <asp:BoundField DataField="ProductMap3" HeaderText="ProductMap3"
                            SortExpression="ProductMap3" />
                        <asp:BoundField DataField="ProductMap4" HeaderText="ProductMap4"
                            SortExpression="ProductMap4" />
                        <asp:BoundField DataField="ProductMap5" HeaderText="ProductMap5"
                            SortExpression="ProductMap5" />
                        <asp:BoundField DataField="LengthMap" HeaderText="LengthMap"
                            SortExpression="LengthMap" />
                        <asp:BoundField DataField="ProductMap0c" HeaderText="ProductMap0c"
                            SortExpression="ProductMapc0" />
                        <asp:BoundField DataField="ProductMap1c" HeaderText="ProductMap1c"
                            SortExpression="ProductMap1c" />
                        <asp:BoundField DataField="ProductMap2c" HeaderText="ProductMap2c"
                            SortExpression="ProductMap2c" />
                        <asp:BoundField DataField="LengthMapc" HeaderText="LengthMapc"
                            SortExpression="LengthMapc" />
                        <asp:BoundField DataField="ProductMap0Old" HeaderText="ProductMap0Old"
                            SortExpression="ProductMap0Old" />
                        <asp:BoundField DataField="ProductMap1Old" HeaderText="ProductMap1Old"
                            SortExpression="ProductMap1Old" />
                        <asp:BoundField DataField="ProductMap2Old" HeaderText="ProductMap2Old"
                            SortExpression="ProductMap2Old" />
                        <asp:BoundField DataField="ProductMap3Old" HeaderText="ProductMap3Old"
                            SortExpression="ProductMap3Old" />
                        <asp:BoundField DataField="ProductMap4Old" HeaderText="ProductMap4Old"
                            SortExpression="ProductMap4Old" />
                        <asp:BoundField DataField="ProductMap5Old" HeaderText="ProductMap5Old"
                            SortExpression="ProductMap5Old" />
                        <asp:BoundField DataField="SortStamps" HeaderText="SortStamps"
                            SortExpression="SortStamps" />
                        <asp:BoundField DataField="SortSprays" HeaderText="SortSprays"
                            SortExpression="SortSprays" />
                        <asp:BoundField DataField="Zone1" HeaderText="Zone1"
                            SortExpression="Zone1" />
                        <asp:BoundField DataField="Zone2" HeaderText="Zone2" SortExpression="Zone2" />
                        <asp:CheckBoxField DataField="TrimFlag" HeaderText="TrimFlag"
                            SortExpression="TrimFlag" />
                        <asp:CheckBoxField DataField="RW" HeaderText="RW" SortExpression="RW" />
                        <asp:CheckBoxField DataField="Active" HeaderText="Active"
                            SortExpression="Active" />
                        <asp:BoundField DataField="ProductsOnly" HeaderText="ProductsOnly"
                            SortExpression="ProductsOnly" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceSortSent" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsSort] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewAlarmsReceived" runat="server" 
                    AllowPaging="True"
                    AllowSorting="True" 
                    AutoGenerateColumns="False" 
                    DataKeyNames="ID" 
                    Caption="Alarm Data Received"
                    CaptionAlign="Left" 
                    Visible="False"
                    DataSourceID="SqlDataSourceAlarms" 
                    CssClass="table plc" >
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="Map0" HeaderText="Map0" SortExpression="Map0" />
                        <asp:BoundField DataField="Map1" HeaderText="Map1" SortExpression="Map1" />
                        <asp:BoundField DataField="Map2" HeaderText="Map2" SortExpression="Map2" />
                        <asp:BoundField DataField="Map3" HeaderText="Map3" SortExpression="Map3" />
                        <asp:BoundField DataField="Map4" HeaderText="Map4" SortExpression="Map4" />
                        <asp:BoundField DataField="Map5" HeaderText="Map5" SortExpression="Map5" />
                        <asp:BoundField DataField="Map6" HeaderText="Map6" SortExpression="Map6" />
                        <asp:BoundField DataField="Map7" HeaderText="Map7" SortExpression="Map7" />
                        <asp:BoundField DataField="Map8" HeaderText="Map8" SortExpression="Map8" />
                        <asp:BoundField DataField="Map9" HeaderText="Map9" SortExpression="Map9" />
                        <asp:BoundField DataField="Map10" HeaderText="Map10" SortExpression="Map10" />
                        <asp:BoundField DataField="Map11" HeaderText="Map11" SortExpression="Map11" />
                        <asp:BoundField DataField="Map12" HeaderText="Map12" SortExpression="Map12" />
                        <asp:BoundField DataField="Map13" HeaderText="Map13" SortExpression="Map13" />
                        <asp:BoundField DataField="Map14" HeaderText="Map14" SortExpression="Map14" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceAlarms" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataReceivedStatus] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewAlarmsSent" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ID" Caption="Alarm Data Requested"
                    CaptionAlign="Left" Visible="False"
                    DataSourceID="SqlDataSourceAlarms1" CssClass="table plc">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="AlarmID" HeaderText="AlarmID"
                            SortExpression="AlarmID" />
                        <asp:BoundField DataField="AlarmData" HeaderText="AlarmData"
                            SortExpression="AlarmData" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceAlarms1" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsAlarmData] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewProductSent" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False"
                    Caption="Product Data Read/Write" CaptionAlign="Left" DataKeyNames="ID"
                    DataSourceID="SqlDataSourceProductSent" 
                    OnSelectedIndexChanged="GridViewProductSent_SelectedIndexChanged"
                    CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="ProductID" HeaderText="ProductID"
                            SortExpression="ProductID" />
                        <asp:CheckBoxField DataField="Active" HeaderText="Active"
                            SortExpression="Active" />
                        <asp:BoundField DataField="ThicknessID" HeaderText="ThicknessID"
                            SortExpression="ThicknessID" />
                        <asp:BoundField DataField="WidthID" HeaderText="WidthID"
                            SortExpression="WidthID" />
                        <asp:BoundField DataField="GradeID" HeaderText="GradeID"
                            SortExpression="GradeID" />
                        <asp:BoundField DataField="MoistureID" HeaderText="MoistureID"
                            SortExpression="MoistureID" />
                        <asp:BoundField DataField="SpecID" HeaderText="SpecID"
                            SortExpression="SpecID" />
                        <asp:BoundField DataField="SpecialX" HeaderText="SpecialX"
                            SortExpression="SpecialX" />
                        <asp:BoundField DataField="SpecialY" HeaderText="SpecialY"
                            SortExpression="SpecialY" />
                        <asp:BoundField DataField="SpecialZ" HeaderText="SpecialZ"
                            SortExpression="SpecialZ" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceProductSent" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsProduct] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewGradeSent" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" Caption="Grade Data Read/Write"
                    CaptionAlign="Left" DataKeyNames="ID" DataSourceID="SqlDataSourceGradeSent"
                     CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="GradeID" HeaderText="GradeID"
                            SortExpression="GradeID" />
                        <asp:BoundField DataField="GradeIDX" HeaderText="GradeIDX"
                            SortExpression="GradeIDX" />
                        <asp:BoundField DataField="GradeStamps" HeaderText="Grade Stamps"
                            SortExpression="GradeStamps" />
                        <asp:BoundField DataField="Write" HeaderText="Write" SortExpression="Write" />
                        <asp:BoundField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceGradeSent" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsGrade] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewGraderTest" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False"
                    Caption="Grader Test Data Read/Write" CaptionAlign="Left" DataKeyNames="ID"
                    DataSourceID="SqlDataSourceGraderTest"  CssClass="table plc"
                    Visible="False">
                    <PagerStyle BackColor="#FFCC66" Font-Bold="False" Font-Italic="False"
                         ForeColor="#333333" HorizontalAlign="Center" />
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="Graders" HeaderText="Graders"
                            SortExpression="Graders" />
                        <asp:BoundField DataField="Grades" HeaderText="Grades"
                            SortExpression="Grades" />
                        <asp:BoundField DataField="Lengths" HeaderText="Lengths"
                            SortExpression="Lengths" />
                        <asp:BoundField DataField="Samplesize" HeaderText="Samplesize"
                            SortExpression="Samplesize" />
                        <asp:BoundField DataField="SamplesRemaining" HeaderText="SamplesRemaining"
                            SortExpression="SamplesRemaining" />
                        <asp:BoundField DataField="BayID" HeaderText="BayID" SortExpression="BayID" />
                        <asp:BoundField DataField="Interval" HeaderText="Interval"
                            SortExpression="Interval" />
                        <asp:CheckBoxField DataField="Active" HeaderText="Active"
                            SortExpression="Active" />
                        <asp:CheckBoxField DataField="stamp" HeaderText="stamp"
                            SortExpression="stamp" />
                        <asp:CheckBoxField DataField="Trim" HeaderText="Trim" SortExpression="Trim" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceGraderTest" runat="server"
                    ConflictDetection="CompareAllValues"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM datarequestsgradertest ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewMSRSample" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False"
                    Caption="MSR Sample Data Read/Write" CaptionAlign="Left" DataKeyNames="ID"
                    DataSourceID="SqlDataSourceMSRSample"  CssClass="table plc"
                    Visible="False">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="TestID" HeaderText="TestID"
                            SortExpression="TestID" />
                        <asp:BoundField DataField="Grades" HeaderText="Grades"
                            SortExpression="Grades" />
                        <asp:BoundField DataField="Lengths" HeaderText="Lengths"
                            SortExpression="Lengths" />
                        <asp:BoundField DataField="Samplesize" HeaderText="Samplesize"
                            SortExpression="Samplesize" />
                        <asp:BoundField DataField="SamplesRemaining" HeaderText="SamplesRemaining"
                            SortExpression="SamplesRemaining" />
                        <asp:BoundField DataField="Interval" HeaderText="Interval"
                            SortExpression="Interval" />
                        <asp:CheckBoxField DataField="Active" HeaderText="Active"
                            SortExpression="Active" />
                        <asp:CheckBoxField DataField="stamp" HeaderText="stamp"
                            SortExpression="stamp" />
                        <asp:CheckBoxField DataField="Trim" HeaderText="Trim" SortExpression="Trim" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <PagerStyle BackColor="#FFCC66" Font-Bold="False" Font-Italic="False"
                         ForeColor="#333333" HorizontalAlign="Center" />
                    <RowStyle Wrap="False" />
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceMSRSample" runat="server"
                    ConflictDetection="CompareAllValues"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM datarequestsmsrtest ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewMoistureSent" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False"
                    Caption="Moisture Data Read/Write" CaptionAlign="Left" DataKeyNames="ID"
                    DataSourceID="SqlDataSourceMoistureSent" 
                    CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="MoistureID" HeaderText="MoistureID"
                            SortExpression="MoistureID" />
                        <asp:BoundField DataField="MoistureMin" HeaderText="MoistureMin"
                            SortExpression="MoistureMin" />
                        <asp:BoundField DataField="MoistureMax" HeaderText="MoistureMax"
                            SortExpression="MoistureMax" />
                        <asp:BoundField DataField="Write" HeaderText="Write" SortExpression="Write" />
                        <asp:BoundField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceMoistureSent" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsMoisture] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewThickness" runat="server" AllowPaging="True"
                    Caption="Thickness Data Read/Write" CaptionAlign="Left"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ID"
                    DataSourceID="SqlDataSourceThickness" CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="ThickID" HeaderText="ThickID"
                            SortExpression="ThickID" />
                        <asp:BoundField DataField="ThickMin" HeaderText="ThickMin"
                            SortExpression="ThickMin" />
                        <asp:BoundField DataField="ThickMax" HeaderText="ThickMax"
                            SortExpression="ThickMax" />
                        <asp:BoundField DataField="ThickNom" HeaderText="ThickNom"
                            SortExpression="ThickNom" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceThickness" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsThickness] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewWidth" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ID"
                    Caption="Width Data Read/Write" CaptionAlign="Left"
                    DataSourceID="SqlDataSourceWidth" CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="WidthID" HeaderText="WidthID"
                            SortExpression="WidthID" />
                        <asp:BoundField DataField="WidthMin" HeaderText="WidthMin"
                            SortExpression="WidthMin" />
                        <asp:BoundField DataField="WidthMax" HeaderText="WidthMax"
                            SortExpression="WidthMax" />
                        <asp:BoundField DataField="WidthNom" HeaderText="WidthNom"
                            SortExpression="WidthNom" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceWidth" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsWidth] ORDER BY [ID] DESC"></asp:SqlDataSource>
                <asp:GridView ID="GridViewLengthSent" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False"
                    Caption="Length Data Read/Write" CaptionAlign="Left" DataKeyNames="ID"
                    DataSourceID="SqlDataSourceLengthSent" 
                    CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="LengthID" HeaderText="LengthID"
                            SortExpression="LengthID" />
                        <asp:BoundField DataField="LengthMin" HeaderText="LengthMin"
                            SortExpression="LengthMin" />
                        <asp:BoundField DataField="LengthMax" HeaderText="LengthMax"
                            SortExpression="LengthMax" />
                        <asp:BoundField DataField="LengthNom" HeaderText="LengthNom"
                            SortExpression="LengthNom" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceLengthSent" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsLength] ORDER BY [ID] DESC"></asp:SqlDataSource>

                <asp:GridView ID="GridViewPETLengthSent" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False"
                    Caption="PET Length Data Read/Write" CaptionAlign="Left" DataKeyNames="ID"
                    DataSourceID="SqlDataSourcePETLength" CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="PETLengthID" HeaderText="PETLengthID"
                            SortExpression="PETLengthID" />
                        <asp:BoundField DataField="SawIndex" HeaderText="SawIndex"
                            SortExpression="SawIndex" />
                        <asp:BoundField DataField="LengthNom" HeaderText="LengthNom"
                            SortExpression="LengthNom" />
                        <asp:BoundField DataField="PETPosition" HeaderText="PETPosition"
                            SortExpression="PETPosition" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourcePETLength" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsPETLength] ORDER BY [TimeStamp] DESC"></asp:SqlDataSource>

                <asp:GridView ID="GridViewTiming" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ID"
                    Caption="Timing Data Read/Write" CaptionAlign="Left"
                    DataSourceID="SqlDataSourceTiming" CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="PLCID" HeaderText="PLCID" SortExpression="PLCID" />
                        <asp:BoundField DataField="Item1Value" HeaderText="Item1Value"
                            SortExpression="Item1Value" />
                        <asp:BoundField DataField="Item2Value" HeaderText="Item2Value"
                            SortExpression="Item2Value" />
                        <asp:BoundField DataField="Item3Value" HeaderText="Item3Value"
                            SortExpression="Item3Value" />
                        <asp:BoundField DataField="Item4Value" HeaderText="Item4Value"
                            SortExpression="Item4Value" />
                        <asp:BoundField DataField="Item5Value" HeaderText="Item5Value"
                            SortExpression="Item5Value" />
                        <asp:BoundField DataField="Item6Value" HeaderText="Item6Value"
                            SortExpression="Item6Value" />
                        <asp:BoundField DataField="Item7Value" HeaderText="Item7Value"
                            SortExpression="Item7Value" />
                        <asp:BoundField DataField="Item8Value" HeaderText="Item8Value"
                            SortExpression="Item8Value" />
                        <asp:BoundField DataField="Item9Value" HeaderText="Item9Value"
                            SortExpression="Item9Value" />
                        <asp:BoundField DataField="Item10Value" HeaderText="Item10Value"
                            SortExpression="Item10Value" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceTiming" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsTiming] ORDER BY [ID] DESC"></asp:SqlDataSource>

                <asp:GridView ID="GridViewDiagnostics" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ID"
                    Caption="Diagnostic Tests Data Read/Write" CaptionAlign="Left"
                    DataSourceID="SqlDataSourceDiagnostics" CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="DiagnosticID" HeaderText="DiagnosticID"
                            SortExpression="DiagnosticID" />
                        <asp:BoundField DataField="DiagnosticMap" HeaderText="DiagnosticMap"
                            SortExpression="DiagnosticMap" />
                        <asp:BoundField DataField="Parameter" HeaderText="Parameter"
                            SortExpression="Parameter" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceDiagnostics" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsDiagnostic] ORDER BY [ID] DESC"></asp:SqlDataSource>

                <asp:GridView ID="GridViewDiagnostics1" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ID"
                    Caption="Diagnostic Modes Data Read/Write" CaptionAlign="Left"
                    DataSourceID="SqlDataSourceDiagnostics1" CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <RowStyle Wrap="False" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="DiagnosticID" HeaderText="DiagnosticID"
                            SortExpression="DiagnosticID" />
                        <asp:BoundField DataField="DiagnosticMap" HeaderText="DiagnosticMap"
                            SortExpression="DiagnosticMap" />
                        <asp:BoundField DataField="Parameter" HeaderText="Parameter"
                            SortExpression="Parameter" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Wrap="False" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceDiagnostics1" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT * FROM [DataRequestsDiagnostic1] ORDER BY [ID] DESC"></asp:SqlDataSource>

                <asp:GridView ID="GridViewDrives" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" Caption="Drive Data Read/Write"
                    CaptionAlign="Left" DataKeyNames="ID" DataSourceID="SqlDataSourceDrives"
                    CssClass="table plc" Visible="False">
                    <PagerStyle HorizontalAlign="Center" />
                        
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False"
                            ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="TimeStamp"
                            SortExpression="TimeStamp" />
                        <asp:BoundField DataField="DriveID" HeaderText="DriveID"
                            SortExpression="DriveID" />
                        <asp:BoundField DataField="Command" HeaderText="Command"
                            SortExpression="Command" />
                        <asp:BoundField DataField="Actual" HeaderText="Actual"
                            SortExpression="Actual" />
                        <asp:BoundField DataField="MasterLink" HeaderText="MasterLink"
                            SortExpression="MasterLink" />
                        <asp:BoundField DataField="MaxSpeed" HeaderText="MaxSpeed"
                            SortExpression="MaxSpeed" />
                        <asp:BoundField DataField="Scale" HeaderText="Scale" SortExpression="Scale" />
                        <asp:BoundField DataField="SpeedMultiplier" HeaderText="SpeedMultiplier"
                            SortExpression="SpeedMultiplier" />
                        <asp:CheckBoxField DataField="Slave" HeaderText="Slave"
                            SortExpression="Slave" />
                        <asp:CheckBoxField DataField="Master" HeaderText="Master"
                            SortExpression="Master" />
                        <asp:CheckBoxField DataField="Independent" HeaderText="Independent"
                            SortExpression="Independent" />
                        <asp:CheckBoxField DataField="Lineal" HeaderText="Lineal"
                            SortExpression="Lineal" />
                        <asp:CheckBoxField DataField="Transverse" HeaderText="Transverse"
                            SortExpression="Transverse" />
                        <asp:CheckBoxField DataField="Lugged" HeaderText="Lugged"
                            SortExpression="Lugged" />
                        <asp:CheckBoxField DataField="Custom" HeaderText="Custom"
                            SortExpression="Custom" />
                        <asp:CheckBoxField DataField="Write" HeaderText="Write"
                            SortExpression="Write" />
                        <asp:CheckBoxField DataField="Processed" HeaderText="Processed"
                            SortExpression="Processed" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer2" EventName="Tick" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:SqlDataSource ID="SqlDataSourceDrives" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
        SelectCommand="SELECT * FROM [DataRequestsDrive] ORDER BY [TimeStamp] DESC"></asp:SqlDataSource>
    <br />

    <table style="width: 100%;">
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
</asp:Content>