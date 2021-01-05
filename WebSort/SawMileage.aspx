<%@ Page Language="C#" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" MasterPageFile="~/WebSort.Master" CodeBehind="SawMileage.aspx.cs" Inherits="WebSort.SawMileage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:GridView ID="GridView1" runat="server"
                AutoGenerateColumns="False" DataSourceID="SqlDataSourceSawMileage"
                DataKeyNames="SawID" CssClass="table plc"
                OnRowCommand="GridView1_RowCommand"
                OnRowDataBound="GridView1_RowDataBound"
                OnRowCancelingEdit="GridView1_RowCancelingEdit" style="max-width: 1500px;"
                OnRowUpdated="GridView1_RowUpdated" OnRowUpdating="GridView1_RowUpdating"
                OnRowEditing="GridView1_RowEditing">
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <EditItemTemplate>
                            <asp:Button ID="Button1" runat="server" CausesValidation="True"
                                CommandName="Update" Text="Save" CssClass="btn-raptor" />
                            &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False"
                                CommandName="Cancel" Text="Cancel" CssClass="btn-raptor" />
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Button ID="Button1" runat="server" CausesValidation="False"
                                CommandName="Edit" Text="Edit" CssClass="btn-raptor" OnClick="Button1_Click" />

                            <asp:Button ID="Button2" runat="server" CausesValidation="False"
                                CommandName="ResetMileage" Text="Reset Mileage" CssClass="btn-raptor" CommandArgument='<%# Eval("id") %>' />
                            <asp:Button ID="Button3" runat="server" CausesValidation="False"
                                CommandName="ResetStrokes" Text="Reset Strokes" CssClass="btn-raptor" CommandArgument='<%# Eval("id") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SawID" HeaderText="SawID"
                        SortExpression="SawID" ReadOnly="True" Visible="False" />
                    <asp:BoundField DataField="SawDescription" HeaderText="Saw"
                        SortExpression="SawDescription" ReadOnly="True" />

                    <asp:BoundField DataField="Mileage" HeaderText="Mileage (ft)" ReadOnly="True"
                        SortExpression="Mileage" HtmlEncode="False" />
                    <asp:BoundField DataField="MileageThreshold" HeaderText="Mileage Threshold (ft)"
                        SortExpression="MileageThreshold" />
                    <asp:BoundField DataField="Strokes" HeaderText="Strokes"
                        SortExpression="Strokes" ReadOnly="True" />
                    <asp:BoundField DataField="StrokeThreshold" HeaderText="Stroke Threshold"
                        SortExpression="StrokeThreshold" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSourceSawMileage" runat="server"
                ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                SelectCommand="SELECT id,sawid,sawdescription,strokes,mileage=convert(int,mileage),strokethreshold,mileagethreshold FROM [SawMileage] ORDER BY [SawID]"
                DeleteCommand="DELETE FROM [SawMileage] WHERE [SawID] = @SawID"
                InsertCommand="INSERT INTO [SawMileage] ([SawID], [SawDescription], [Strokes], [Mileage], [Threshold]) VALUES (@SawID, @SawDescription, @Strokes, @Mileage, @Threshold)"
                UpdateCommand="UPDATE [SawMileage] SET  [StrokeThreshold] = @StrokeThreshold,[MileageThreshold] = @MileageThreshold  WHERE [SawID] = @SawID">
                <DeleteParameters>
                    <asp:Parameter Name="SawID" Type="Int16" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="SawDescription" Type="String" />
                    <asp:Parameter Name="Strokes" Type="Int32" />
                    <asp:Parameter Name="Mileage" Type="Int32" />
                    <asp:Parameter Name="StrokeThreshold" Type="Int32" />
                    <asp:Parameter Name="MileageThreshold" Type="Int32" />
                    <asp:Parameter Name="SawID" Type="Int16" />
                </UpdateParameters>
                <InsertParameters>
                    <asp:Parameter Name="SawID" Type="Int16" />
                    <asp:Parameter Name="SawDescription" Type="String" />
                    <asp:Parameter Name="Strokes" Type="Int32" />
                    <asp:Parameter Name="Mileage" Type="Int32" />
                    <asp:Parameter Name="Threshold" Type="Int32" />
                </InsertParameters>
            </asp:SqlDataSource>
            <asp:Timer ID="Timer2" runat="server" Interval="5000" OnTick="Timer2_Tick">
            </asp:Timer>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer2" EventName="Tick" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>