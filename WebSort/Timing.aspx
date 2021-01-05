<%@ Page Language="C#" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" MasterPageFile="~/WebSort.Master" CodeBehind="Timing.aspx.cs" Inherits="WebSort.Timing" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxTabControl" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v10.2, Version=10.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxClasses" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script language='javascript'>

        function confirm_delete(plcgroup) {
            var message = "Are you sure you want to delete ";
            message = message.concat(plcgroup, "?");

            if (confirm(message) == true)
                return true;
            else
                return false;
        }

        function myPostBack() {
            var o = window.event.srcElement;

            if (o.tagName == "INPUT" && o.type == "checkbox") {
                __doPostBack("", "");
            }

        }
    </script>
    <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="0"
        SaveStateToCookies="True"
        CssFilePath="~/App_Themes/BlackGlass/{0}/styles.css"
        CssPostfix="BlackGlass">
        <LoadingPanelImage Url="~/App_Themes/BlackGlass/Web/Loading.gif">
        </LoadingPanelImage>
        <ContentStyle>
            <Border BorderColor="#4E4F51" BorderStyle="Solid" BorderWidth="1px" />
        </ContentStyle>
        <TabPages>
            <dx:TabPage Text="Timing">
                <ContentCollection>
                    <dx:ContentControl ID="ContentControl1" runat="server">
                        <table style="width: 100%;">
                            <tr>
                                <td>

                                    <dx:ASPxTabControl ID="ASPxTabControl1" runat="server"
                                        OnLoad="ASPxTabControl1_Load"
                                        OnActiveTabChanged="ASPxTabControl1_ActiveTabChanged"
                                        CssFilePath="~/App_Themes/BlackGlass/{0}/styles.css"
                                        CssPostfix="BlackGlass">
                                        <ContentStyle>
                                            <Border BorderColor="#4E4F51" BorderStyle="Solid" BorderWidth="1px" />
                                        </ContentStyle>
                                    </dx:ASPxTabControl>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Label ID="LabelPLCID" runat="server" Font-Size="Small" ForeColor="Black"></asp:Label>
                                            <asp:GridView ID="GridView1" runat="server"
                                                AutoGenerateColumns="False" DataKeyNames="ID" DataSourceID="SqlDataSource1"
                                                SkinID="gridviewSkin" Width="800px" OnRowUpdating="GridView1_RowUpdating"
                                                OnDataBound="GridView1_DataBound" OnRowDataBound="GridView1_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField ShowHeader="False">
                                                        <EditItemTemplate>
                                                            <asp:Button ID="Button1" runat="server" CausesValidation="True"
                                                                CommandName="Update" Text="Save" Width="70px" />
                                                            &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False"
                                                                CommandName="Cancel" Text="Cancel" Width="70px" />
                                                            <br />
                                                            <a href="#">
                                                                <asp:Label ID="LabelPLCTimeoutProd" runat="server" ForeColor="Red"
                                                                    Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                                                <asp:Label ID="LabelTooSmall" runat="server" ForeColor="Red"
                                                                    Text="The value entered is too small" Visible="False"></asp:Label>
                                                                <asp:Label ID="LabelTooLarge" runat="server" ForeColor="Red"
                                                                    Text="The value entered is too large" Visible="False"></asp:Label>
                                                            </a>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Button ID="Button1" runat="server" CausesValidation="False"
                                                                CommandName="Edit" Text="Edit" Width="70px" />
                                                            <asp:Button ID="Button3" runat="server" CausesValidation="False"
                                                                CommandName="Delete" Text="Delete" Width="70px" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="PLCID" HeaderText="PLC ID" ReadOnly="True"
                                                        SortExpression="PLCID" Visible="false" />
                                                    <asp:BoundField DataField="ID" HeaderText="Item ID" ReadOnly="True"
                                                        SortExpression="ID" />
                                                    <asp:BoundField DataField="ItemName" HeaderText="Item"
                                                        SortExpression="ItemName">
                                                        <ControlStyle Width="100%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ItemValue" HeaderText="Value"
                                                        SortExpression="ItemValue" />
                                                    <asp:BoundField DataField="PreviousValue" HeaderText="Previous"
                                                        SortExpression="PreviousValue" ReadOnly="true" />
                                                    <asp:BoundField DataField="Min" HeaderText="Min"
                                                        SortExpression="Min" />
                                                    <asp:BoundField DataField="Max" HeaderText="Max"
                                                        SortExpression="Max" />
                                                </Columns>
                                            </asp:GridView>
                                            <asp:Button ID="ButtonInsertNewItem" runat="server"
                                                OnClick="ButtonInsertNewItem_Click" Text="Insert New Item" Width="125px" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ASPxTabControl1"
                                                EventName="ActiveTabChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="DataBound" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                            <asp:Button ID="ButtonRefreshAll" runat="server" Font-Size="Small"
                                                OnClick="ButtonRefreshAll_Click" Text="Refresh All" Width="125px" />
                                            <asp:Label ID="LabelTimeout0" runat="server" Font-Size="Small" ForeColor="Red"
                                                Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                            <asp:UpdateProgress ID="UpdateProgress1" runat="server"
                                                AssociatedUpdatePanelID="UpdatePanel4">
                                                <ProgressTemplate>
                                                    Reading Timing, Please Wait...
                                                </ProgressTemplate>
                                            </asp:UpdateProgress>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="ButtonAddNewGroup" runat="server" CausesValidation="False"
                                        OnClick="ButtonAddNewGroup_Click" Text="Add New Group" Width="125px" />
                                    <asp:Button ID="ButtonRenameGroup0" runat="server" CausesValidation="False"
                                        OnClick="ButtonRenameGroup0_Click" Text="Rename Group" Width="125px" />
                                    <asp:Button ID="ButtonDeleteGroup" runat="server" CausesValidation="False"
                                        OnClick="ButtonDeleteGroup_Click" Text="Delete Group" Width="125px" />
                                    <asp:Button ID="ButtonChangeGroupID" runat="server" CausesValidation="False"
                                        OnClick="ButtonChangeGroupID_Click" Text="Change Group ID" Width="125px" />
                                    <asp:Label ID="LabelTimeout" runat="server" Font-Size="Small" ForeColor="Red"
                                        Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="LabelOldName" runat="server" Font-Size="Small" ForeColor="Black"
                                        Text="Old Group Name: " Visible="False"></asp:Label>
                                    <asp:Label ID="LabelOldName1" runat="server" Font-Size="Small"
                                        ForeColor="Black" Visible="False"></asp:Label>
                                    <asp:Label ID="LabelOldGroupID" runat="server" Font-Size="Small"
                                        ForeColor="Black" Text="Old Group ID: " Visible="False"></asp:Label>
                                    <asp:Label ID="LabelOldGroupID1" runat="server" Font-Size="Small"
                                        ForeColor="Black" Visible="False"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="LabelNewName" runat="server" Font-Size="Small" ForeColor="Black"
                                        Text="New Group Name: " Visible="False"></asp:Label>
                                    <asp:TextBox ID="TextBoxNewName" runat="server"
                                        Visible="False" Width="200px"></asp:TextBox>
                                    <asp:Label ID="LabelNewGroupID" runat="server" Font-Size="Small"
                                        ForeColor="Black" Text="New Group ID: " Visible="False"></asp:Label>
                                    <asp:DropDownList ID="DropDownListGroupID" runat="server" Visible="False">
                                    </asp:DropDownList>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="ButtonSave" runat="server" OnClick="ButtonSave_Click"
                                        Text="Save" Visible="False" Width="50px" />
                                    <asp:Button ID="ButtonCancel" runat="server" OnClick="ButtonCancel_Click"
                                        Text="Cancel" Visible="False" Width="50px" />
                                    <asp:Button ID="ButtonSave0" runat="server" OnClick="ButtonSave0_Click"
                                        Text="Save" Visible="False" Width="50px" />
                                    <asp:Button ID="ButtonCancel0" runat="server" OnClick="ButtonCancel0_Click"
                                        Text="Cancel" Visible="False" Width="50px" />
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                                        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                                        DeleteCommand="if (select count(*) from timing where [categoryname] = @CategoryName) &gt; 1
begin
  DELETE from Timing where [categoryname] = @CategoryName and [id] = @ID
  update timing set id = id - 1 where [categoryname] = @CategoryName and [id] &gt; @ID
