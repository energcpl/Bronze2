<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckForms.aspx.cs" Inherits="BronzeWebUtils.CheckForms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server" Text="Job ID"></asp:Label>
        <asp:TextBox ID="txtJobID" runat="server"></asp:TextBox>
        <asp:Button ID="btnGo" runat="server" onclick="btnGo_Click" Text="GO" />
    
    </div>
    </form>
</body>
</html>
