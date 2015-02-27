<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TryAgain.aspx.cs" Inherits="BronzeWebUtils.TryAgain" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
    
        <asp:Button ID="btnHome" runat="server" onclick="btnHome_Click" Text="Home" />
        <asp:Button ID="btnTimer" runat="server" onclick="btnTimer_Click" 
            Text="Timer" />
        <br />
        <br />
        <br />
    
    </div>
    </form>
</body>
</html>
