<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendAJob.aspx.cs" Inherits="BronzeWebUtils.SendAJob" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server" Text="Enter Job Number ..."></asp:Label>
        <asp:TextBox ID="txtJobId" runat="server"></asp:TextBox>
        <asp:Button ID="btnShow" runat="server" onclick="btnShow_Click" Text="Show" />
        <asp:Label ID="lblError" runat="server"></asp:Label>
        <br />
        <br />
        <asp:GridView ID="gvJobs" runat="server" AutoGenerateSelectButton="True" 
            CellPadding="4" ForeColor="#333333" GridLines="None" 
            onrowcommand="gvJobs_RowCommand">
            <AlternatingRowStyle BackColor="White" />
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
    
        <br />
        <asp:Button ID="btnSendTomTom" runat="server" onclick="btnSendTomTom_Click" 
            Text="Send to tomtom" />
        <asp:Button ID="btnDelTomTom" runat="server" onclick="btnDelTomTom_Click" 
            Text="Delete from TomTom" />
    
        <br />
        <asp:Button ID="btnBack" runat="server" onclick="btnBack_Click" Text="Home" />
    
    </div>
    </form>
</body>
</html>
