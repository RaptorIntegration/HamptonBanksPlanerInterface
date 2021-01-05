<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Security.aspx.cs" Inherits="WebSort.Securuty" MasterPageFile="~/WebSort.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .label-text-grid {
            display:grid; 
            grid-gap:1rem; 
            grid-template-columns: 150px 200px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"
        ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
        SelectCommand="SELECT [UserName] FROM [Users] ORDER BY [UserID]"></asp:SqlDataSource>

    <div style="display:grid; grid-gap: 1rem; grid-template-columns: repeat(auto-fit, minmax(300px, 500px)); width: 100%;">
        <div style="display:grid; grid-gap: 1rem; grid-template-rows: auto;">
            <div class="card d-flex justify-content-center align-items-center">
                <asp:Label ID="Label15" runat="server" Text="User:" class="mr-3"></asp:Label>
                <asp:DropDownList ID="DropDownListUsers" runat="server"
                    DataSourceID="SqlDataSource1" DataTextField="UserName"
                    DataValueField="UserName" Font-Size="Medium" Width="200px"
                    AutoPostBack="True"
                    OnSelectedIndexChanged="DropDownListUsers_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div class="card">
                <asp:Label ID="Label1" runat="server" Text="Edit Screen Access Per User:" ></asp:Label>
                <asp:GridView ID="GridView1" runat="server" 
                    AutoGenerateColumns="False"
                    DataSourceID="SqlDataSource2" 
                    OnRowCommand="GridView1_RowCommand"
                    CssClass="table mt-3" 
                    OnLoad="GridView1_Load">
                    <Columns>
                        <asp:BoundField DataField="screenname" HeaderText="Screen Name"
                            SortExpression="screenname" ReadOnly="True" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True"
                                    OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                                    <asp:ListItem Value="0">Read Only</asp:ListItem>
                                    <asp:ListItem Value="1">Read / Write</asp:ListItem>
                                    <asp:ListItem Value="2">No Access</asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                            <ItemStyle Width="100px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server"
                    ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>"
                    SelectCommand="SELECT distinct screenname FROM [SecurityScreenAccess] ORDER BY [ScreenName]"></asp:SqlDataSource>
            </div>
        </div>
        <div style="display:grid; grid-gap: 1rem; grid-template-rows: auto">
            <div class="card d-flex justify-content-center align-items-center">
                <asp:Label ID="Label6" runat="server" Text="Remove User:"></asp:Label>
                <asp:Button ID="ButtonDeleteUser" runat="server" CssClass="btn-raptor ml-3"
                    OnClick="ButtonDeleteUser_Click" Text="Delete User" />
            </div>
            <div class="card">
                <asp:Label ID="Label2" runat="server" Text="Add User:"></asp:Label>
                <div class="mt-2 label-text-grid">
                    <asp:Label ID="Label3" runat="server" Font-Size="Small" Text="New User Name:" ></asp:Label>
                    <asp:TextBox ID="TextBoxUserName" runat="server"></asp:TextBox>

                    <asp:Label ID="Label4" runat="server" Font-Size="Small" Text="Password:"></asp:Label>
                    <asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password"></asp:TextBox>

                    <asp:Label ID="Label5" runat="server" Font-Size="Small" Text="Confirm Password:"></asp:Label>   
                    <asp:TextBox ID="TextBoxPasswordConfirm" runat="server" TextMode="Password"></asp:TextBox>
                </div>                
                <asp:Button ID="ButtonAddUser" runat="server" OnClick="ButtonAddUser_Click" CssClass="btn-raptor mt-3"
                    Text="Create New User" UseSubmitBehavior="False" />
                <asp:CompareValidator ID="CompareValidator1" runat="server"
                    ControlToCompare="TextBoxPassword" ControlToValidate="TextBoxPasswordConfirm"
                    ErrorMessage="Passwords do not match"></asp:CompareValidator>
            </div>
            <div class="card">
                <asp:Label ID="Label11" runat="server" Text="Change Password:"></asp:Label>
                <div class="mt-2 label-text-grid">
                    <asp:Label ID="Label12" runat="server" Font-Size="Small" Text="Old  Password:"></asp:Label>
                    <asp:TextBox ID="TextBoxOldPassword" runat="server" TextMode="Password"></asp:TextBox>

                    <asp:Label ID="Label13" runat="server" Font-Size="Small" Text="New Password:"></asp:Label>
                    <asp:TextBox ID="TextBoxNewPassword" runat="server" TextMode="Password"></asp:TextBox>

                    <asp:Label ID="Label14" runat="server" Font-Size="Small" Text="Confirm Password:"></asp:Label>
                    <asp:TextBox ID="TextBoxnewPasswordConfirm" runat="server" TextMode="Password"></asp:TextBox>
                </div>  
                <asp:Button ID="ButtonAddUser1" runat="server" OnClick="ButtonAddUser1_Click" CssClass="btn-raptor mt-3"
                    Text="Change Password:" />
                <asp:CompareValidator ID="CompareValidator2" runat="server"
                    ControlToCompare="TextBoxNewPassword"
                    ControlToValidate="TextBoxnewPasswordConfirm"
                    ErrorMessage="Passwords do not match"></asp:CompareValidator>
            </div>
        </div>
    </div>    
</asp:Content>