end"
                                        SelectCommand="SELECT [PLCID], id,[ItemName], [ItemValue],[PreviousValue],[min],[max] FROM [Timing] WHERE ([CategoryName] = @CategoryName) order by id"
                                        UpdateCommand="UPDATE [Timing] SET [itemname] = @itemname, [itemvalue] =@itemvalue,[previousvalue]=@previousvalue,[min]=@min,[max]=@max WHERE [categoryname] = @CategoryName and [id] = @ID">
                                        <DeleteParameters>
                                            <asp:ControlParameter ControlID="ASPxTabControl1" Name="CategoryName"
                                                PropertyName="ActiveTab.Text" Type="String" />
                                            <asp:Parameter Name="ID" Type="Int32" />
                                        </DeleteParameters>
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="ASPxTabControl1" Name="CategoryName"
                                                PropertyName="ActiveTab.Text" Type="String" />
                                        </SelectParameters>
                                        <UpdateParameters>
                                            <asp:ControlParameter ControlID="ASPxTabControl1" Name="CategoryName"
                                                PropertyName="ActiveTab.Text" Type="String" />
                                            <asp:Parameter Name="itemname" Type="String" />
                                            <asp:Parameter Name="ID" Type="Int32" />
                                            <asp:Parameter Name="itemvalue" Type="Int32" />
                                            <asp:Parameter Name="previousvalue" Type="Int32" />
                                            <asp:Parameter Name="min" Type="Int32" />
                                            <asp:Parameter Name="max" Type="Int32" />
                                        </UpdateParameters>
                                    </asp:SqlDataSource>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
            <dx:TabPage Text="Parameters">
                <ContentCollection>
                    <dx:ContentControl ID="ContentControl2" runat="server">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="overflow: hidden" valign="top">
                                            <asp:TreeView ID="TreeView1" runat="server" BorderColor="Black"
                                                BorderStyle="Solid" BorderWidth="1px" EnableClientScript="true"
                                                Font-Size="Small" ForeColor="Black" NodeWrap="True"
                                                onclick="javascript:myPostBack()"
                                                OnSelectedNodeChanged="TreeView1_SelectedNodeChanged"
                                                OnTreeNodePopulate="TreeView1_TreeNodePopulate" ShowLines="True"
                                                Width="257px">
                                                <SelectedNodeStyle BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
                                                    Font-Size="Large" ForeColor="Black" HorizontalPadding="5px" />
                                                <Nodes>
                                                    <asp:TreeNode Expanded="true" PopulateOnDemand="true" SelectAction="Expand" Text="Recipes" Value="0" />
                                                </Nodes>
                                                <RootNodeStyle ForeColor="Black" />
                                                <NodeStyle ForeColor="Black" />
                                            </asp:TreeView>
                                        </td>
                                        <td style="overflow: hidden" valign="top">
                                            <asp:GridView ID="GridView2" runat="server" AllowPaging="False" AutoGenerateColumns="False"
                                                DataKeyNames="ID" DataSourceID="SqlDataSource2" OnRowCancelingEdit="GridView2_RowCancelingEdit" OnRowDeleted="GridView2_RowDeleted" OnRowDeleting="GridView2_RowDeleting" OnRowEditing="GridView2_RowEditing" OnRowUpdated="GridView2_RowUpdated" OnRowUpdating="GridView2_RowUpdating" SkinID="gridviewSkin" Width="650px">
                                                <Columns>
                                                    <asp:TemplateField ShowHeader="False">
                                                        <EditItemTemplate>
                                                            <asp:Button ID="Button5" runat="server" CausesValidation="True" CommandName="Update" Text="Save" Width="70px" />
                                                            &nbsp;<asp:Button ID="Button6" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" Width="70px" />
                                                            <br />
                                                            <a href="#">
                                                                <asp:Label ID="LabelPLCTimeoutProd0" runat="server" ForeColor="Red" Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                                                <asp:Label ID="LabelTooSmall0" runat="server" ForeColor="Red" Text="The value entered is too small" Visible="False"></asp:Label>
                                                                <asp:Label ID="LabelTooLarge0" runat="server" ForeColor="Red" Text="The value entered is too large" Visible="False"></asp:Label>
                                                            </a>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Button ID="Button7" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" Width="70px" />
                                                            <asp:Button ID="Button8" runat="server" CausesValidation="False" CommandName="Delete" OnClick="Button8_Click" Text="Delete" Visible="False" Width="70px" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="ID" HeaderText="Item ID" ReadOnly="True" SortExpression="ID" />
                                                    <asp:BoundField DataField="ItemName" HeaderText="Item" SortExpression="ItemName">
                                                        <ControlStyle Width="100%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ItemValue" HeaderText="Value" SortExpression="ItemValue" />
                                                    <asp:BoundField DataField="Min" HeaderText="Min" SortExpression="Min" />
                                                    <asp:BoundField DataField="Max" HeaderText="Max" SortExpression="Max" />
                                                </Columns>
                                            </asp:GridView>
                                            <asp:Button ID="ButtonInsertNewItem0" runat="server" OnClick="ButtonInsertNewItem0_Click" Text="Insert New Item" Width="125px" />
                                            <br />
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="Button4" runat="server" Font-Size="Small" OnClick="Button4_Click" Text="Refresh All" Width="125px" />
                                                    <asp:Label ID="Label1" runat="server" Font-Size="Small" ForeColor="Red" Text="Timeout Communicating with PLC" Visible="False"></asp:Label>
                                                    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel5" Visible="False">
                                                        <ProgressTemplate>
                                                            Reading Parameters, Please Wait...
                                                        </ProgressTemplate>
                                                    </asp:UpdateProgress>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="overflow: hidden" valign="top">&nbsp;</td>
                                        <td style="overflow: hidden" valign="top">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="overflow: hidden" valign="top">&nbsp;</td>
                                        <td style="overflow: hidden" valign="top">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="style7" valign="top">&nbsp;</td>
                                        <td class="style7" valign="top">
                                            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>" DeleteCommand="DELETE FROM Parameters WHERE (ID = @ID)" SelectCommand="SELECT id,[ItemName], [ItemValue],[min],[max] FROM [Parameters] where id&lt;50 and recipeid = (select min(recipeid) from recipes where editing=1) order by id" UpdateCommand="UPDATE [Parameters] SET [itemname] = @itemname, [itemvalue] =@itemvalue,[min]=@min,[max]=@max WHERE recipeid = @recipeid and [id] = @ID
update [Parameters] set [itemname]=@itemname where [id]=@ID">
                                                <DeleteParameters>
                                                    <asp:Parameter Name="ID" Type="Int32" />
                                                </DeleteParameters>
                                                <UpdateParameters>
                                                    <asp:ControlParameter ControlID="ASPxTabControl1" Name="CategoryName" PropertyName="ActiveTab.Text" Type="String" />
                                                    <asp:Parameter Name="itemname" Type="String" />
                                                    <asp:Parameter Name="ID" Type="Int32" />
                                                    <asp:Parameter Name="itemvalue" Type="Single" />
                                                    <asp:Parameter Name="min" Type="Single" />
                                                    <asp:Parameter Name="max" Type="Single" />
                                                    <asp:Parameter Name="recipeid" Type="Int32" />
                                                </UpdateParameters>
                                            </asp:SqlDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">&nbsp;</td>
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