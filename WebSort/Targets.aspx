<%@ Page Language="C#" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" MasterPageFile="~/WebSort.Master" CodeBehind="Targets.aspx.cs" Inherits="WebSort.Targets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .card {
            background-color: var(--accent);
            box-shadow: 0px 5px 5px 3px var(--shadow);
            padding: 0.5rem;
            color: var(--text-secondary);
        }

        .card-title {
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Font-Size="Medium" Text="Target Mode:"></asp:Label>
        <asp:RadioButtonList ID="RadioButtonListTargetMode" runat="server"
            Font-Size="Small" 
            OnSelectedIndexChanged="RadioButtonListTargetMode_SelectedIndexChanged"
            RepeatDirection="Horizontal" AutoPostBack="True">
            <asp:ListItem>Shift</asp:ListItem>
            <asp:ListItem>Run</asp:ListItem>
        </asp:RadioButtonList>
    </div>
    <div>
        <asp:Panel ID="PanelShift" runat="server" Visible="False" CssClass="card mt-5" style="max-width: 1000px;">
            <asp:GridView ID="GridView2" runat="server" 
                AutoGenerateColumns="False"
                DataKeyNames="ShiftIndex" 
                DataSourceID="SqlDataSource2" 
                CssClass="table">
                <RowStyle Wrap="False" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" ButtonType="Button" ControlStyle-CssClass="btn-raptor" />
                    <asp:BoundField DataField="ShiftIndex" HeaderText="ShiftIndex" ReadOnly="True" Visible="false"
                        SortExpression="ShiftIndex" />
                    <asp:BoundField DataField="TargetVolumePerHour"
                        HeaderText="Target Volume Per Hour" SortExpression="TargetVolumePerHour" />
                    <asp:BoundField DataField="TargetPiecesPerHour"
                        HeaderText="Target Pieces Per Hour" SortExpression="TargetPiecesPerHour" />
                    <asp:BoundField DataField="TargetLugFill" HeaderText="Target LugFill"
                        SortExpression="TargetLugFill" />
                    <asp:BoundField DataField="TargetUptime" HeaderText="Target Uptime" Visible="false"
                        SortExpression="TargetUptime" />
                </Columns>
                <HeaderStyle Font-Bold="True" Font-Size="Small" Wrap="False" />
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource2" runat="server"
                ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                SelectCommand="SELECT [ShiftIndex], [TargetVolumePerHour], [TargetPiecesPerHour], [TargetLugFill], [TargetUptime] FROM [Shifts] WHERE ([ShiftIndex] = (select max(shiftindex) from shifts))"
                UpdateCommand="update shifts set  [TargetVolumePerHour]=@TargetVolumePerHour, [TargetPiecesPerHour]=@TargetPiecesPerHour, [TargetLugFill]=@TargetLugFill, [TargetUptime]=@TargetUptime  WHERE ([ShiftIndex] = (select max(shiftindex) from shifts))">
                <UpdateParameters>
                    <asp:Parameter Name="TargetVolumePerHour" />
                    <asp:Parameter Name="TargetPiecesPerHour" />
                    <asp:Parameter Name="TargetLugFill" />
                    <asp:Parameter Name="TargetUptime" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </asp:Panel>

        <asp:Panel ID="PanelRun" runat="server" Visible="False" CssClass="card mt-5" style="max-width: 1000px;">
            
            <asp:GridView ID="GridView1" runat="server" 
                AllowSorting="True"
                AutoGenerateColumns="False" 
                DataKeyNames="RecipeID"
                DataSourceID="SqlDataSource1" 
                CssClass="table">
                <RowStyle Wrap="False" />
                <Columns>
                    <asp:CommandField ButtonType="Button" ShowEditButton="True" ControlStyle-CssClass="btn-raptor" />
                    <asp:BoundField DataField="RecipeID" HeaderText="RecipeID"
                        InsertVisible="False" ReadOnly="True" SortExpression="RecipeID"
                        Visible="false" />
                    <asp:BoundField DataField="RecipeLabel" HeaderText="Recipe" ReadOnly="True"
                        SortExpression="RecipeLabel" />
                    <asp:BoundField DataField="TargetVolumePerHour"
                        HeaderText="Target Volume Per Hour" SortExpression="TargetVolumePerHour" />
                    <asp:BoundField DataField="TargetPiecesPerHour"
                        HeaderText="Target Pieces Per Hour" SortExpression="TargetPiecesPerHour" />
                    <asp:BoundField DataField="TargetLugFill" HeaderText="Target LugFill"
                        SortExpression="TargetLugFill" />
                    <asp:BoundField DataField="TargetUptime" HeaderText="Target Uptime"
                        SortExpression="TargetUptime" Visible="false" />
                </Columns>
                <HeaderStyle Font-Bold="True" Font-Size="Small" Wrap="False" />
            </asp:GridView>

            <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                DeleteCommand="DELETE FROM [Recipes] WHERE [RecipeLabel] = @RecipeLabel"
                InsertCommand="INSERT INTO [Recipes] ([RecipeLabel], [TargetVolumePerHour], [TargetPiecesPerHour], [TargetLugFill], [TargetUptime]) VALUES (@RecipeLabel, @TargetVolumePerHour, @TargetPiecesPerHour, @TargetLugFill, @TargetUptime)"
                SelectCommand="SELECT * FROM [Recipes]"
                UpdateCommand="UPDATE [Recipes] SET [TargetVolumePerHour] = @TargetVolumePerHour, [TargetPiecesPerHour] = @TargetPiecesPerHour, [TargetLugFill] = @TargetLugFill, [TargetUptime] = @TargetUptime WHERE [Recipeid] = @Recipeid">
                <DeleteParameters>
                    <asp:Parameter Name="RecipeLabel" Type="String" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="RecipeID" Type="Int32" />
                    <asp:Parameter Name="TargetVolumePerHour" Type="Single" />
                    <asp:Parameter Name="TargetPiecesPerHour" Type="Int32" />
                    <asp:Parameter Name="TargetLugFill" Type="Int16" />
                    <asp:Parameter Name="TargetUptime" Type="Int32" />
                    <asp:Parameter Name="RecipeLabel" Type="String" />
                </UpdateParameters>
                <InsertParameters>
                    <asp:Parameter Name="RecipeLabel" Type="String" />
                    <asp:Parameter Name="TargetVolumePerHour" Type="Single" />
                    <asp:Parameter Name="TargetPiecesPerHour" Type="Int32" />
                    <asp:Parameter Name="TargetLugFill" Type="Int16" />
                    <asp:Parameter Name="TargetUptime" Type="Int32" />
                </InsertParameters>
            </asp:SqlDataSource>
        </asp:Panel>
    </div>
    <div class="card mt-5" style="display:grid; grid-gap:1rem; grid-template-columns:200px; max-width:200px;">
        <asp:Label ID="Label2" runat="server" Text="Trimloss Factor:"></asp:Label>
        <asp:TextBox ID="TextBoxtlf" runat="server" OnTextChanged="TextBoxtlf_TextChanged"></asp:TextBox>
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Save" class="btn-raptor" />
    </div>    
</asp:Content>