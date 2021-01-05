<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebSort.Login" MasterPageFile="~/WebSort.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-12">
            <h2>Please Login</h2>
        </div>
    </div>
    <div class="row">
        <div class="col-12 col-lg-3 col-md-6 col-sm-12">
            <label>Username</label>
            <asp:DropDownList ID="DropDownListUserName" runat="server" 
                CssClass="form-control" 
                DataSourceID="SqlDataSource1" 
                DataTextField="UserName" 
                DataValueField="UserName">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                ConnectionString="<%$ ConnectionStrings:RaptorWebSortConnectionString %>" 
                SelectCommand="SELECT [UserName] FROM [Users] where userid>0 ORDER BY UserID">
            </asp:SqlDataSource>
        </div>
    </div>
    <div class="row mt-5">
        <div class="col-12 col-lg-3 col-md-6 col-sm-12">
            <label>Password</label>
            <asp:TextBox ID="TextBoxPassword" runat="server" 
                TextMode="Password" 
                CssClass="form-control" autocomplete="new-password current-password">
            </asp:TextBox>
        </div>
    </div>
    <div class="row mt-5">
        <div class="col-12 col-lg-3 col-md-6 col-sm-12">
            <asp:Button ID="ButtonLogin" runat="server" 
                CssClass="btn-raptor" 
                Text="Login" 
                onclick="ButtonLogin_Click" />
        </div>
    </div>
    <div class="row mt-5">
        <div class="col-12">
            <asp:UpdatePanel runat="server" ID="UpdatePanelError">
                <ContentTemplate>
                    <asp:Label ID="LabelCredentialsWarning" runat="server" 
                        Text="The credentials you have entered are invalid." 
                        CssClass="error-label" 
                        Visible="False">
                    </asp:Label>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ButtonLogin" />
                </Triggers>
            </asp:UpdatePanel>           
        </div>
    </div>
</asp:Content>