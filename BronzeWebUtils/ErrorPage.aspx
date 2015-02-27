<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="BronzeWebUtils.ErrorPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="XX-Large" 
            ForeColor="Red" Text="Error"></asp:Label>
        <br />
        <br />
        <asp:TextBox ID="txtError" runat="server" Height="300px" ReadOnly="True" 
            TextMode="MultiLine" Width="1380px"></asp:TextBox>
    
        <br />
        <asp:Button ID="btnBack" runat="server" onclick="btnBack_Click" 
            Text="Send A Job" />
        <asp:Button ID="btnHome" runat="server" onclick="btnHome_Click" Text="Home" />
        <br />
    
    </div>
    </form>
</body>
</html>
