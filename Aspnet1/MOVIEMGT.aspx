<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MOVIEMGT.aspx.cs" Inherits="Aspnet1.MOVIEMGT" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>MOVIE MANAGEMENT</h3>
    
    <asp:Label ID="messagelbl" runat="server" CssClass="alert alert-success" Visible="false"></asp:Label><br />
    <asp:Label ID="errorlbl" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label><br />

    <asp:Label ID="Label2" runat="server" Text="Movie Name"></asp:Label>
    <asp:TextBox ID="nameBox" runat="server" CssClass="form-control"></asp:TextBox><br />

    <asp:Label ID="Label3" runat="server" Text="Director"></asp:Label>
    <asp:TextBox ID="directorBox" runat="server" CssClass="form-control"></asp:TextBox><br />

    <asp:Label ID="Label4" runat="server" Text="Description"></asp:Label>
    <asp:TextBox ID="descriptionBox" runat="server" CssClass="form-control"></asp:TextBox><br />

    <asp:Label ID="Label5" runat="server" Text="Rating"></asp:Label>
    <asp:TextBox ID="ratingBox" runat="server" CssClass="form-control"></asp:TextBox><br />

    <asp:Button ID="saveBtn" runat="server" CssClass="btn btn-success" Text="Save" OnClick="saveBox_Click" />
    <asp:Button ID="editBtn" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="editBtn_Click" />
    <asp:Button ID="deleteBtn" runat="server" CssClass="btn btn-danger" Text="Delete Selected"
                OnClientClick="return confirm('Are you sure you want to delete this movie?');"
                OnClick="deleteBtn_Click" /><br /><br />

    <asp:GridView 
        ID="movieGridView" 
        runat="server" 
        CssClass="table table-striped"
        AutoGenerateColumns="False"
        DataKeyNames="movieId"
        OnSelectedIndexChanged="movieGridView_SelectedIndexChanged"
        AutoGenerateSelectButton="True">
        <Columns>
            <asp:BoundField DataField="movieId" HeaderText="ID" ReadOnly="true" />
            <asp:BoundField DataField="name" HeaderText="Name" />
            <asp:BoundField DataField="director" HeaderText="Director" />
            <asp:BoundField DataField="description" HeaderText="Description" />
            <asp:BoundField DataField="rating" HeaderText="Rating" />
        </Columns>
    </asp:GridView>
</asp:Content>
