<%@ Page Language="C#" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" MasterPageFile="~/WebSort.Master" CodeBehind="Drives.aspx.cs" Inherits="WebSort.Drives" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxClasses" TagPrefix="dx" %>
<%@ Register Assembly="EO.Web" Namespace="EO.Web" TagPrefix="eo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .style1 {
            width: 106px;
        }

        .auto-style1 {
            width: 129px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function myPostBack() {
            var o = window.event.srcElement;

            if (o.tagName == "INPUT" && o.type == "checkbox") {
                __doPostBack("", "");
            }
        }
        function stopTimer() {
            var timer = $find("<%=this.Page.Master.FindControl("TimerHeader").ClientID%>")
        timer._stopTimer();
        var timer1 = $find('<%=Timer2.ClientID %>');
            timer1._stopTimer();
        }
        function startTimer() {
            var timer = $find("<%=this.Page.Master.FindControl("TimerHeader").ClientID%>")
        timer._startTimer();
        var timer1 = $find('<%=Timer2.ClientID %>');
            timer1._startTimer();
        }
        function stopTimer1() {
            var timer = $find("<%=this.Page.Master.FindControl("TimerHeader").ClientID%>")
            timer._stopTimer();

        }
        function startTimer1() {
            var timer = $find("<%=this.Page.Master.FindControl("TimerHeader").ClientID%>")
            timer._startTimer();

        }
        function OnItemCommand(grid, itemIndex, colIndex, commandName) {

            grid.raiseItemCommandEvent(itemIndex, "select");
        }
        function OnItemSelected(grid) {
            //Get the selected item
            var item = grid.getSelectedItem();

            //Raises server side ItemCommand event.
            //The first parameter is the item index.
            //The second parameter is an additional
            //value that you pass to the server side.
            //This value is not used by the Grid
            grid.raiseItemCommandEvent(item.getIndex(), "select");
        }
        function on_cell_selected_handler(grid) {
            //Get the selected cell
            var cell = grid.getSelectedCell();
            if (cell) {
                //Put the selected cell into edit mode
                setTimeout(function () {
                    grid.editCell(cell.getItemIndex(), cell.getColIndex(), true);
                }, 10);
            }
        }
    </script>
    <eo:ScriptManager ID="ScriptManager1" runat="server"></eo:ScriptManager>
    <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="2"
        AutoPostBack="True"
        CssFilePath="~/App_Themes/BlackGlass/{0}/styles.css"
        CssPostfix="BlackGlass"
        OnActiveTabChanged="ASPxPageControl1_ActiveTabChanged">
        <LoadingPanelImage Url="~/App_Themes/BlackGlass/Web/Loading.gif">
        </LoadingPanelImage>
        <ContentStyle>
            <Border BorderColor="#4E4F51" BorderStyle="Solid" BorderWidth="1px" />
        </ContentStyle>
        <ActiveTabStyle Font-Bold="True">
        </ActiveTabStyle>
        <TabPages>
            <dx:TabPage Text="Drives">
                <ContentCollection>
                    <dx:ContentControl runat="server">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table style="width: 120%;">
                                    <tr>
                                        <td>
                                            <asp:TreeView ID="TreeView1" runat="server" BorderColor="Black"
                                                BorderStyle="Solid" BorderWidth="1px" EnableClientScript="true"
                                                Font-Size="Small" ForeColor="Black" NodeWrap="True"
                                                onclick="javascript:myPostBack()"
                                                OnSelectedNodeChanged="TreeView1_SelectedNodeChanged"
                                                OnTreeNodePopulate="TreeView1_TreeNodePopulate" ShowLines="True"
                                                Width="400px" BackColor="#A4A4A4" Visible="False">
                                                <SelectedNodeStyle BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
                                                    Font-Size="Large" ForeColor="Black" HorizontalPadding="5px" />
                                                <Nodes>
                                                    <asp:TreeNode Expanded="true" PopulateOnDemand="true" SelectAction="Expand"
                                                        Text="Recipes" Value="0" />
                                                </Nodes>
                                                <RootNodeStyle ForeColor="Black" />
                                                <NodeStyle ForeColor="Black" />
                                            </asp:TreeView>
                                            <br />
                                            &nbsp;<asp:UpdatePanel ID="UpdatePanel9" runat="server"
                                                UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <table style="width: 100%;">
                                                        <tr>
                                                            <td class="style1">
                                                                <asp:Label ID="LabelProductLength" runat="server" Font-Size="Small"
                                                                    ForeColor="Black" Text="Product Length:" Visible="False"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LabelProductLength1" runat="server" Font-Size="Small"
                                                                    ForeColor="Black" Visible="False"></asp:Label>
                                                            <td>
                                                        </tr>

                                                        <tr>
                                                            <td class="style1">
                                                                <asp:Label ID="LabelPlanerSpeed" runat="server" Font-Size="Small"
                                                                    ForeColor="Black" Text="Planer Speed:" Visible="False"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LabelPlanerSpeed1" runat="server" Font-Size="Small"
                                                                    ForeColor="Black" Visible="False"></asp:Label>
                                                                <asp:Timer ID="Timer3" runat="server" Interval="100" OnTick="Timer3_Tick" Enabled="False">
                                                                </asp:Timer>
                                                            </td>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="Timer3" EventName="Tick" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                            <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>

                                                    <asp:GridView ID="GridViewDrives" runat="server" Caption="Drives" Visible="false"
                                                        CaptionAlign="Top" SkinID="gridviewSkin" AutoGenerateColumns="False"
                                                        DataSourceID="SqlDataSourceDrives" Width="1000px"
                                                        OnRowDataBound="GridViewDrives_RowDataBound"
                                                        OnRowEditing="GridViewDrives_RowEditing"
                                                        OnRowUpdated="GridViewDrives_RowUpdated"
                                                        OnRowUpdating="GridViewDrives_RowUpdating"
                                                        OnRowCancelingEdit="GridViewDrives_RowCancelingEdit">
                                                        <Columns>
                                                            <asp:TemplateField ShowHeader="False">
                                                                <EditItemTemplate>
                                                                    <asp:Button ID="Button1" runat="server" CausesValidation="True"
                                                                        CommandName="Update" Text="Save" Width="70px" />
                                                                    &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False"
                                                                        CommandName="Cancel" Text="Cancel" Width="70px" />
                                                                </EditItemTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Button ID="Button1" runat="server" CausesValidation="False"
                                                                        CommandName="Edit" Text="Edit" Width="70px" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="DriveID" HeaderText="Drive ID"
                                                                SortExpression="DriveID" ReadOnly="true" />
                                                            <asp:BoundField DataField="DriveLabel" HeaderText="Drive Label"
                                                                SortExpression="DriveLabel" ReadOnly="true" />
                                                            <asp:TemplateField HeaderText="Dependency" SortExpression="Type" Visible="false">
                                                                <EditItemTemplate>
                                                                    <table>
                                                                        <tr>
                                                                            <td width="260">
                                                                                <asp:RadioButtonList ID="RadioButtonList1" runat="server" Enabled="False"
                                                                                    RepeatDirection="Horizontal">
                                                                                    <asp:ListItem Value="-1">Stand Alone</asp:ListItem>
                                                                                    <asp:ListItem Value="0">Master</asp:ListItem>
                                                                                    <asp:ListItem Value="1">Slave To:</asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label1" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </EditItemTemplate>
                                                                <ItemTemplate>
                                                                    <table>
                                                                        <tr>
                                                                            <td width="260">
                                                                                <asp:RadioButtonList ID="RadioButtonList1" runat="server" Enabled="False"
                                                                                    RepeatDirection="Horizontal">
                                                                                    <asp:ListItem Value="-1">Stand Alone</asp:ListItem>
                                                                                    <asp:ListItem Value="0">Master</asp:ListItem>
                                                                                    <asp:ListItem Value="1">Slave To:</asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label1" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="Command" HeaderText="Command"
                                                                SortExpression="Command" />
                                                            <asp:BoundField DataField="Actual" HeaderText="Actual"
                                                                SortExpression="Actual" ReadOnly="true" />
                                                            <asp:BoundField DataField="L1" HeaderText="L1" SortExpression="L1" />
                                                            <asp:BoundField DataField="L2" HeaderText="L2" SortExpression="L2" />
                                                            <asp:BoundField DataField="L3" HeaderText="L3" SortExpression="L3" />
                                                            <asp:BoundField DataField="L4" HeaderText="L4" SortExpression="L4" />
                                                            <asp:BoundField DataField="L5" HeaderText="L5" SortExpression="L5" />
                                                            <asp:BoundField DataField="L6" HeaderText="L6" SortExpression="L6" />
                                                            <asp:BoundField DataField="L7" HeaderText="L7" SortExpression="L7" />
                                                            <asp:BoundField DataField="L8" HeaderText="L8" SortExpression="L8" />
                                                            <asp:BoundField DataField="L9" HeaderText="L9" SortExpression="L9" />
                                                            <asp:BoundField DataField="L10" HeaderText="L10" SortExpression="L10" />
                                                        </Columns>
                                                        <EditRowStyle Wrap="False" />
                                                        <RowStyle Wrap="False" />
                                                    </asp:GridView>

                                                    <eo:CallbackPanel ID="CallbackPanel2" runat="server" Triggers="{ControlID:Button5;Parameter:}"
                                                        AutoDisableContents="True" SafeGuardUpdate="False">
                                                        <table style="width: 40%;">
                                                            <tr>
                                                                <td class="auto-style14">
                                                                    <asp:Button ID="Button8" runat="server" OnClick="Button5_Click" Text="Save" CssClass="btn-raptor" />
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="Button7" runat="server" OnClick="Button7_Click" Text="Cancel" CssClass="btn-raptor" Visible="false" /></td>
                                                            </tr>
                                                        </table>
                                                        <asp:Panel ID="panChanges1" runat="server" Visible="False" Width="400px">
                                                            Change Summary:<br>
                                                        </asp:Panel>
                                                        <eo:Grid ID="Grid1" runat="server" 
                                                            KeyField="driveid" 
                                                            ClientSideOnItemSelected="OnItemSelected" 
                                                            OnItemCommand="Grid1_ItemCommand"
                                                            ColumnHeaderDescImage="00050105" 
                                                            ColumnHeaderAscImage="00050104"
                                                            FixedColumnCount="1" 
                                                            ClientSideOnItemCommand="OnItemCommand"
                                                            GoToBoxVisible="True" 
                                                            GridLines="Both" 
                                                            BorderColor="#7F9DB9"
                                                            GridLineColor="220, 223, 228"
                                                            ColumnHeaderDividerImage="00050103" 
                                                            Font-Names="Tahoma" 
                                                            Font-Size="Small" 
                                                            BorderWidth="1px"
                                                            FullRowMode="False" 
                                                            Font-Bold="True" 
                                                            Font-Italic="False" 
                                                            ColumnHeaderHeight="30" 
                                                            Font-Overline="False" 
                                                            Font-Strikeout="False" 
                                                            Font-Underline="False" E
                                                            nableKeyboardNavigation="True"
                                                            ScrollBars="None" 
                                                            AllowPaging="False" 
                                                            OnItemChanged="Grid1_ItemChanged">
                                                            <ItemStyles>
                                                                <eo:GridItemStyleSet>
                                                                    <ItemHoverStyle CssText="background-color: black"></ItemHoverStyle>
                                                                    <ItemStyle CssText="background-color:#F8F8F8;" />
                                                                    <AlternatingItemStyle CssText="background-color:#D0D1D3;" />
                                                                </eo:GridItemStyleSet>                                                                
                                                            </ItemStyles>
                                                            <FooterStyle CssText="padding-bottom:4px;padding-left:4px;padding-right:4px;padding-top:4px;"></FooterStyle>
                                                            <GoToBoxStyle CssText="BORDER-RIGHT: #7f9db9 1px solid; BORDER-TOP: #7f9db9 1px solid; BORDER-LEFT: #7f9db9 1px solid; WIDTH: 40px; BORDER-BOTTOM: #7f9db9 1px solid"></GoToBoxStyle>
                                                            <ContentPaneStyle CssText="border-bottom-color:#7f9db9;border-bottom-style:solid;border-bottom-width:1px;border-left-color:#7f9db9;border-left-style:solid;border-left-width:1px;border-right-color:#7f9db9;border-right-style:solid;border-right-width:1px;border-top-color:#7f9db9;border-top-style:solid;border-top-width:1px;"></ContentPaneStyle>
                                                            <ColumnTemplates>
                                                                <eo:TextBoxColumn>
                                                                    <TextBoxStyle CssText="TEXT-ALIGN: right; BORDER-RIGHT: #7f9db9 1px solid; PADDING-RIGHT: 2px; BORDER-TOP: #7f9db9 1px solid; PADDING-LEFT: 2px; FONT-SIZE: 8.75pt; PADDING-BOTTOM: 1px; MARGIN: 0px; BORDER-LEFT: #7f9db9 1px solid; PADDING-TOP: 2px; BORDER-BOTTOM: #7f9db9 1px solid; FONT-FAMILY: Tahoma"></TextBoxStyle>
                                                                </eo:TextBoxColumn>                                                                
                                                            </ColumnTemplates>
                                                            <Columns>
                                                                <eo:TextBoxColumn Width="80" HeaderText="Drive ID" DataField="DriveID" AllowSort="False"></eo:TextBoxColumn>
                                                                <eo:TextBoxColumn Width="450" HeaderText="Drive Label" DataField="DriveLabel" ReadOnly="true" AllowSort="False" ClientSideBeginEdit="stopTimer"></eo:TextBoxColumn>

                                                                <eo:TextBoxColumn Width="90" HeaderText="Command" DataField="Command" AllowSort="False" ClientSideBeginEdit="stopTimer">
                                                                    <CellStyle CssText="TEXT-ALIGN: left" />
                                                                </eo:TextBoxColumn>
                                                                <eo:TextBoxColumn Width="80" HeaderText="Actual" DataField="Actual" AllowSort="False" ReadOnly="true" ClientSideBeginEdit="stopTimer"></eo:TextBoxColumn>
                                                                <eo:TextBoxColumn Width="90" HeaderText="Multiplier" DataField="L1" AllowSort="False" ClientSideBeginEdit="stopTimer"></eo:TextBoxColumn>
                                                            </Columns>
                                                            <ColumnHeaderStyle CssText="background-color:#2E3A62;font-family: tahoma; font-size: 14px; color: white;padding-left:8px;padding-top:3px;TEXT-ALIGN: center"></ColumnHeaderStyle>
                                                        </eo:Grid>

                                                        <br>
                                                        <table style="width: 40%;">
                                                            <tr>
                                                                <td class="auto-style14">
                                                                    <asp:Button ID="Button5" runat="server" OnClick="Button5_Click" Text="Save" CssClass="btn-raptor" />
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="Button10" runat="server" OnClick="Button7_Click" Text="Cancel" CssClass="btn-raptor" Visible="false" /></td>
                                                            </tr>
                                                        </table>

                                                        <asp:Panel ID="panChanges" runat="server" Visible="False" Width="400px">
                                                            Change Summary:
                                                        </asp:Panel>
                                                    </eo:CallbackPanel>
                                                    <asp:SqlDataSource ID="SqlDataSourceDrives" runat="server"
                                                        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                                                        SelectCommand="selectDrives" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    <asp:SqlDataSource ID="SqlDataSource2" runat="server"
                                                        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                                                        SelectCommand="selectInfeed2Drives" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    <asp:SqlDataSource ID="SqlDataSource3" runat="server"
                                                        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                                                        SelectCommand="selectInfeed4Drives" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    <asp:Timer ID="Timer2" runat="server" Interval="1000" OnTick="Timer2_Tick">
                                                    </asp:Timer>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="Timer2" EventName="Tick" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                            &nbsp;&nbsp;</td>
                                        <td>&nbsp;</td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="ButtonRefreshAll" runat="server" Font-Size="Small"
                                                        CssClass="btn-raptor" OnClick="ButtonRefreshAll_Click" Text="Refresh All" />
                                                    <asp:Label ID="LabelPLCTimeout" runat="server" Font-Size="Small"
                                                        ForeColor="Red" Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                                    <asp:UpdateProgress ID="UpdateProgress2" runat="server"
                                                        AssociatedUpdatePanelID="UpdatePanel6">
                                                        <ProgressTemplate>
                                                            Reading Drives, Please Wait...
                                                        </ProgressTemplate>
                                                    </asp:UpdateProgress>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td>&nbsp;</td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
            <dx:TabPage Text="Drive Setup">
                <ContentCollection>
                    <dx:ContentControl runat="server">

                        <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                            ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                            SelectCommand="SELECT * FROM [DriveSettings]"
                            DeleteCommand="DELETE FROM [DriveSettings] WHERE [DriveID] = @DriveID"
                            InsertCommand="INSERT INTO [DriveSettings] ([DriveID], [DriveLabel], [Type], [MaxSpeed], [MaxDrive], [GearingActual]) VALUES (@DriveID, @DriveLabel, @Type, @MaxSpeed, 0, @GearingActual)"
                            UpdateCommand="UPDATE [DriveSettings] SET [DriveLabel] = @DriveLabel, [Type] = @Type, Configuration = @Configuration,[MaxSpeed] = @MaxSpeed, [GearingActual] = @GearingActual WHERE [DriveID] = @DriveID"
                            OnSelecting="SqlDataSource1_Selecting">
                            <DeleteParameters>
                                <asp:Parameter Name="DriveID" Type="Int16" />
                            </DeleteParameters>
                            <InsertParameters>
                                <asp:Parameter Name="DriveID" Type="Int16" />
                                <asp:Parameter Name="DriveLabel" Type="String" />
                                <asp:Parameter Name="Type" Type="Int32" />
                                <asp:Parameter Name="MaxSpeed" Type="Single" />
                                <asp:Parameter Name="GearingActual" Type="Single" />
                            </InsertParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="DriveLabel" Type="String" />
                                <asp:Parameter Name="Type" Type="Int32" />
                                <asp:Parameter Name="Configuration" Type="Int32" />
                                <asp:Parameter Name="MaxSpeed" Type="Single" />
                                <asp:Parameter Name="GearingActual" Type="Single" />
                                <asp:Parameter Name="DriveID" Type="Int16" />
                            </UpdateParameters>
                        </asp:SqlDataSource>

                        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                                    Caption="Drive Settings" CaptionAlign="Top" DataKeyNames="DriveID"
                                    DataSourceID="SqlDataSource1" OnRowDataBound="GridView1_RowDataBound"
                                    OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating"
                                    SkinID="gridviewSkin" Width="800px" OnRowUpdated="GridView1_RowUpdated"
                                    OnRowCancelingEdit="GridView1_RowCancelingEdit">
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <EditItemTemplate>
                                                <asp:Button ID="Button1" runat="server" CausesValidation="True"
                                                    CommandName="Update" Text="Save" Width="70px" />
                                                &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False"
                                                    CommandName="Cancel" Text="Cancel" Width="70px" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Button ID="Button1" runat="server" CausesValidation="False"
                                                    CommandName="Edit" Text="Edit" Width="70px" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="DriveID" HeaderText="Drive ID" ReadOnly="True"
                                            SortExpression="DriveID" />
                                        <asp:BoundField DataField="DriveLabel" HeaderText="Drive Label"
                                            SortExpression="DriveLabel" />
                                        <asp:TemplateField HeaderText="Type" SortExpression="Type">
                                            <EditItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td width="260px">
                                                            <asp:RadioButtonList ID="RadioButtonList1" runat="server"
                                                                RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="-1">Stand Alone</asp:ListItem>
                                                                <asp:ListItem Value="0">Master</asp:ListItem>
                                                                <asp:ListItem Value="1">Slave</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DropDownList1" runat="server" AppendDataBoundItems="True"
                                                                DataSourceID="SqlDataSourceDriveLabels" DataTextField="DriveLabel"
                                                                DataValueField="DriveID">
                                                                <asp:ListItem Value="0">None</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:SqlDataSource ID="SqlDataSourceDriveLabels" runat="server"
                                                                ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                                                                SelectCommand="SELECT [DriveID], [DriveLabel] FROM [DriveSettings]"></asp:SqlDataSource>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td width="260">
                                                            <asp:RadioButtonList ID="RadioButtonList1" runat="server" Enabled="False"
                                                                RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="-1">Stand Alone</asp:ListItem>
                                                                <asp:ListItem Value="0">Master</asp:ListItem>
                                                                <asp:ListItem Value="1">Slave To:</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="Label1" runat="server">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                            </asp:Label>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Configuration" SortExpression="Configuration">
                                            <EditItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td width="260px">
                                                            <asp:RadioButtonList ID="RadioButtonList2" runat="server"
                                                                RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="0">Lineal</asp:ListItem>
                                                                <asp:ListItem Value="1">Transverse</asp:ListItem>
                                                                <asp:ListItem Value="2">Lugged</asp:ListItem>
                                                                <asp:ListItem>Custom</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                </table>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td width="260">
                                                            <asp:RadioButtonList ID="RadioButtonList2" runat="server" Enabled="False"
                                                                RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="0">Lineal</asp:ListItem>
                                                                <asp:ListItem Value="1">Transverse</asp:ListItem>
                                                                <asp:ListItem Value="2">Lugged</asp:ListItem>
                                                                <asp:ListItem>Custom</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="MaxSpeed" HeaderText="MaxSpeed"
                                            SortExpression="MaxSpeed" />
                                        <asp:BoundField DataField="GearingActual" HeaderText="Gearing Actual"
                                            SortExpression="GearingActual" />
                                    </Columns>
                                    <EditRowStyle Wrap="False" />
                                    <RowStyle Wrap="False" />
                                </asp:GridView>
                                <br />
                                <asp:Button ID="ButtonRefreshAll0" runat="server" Font-Size="Small"
                                    Height="26px" OnClick="ButtonRefreshAll0_Click" Text="Refresh All" />
                                <asp:Label ID="LabelPLCTimeout0" runat="server" Font-Size="Small"
                                    ForeColor="Red" Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                <asp:UpdateProgress ID="UpdateProgress3" runat="server"
                                    AssociatedUpdatePanelID="UpdatePanel8">
                                    <ProgressTemplate>
                                        Reading Drives, Please Wait...
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
            <dx:TabPage Text="Planer Infeed Drives" Visible="false">
                <ContentCollection>
                    <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                        <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                            <ContentTemplate>
                                <table style="width: 120%;">
                                    <tr>
                                        <td>
                                            <br />
                                            &nbsp;
                                                <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                                                    <ContentTemplate>
                                                        <eo:CallbackPanel ID="CallbackPanel3" runat="server" AutoDisableContents="True" SafeGuardUpdate="False" Triggers="{ControlID:Button14;Parameter:}" UpdateMode="Always">

                                                            <asp:Panel ID="panChanges2" runat="server" Visible="False" Width="400px">
                                                                Change Summary:<br></br>
                                                            </asp:Panel>
                                                            <asp:Label ID="Label2" runat="server" Font-Size="Large" ForeColor="Black" Text="2 Inch Thickness Speeds:"></asp:Label>

                                                            <eo:Grid ID="Grid2" runat="server" AllowPaging="False" BorderColor="#7F9DB9" BorderWidth="1px" ClientSideOnItemCommand="OnItemCommand" ClientSideOnItemSelected="OnItemSelected" ColumnHeaderAscImage="00050104" ColumnHeaderDescImage="00050105" ColumnHeaderDividerImage="00050103" ColumnHeaderHeight="50" EnableKeyboardNavigation="True" FixedColumnCount="1" Font-Bold="True" Font-Italic="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" Font-Strikeout="False" Font-Underline="False" FullRowMode="False" GoToBoxVisible="True" GridLineColor="220, 223, 228" GridLines="Both" KeyField="driveid" OnItemChanged="Grid2_ItemChanged" OnItemCommand="Grid2_ItemCommand" ScrollBars="None" Height="500px">
                                                                <FooterStyle CssText="padding-bottom:4px;padding-left:4px;padding-right:4px;padding-top:4px;" />
                                                                <ItemStyles>
                                                                    <eo:GridItemStyleSet>
                                                                        <ItemHoverStyle CssText="background-color: whitesmoke" />
                                                                        <SelectedStyle CssText="background-color:#316ac5;color:white;" />
                                                                        <ItemStyle CssText="background-color:#FFFBD6;" />
                                                                        <AlternatingItemStyle CssText="background-color:white;" />
                                                                        <FixedColumnCellStyle CssText="border-right: #d6d2c2 1px solid; padding-right: 10px; border-top: #faf9f4 1px solid; border-left: #faf9f4 1px solid; border-bottom: #d6d2c2 1px solid; background-color: #ebeadb; text-align: right" />
                                                                    </eo:GridItemStyleSet>
                                                                </ItemStyles>
                                                                <GoToBoxStyle CssText="BORDER-RIGHT: #7f9db9 1px solid; BORDER-TOP: #7f9db9 1px solid; BORDER-LEFT: #7f9db9 1px solid; WIDTH: 40px; BORDER-BOTTOM: #7f9db9 1px solid" />
                                                                <ContentPaneStyle CssText="border-bottom-color:#7f9db9;border-bottom-style:solid;border-bottom-width:1px;border-left-color:#7f9db9;border-left-style:solid;border-left-width:1px;border-right-color:#7f9db9;border-right-style:solid;border-right-width:1px;border-top-color:#7f9db9;border-top-style:solid;border-top-width:1px;" />
                                                                <ColumnTemplates>
                                                                    <eo:TextBoxColumn>
                                                                        <TextBoxStyle CssText="TEXT-ALIGN: right; BORDER-RIGHT: #7f9db9 1px solid; PADDING-RIGHT: 2px; BORDER-TOP: #7f9db9 1px solid; PADDING-LEFT: 2px; FONT-SIZE: 8.75pt; PADDING-BOTTOM: 1px; MARGIN: 0px; BORDER-LEFT: #7f9db9 1px solid; PADDING-TOP: 2px; BORDER-BOTTOM: #7f9db9 1px solid; FONT-FAMILY: Tahoma" />
                                                                    </eo:TextBoxColumn>
                                                                    <eo:DateTimeColumn>
                                                                        <DatePicker ControlSkinID="None" DayCellHeight="16" DayCellWidth="19" DayHeaderFormat="FirstLetter" DisabledDates="" OtherMonthDayVisible="True" SelectedDates="" TitleLeftArrowImageUrl="DefaultSubMenuIconRTL" TitleRightArrowImageUrl="DefaultSubMenuIcon">
                                                                            <DayHoverStyle CssText="font-family: tahoma; font-size: 12px; border-right: #fbe694 1px solid; border-top: #fbe694 1px solid; border-left: #fbe694 1px solid; border-bottom: #fbe694 1px solid" />
                                                                            <TitleStyle CssText="background-color:#9ebef5;font-family:Tahoma;font-size:12px;padding-bottom:2px;padding-left:6px;padding-right:6px;padding-top:2px;" />
                                                                            <DayHeaderStyle CssText="font-family: tahoma; font-size: 12px; border-bottom: #aca899 1px solid" />
                                                                            <DayStyle CssText="font-family: tahoma; font-size: 12px; border-right: white 1px solid; border-top: white 1px solid; border-left: white 1px solid; border-bottom: white 1px solid" />
                                                                            <SelectedDayStyle CssText="font-family: tahoma; font-size: 12px; background-color: #fbe694; border-right: white 1px solid; border-top: white 1px solid; border-left: white 1px solid; border-bottom: white 1px solid" />
                                                                            <TitleArrowStyle CssText="cursor:hand" />
                                                                            <TodayStyle CssText="font-family: tahoma; font-size: 12px; border-right: #bb5503 1px solid; border-top: #bb5503 1px solid; border-left: #bb5503 1px solid; border-bottom: #bb5503 1px solid" />
                                                                            <PickerStyle CssText="border-bottom-color:#7f9db9;border-bottom-style:solid;border-bottom-width:1px;border-left-color:#7f9db9;border-left-style:solid;border-left-width:1px;border-right-color:#7f9db9;border-right-style:solid;border-right-width:1px;border-top-color:#7f9db9;border-top-style:solid;border-top-width:1px;font-family:Courier New;font-size:8pt;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:1px;padding-left:2px;padding-right:2px;padding-top:2px;" />
                                                                            <OtherMonthDayStyle CssText="font-family: tahoma; font-size: 12px; color: gray; border-right: white 1px solid; border-top: white 1px solid; border-left: white 1px solid; border-bottom: white 1px solid" />
                                                                            <CalendarStyle CssText="background-color: white; border-right: #7f9db9 1px solid; padding-right: 4px; border-top: #7f9db9 1px solid; padding-left: 4px; font-size: 9px; padding-bottom: 4px; border-left: #7f9db9 1px solid; padding-top: 4px; border-bottom: #7f9db9 1px solid; font-family: tahoma" />
                                                                            <DisabledDayStyle CssText="font-family: tahoma; font-size: 12px; color: gray; border-right: white 1px solid; border-top: white 1px solid; border-left: white 1px solid; border-bottom: white 1px solid" />
                                                                            <MonthStyle CssText="font-family: tahoma; font-size: 12px; margin-left: 14px; cursor: hand; margin-right: 14px" />
                                                                        </DatePicker>
                                                                    </eo:DateTimeColumn>
                                                                </ColumnTemplates>
                                                                <Columns>
                                                                    <eo:TextBoxColumn AllowSort="False" DataField="DriveID" HeaderText="Drive ID" Width="70">
                                                                    </eo:TextBoxColumn>
                                                                    <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="DriveLabel" HeaderText="Drive Label" Width="450">
                                                                    </eo:TextBoxColumn>
                                                                    <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width4Multiplier" HeaderText="Width 4" Width="80">
                                                                    </eo:TextBoxColumn>
                                                                    <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width6Multiplier" HeaderText="Width 6" Width="80">
                                                                    </eo:TextBoxColumn>
                                                                    <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width8Multiplier" HeaderText="Width 8" Width="80">
                                                                    </eo:TextBoxColumn>
                                                                    <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width10Multiplier" HeaderText="Width 10" Width="80">
                                                                    </eo:TextBoxColumn>
                                                                    <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width12Multiplier" HeaderText="Width 12" Width="80">
                                                                    </eo:TextBoxColumn>
                                                                </Columns>
                                                                <ColumnHeaderStyle CssText="background-color:#990000;font-family: tahoma; font-size: 12px; color: white;padding-left:8px;padding-top:3px;TEXT-ALIGN: center" />
                                                            </eo:Grid>
                                                            <table style="width: 40%;">

                                                                <tr>
                                                                    <td class="auto-style14">
                                                                        <asp:Button ID="Button14" runat="server" OnClick="Button14_Click" Text="Save" Width="100px" />
                                                                    </td>
                                                                    <td class="auto-style1">
                                                                        <asp:Button ID="Button15" runat="server" OnClick="Button15_Click" Text="Cancel" Visible="false" Width="100px" />
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                            </table>
                                                            <br>
                                                                <asp:Label ID="Label3" runat="server" Font-Size="Large" ForeColor="Black" Text="4 Inch Thickness Speeds:"></asp:Label>

                                                                <eo:Grid ID="Grid3" runat="server" AllowPaging="False" BorderColor="#7F9DB9" BorderWidth="1px" ClientSideOnItemCommand="OnItemCommand" ClientSideOnItemSelected="OnItemSelected" ColumnHeaderAscImage="00050104" ColumnHeaderDescImage="00050105" ColumnHeaderDividerImage="00050103" ColumnHeaderHeight="50" EnableKeyboardNavigation="True" FixedColumnCount="1" Font-Bold="True" Font-Italic="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" Font-Strikeout="False" Font-Underline="False" FullRowMode="False" GoToBoxVisible="True" GridLineColor="220, 223, 228" GridLines="Both" KeyField="driveid" OnItemCommand="Grid3_ItemCommand" ScrollBars="None" Height="500px">
                                                                    <FooterStyle CssText="padding-bottom:4px;padding-left:4px;padding-right:4px;padding-top:4px;" />
                                                                    <ItemStyles>
                                                                        <eo:GridItemStyleSet>
                                                                            <ItemHoverStyle CssText="background-color: whitesmoke" />
                                                                            <SelectedStyle CssText="background-color:#316ac5;color:white;" />
                                                                            <ItemStyle CssText="background-color:#FFFBD6;" />
                                                                            <AlternatingItemStyle CssText="background-color:white;" />
                                                                            <FixedColumnCellStyle CssText="border-right: #d6d2c2 1px solid; padding-right: 10px; border-top: #faf9f4 1px solid; border-left: #faf9f4 1px solid; border-bottom: #d6d2c2 1px solid; background-color: #ebeadb; text-align: right" />
                                                                        </eo:GridItemStyleSet>
                                                                    </ItemStyles>
                                                                    <GoToBoxStyle CssText="BORDER-RIGHT: #7f9db9 1px solid; BORDER-TOP: #7f9db9 1px solid; BORDER-LEFT: #7f9db9 1px solid; WIDTH: 40px; BORDER-BOTTOM: #7f9db9 1px solid" />
                                                                    <ContentPaneStyle CssText="border-bottom-color:#7f9db9;border-bottom-style:solid;border-bottom-width:1px;border-left-color:#7f9db9;border-left-style:solid;border-left-width:1px;border-right-color:#7f9db9;border-right-style:solid;border-right-width:1px;border-top-color:#7f9db9;border-top-style:solid;border-top-width:1px;" />
                                                                    <ColumnTemplates>
                                                                        <eo:TextBoxColumn>
                                                                            <TextBoxStyle CssText="TEXT-ALIGN: right; BORDER-RIGHT: #7f9db9 1px solid; PADDING-RIGHT: 2px; BORDER-TOP: #7f9db9 1px solid; PADDING-LEFT: 2px; FONT-SIZE: 8.75pt; PADDING-BOTTOM: 1px; MARGIN: 0px; BORDER-LEFT: #7f9db9 1px solid; PADDING-TOP: 2px; BORDER-BOTTOM: #7f9db9 1px solid; FONT-FAMILY: Tahoma" />
                                                                        </eo:TextBoxColumn>
                                                                        <eo:DateTimeColumn>
                                                                            <DatePicker ControlSkinID="None" DayCellHeight="16" DayCellWidth="19" DayHeaderFormat="FirstLetter" DisabledDates="" OtherMonthDayVisible="True" SelectedDates="" TitleLeftArrowImageUrl="DefaultSubMenuIconRTL" TitleRightArrowImageUrl="DefaultSubMenuIcon">
                                                                                <DayHoverStyle CssText="font-family: tahoma; font-size: 12px; border-right: #fbe694 1px solid; border-top: #fbe694 1px solid; border-left: #fbe694 1px solid; border-bottom: #fbe694 1px solid" />
                                                                                <TitleStyle CssText="background-color:#9ebef5;font-family:Tahoma;font-size:12px;padding-bottom:2px;padding-left:6px;padding-right:6px;padding-top:2px;" />
                                                                                <DayHeaderStyle CssText="font-family: tahoma; font-size: 12px; border-bottom: #aca899 1px solid" />
                                                                                <DayStyle CssText="font-family: tahoma; font-size: 12px; border-right: white 1px solid; border-top: white 1px solid; border-left: white 1px solid; border-bottom: white 1px solid" />
                                                                                <SelectedDayStyle CssText="font-family: tahoma; font-size: 12px; background-color: #fbe694; border-right: white 1px solid; border-top: white 1px solid; border-left: white 1px solid; border-bottom: white 1px solid" />
                                                                                <TitleArrowStyle CssText="cursor:hand" />
                                                                                <TodayStyle CssText="font-family: tahoma; font-size: 12px; border-right: #bb5503 1px solid; border-top: #bb5503 1px solid; border-left: #bb5503 1px solid; border-bottom: #bb5503 1px solid" />
                                                                                <PickerStyle CssText="border-bottom-color:#7f9db9;border-bottom-style:solid;border-bottom-width:1px;border-left-color:#7f9db9;border-left-style:solid;border-left-width:1px;border-right-color:#7f9db9;border-right-style:solid;border-right-width:1px;border-top-color:#7f9db9;border-top-style:solid;border-top-width:1px;font-family:Courier New;font-size:8pt;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:1px;padding-left:2px;padding-right:2px;padding-top:2px;" />
                                                                                <OtherMonthDayStyle CssText="font-family: tahoma; font-size: 12px; color: gray; border-right: white 1px solid; border-top: white 1px solid; border-left: white 1px solid; border-bottom: white 1px solid" />
                                                                                <CalendarStyle CssText="background-color: white; border-right: #7f9db9 1px solid; padding-right: 4px; border-top: #7f9db9 1px solid; padding-left: 4px; font-size: 9px; padding-bottom: 4px; border-left: #7f9db9 1px solid; padding-top: 4px; border-bottom: #7f9db9 1px solid; font-family: tahoma" />
                                                                                <DisabledDayStyle CssText="font-family: tahoma; font-size: 12px; color: gray; border-right: white 1px solid; border-top: white 1px solid; border-left: white 1px solid; border-bottom: white 1px solid" />
                                                                                <MonthStyle CssText="font-family: tahoma; font-size: 12px; margin-left: 14px; cursor: hand; margin-right: 14px" />
                                                                            </DatePicker>
                                                                        </eo:DateTimeColumn>
                                                                    </ColumnTemplates>
                                                                    <Columns>
                                                                        <eo:TextBoxColumn AllowSort="False" DataField="DriveID" HeaderText="Drive ID" Width="70">
                                                                        </eo:TextBoxColumn>
                                                                        <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="DriveLabel" HeaderText="Drive Label" Width="450">
                                                                        </eo:TextBoxColumn>
                                                                        <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width4Multiplier" HeaderText="Width 4" Width="80">
                                                                        </eo:TextBoxColumn>
                                                                        <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width6Multiplier" HeaderText="Width 6" Width="80">
                                                                        </eo:TextBoxColumn>
                                                                        <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width8Multiplier" HeaderText="Width 8" Width="80">
                                                                        </eo:TextBoxColumn>
                                                                        <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width10Multiplier" HeaderText="Width 10" Width="80">
                                                                        </eo:TextBoxColumn>
                                                                        <eo:TextBoxColumn AllowSort="False" ClientSideBeginEdit="stopTimer1" DataField="Width12Multiplier" HeaderText="Width 12" Width="80">
                                                                        </eo:TextBoxColumn>
                                                                    </Columns>
                                                                    <ColumnHeaderStyle CssText="background-color:#990000;font-family: tahoma; font-size: 12px; color: white;padding-left:8px;padding-top:3px;TEXT-ALIGN: center" />
                                                                </eo:Grid>
                                                                <table style="width: 40%;">
                                                                    <tr>
                                                                        <td class="auto-style14">
                                                                            <asp:Button ID="Button16" runat="server" OnClick="Button14_Click" Text="Save" Width="100px" />
                                                                        </td>
                                                                        <td>
                                                                            <asp:Button ID="Button17" runat="server" OnClick="Button15_Click" Text="Cancel" Visible="false" Width="100px" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:Panel ID="panChanges3" runat="server" Visible="False" Width="400px">
                                                                    Change Summary:
                                                                </asp:Panel>
                                                                <br></br>
                                                            </br>
                                                        </eo:CallbackPanel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            &nbsp;&nbsp;</td>
                                        <td>&nbsp;</td>
                                        <td>&nbsp;&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel ID="UpdatePanel13" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="ButtonRefreshAll1" runat="server" Font-Size="Small" Height="26px" OnClick="ButtonRefreshAll_Click" Text="Refresh All" Visible="False" />
                                                    <asp:Label ID="LabelPLCTimeout1" runat="server" Font-Size="Small" ForeColor="Red" Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                                    <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="UpdatePanel6">
                                                        <ProgressTemplate>
                                                            Reading Drives, Please Wait...
                                                        </ProgressTemplate>
                                                    </asp:UpdateProgress>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
        </TabPages>
    </dx:ASPxPageControl>
</asp:Content>