<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotInBronze.aspx.cs" Inherits="BronzeWebUtils.NotInBronze" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Calendar ID="Calendar1" runat="server" SelectedDate="12/10/2014 10:43:51">
        </asp:Calendar>
        <asp:Button ID="btnGo" runat="server" onclick="Button1_Click" Text="GO" />
        <asp:Button ID="btnClear" runat="server" onclick="btnClear_Click" 
            Text="Clear Results" />
        <br />
        <br />
        <asp:GridView ID="gvJobs" runat="server" onrowcommand="GridView1_RowCommand">
            <Columns>
                <asp:CommandField HeaderText="Send" ShowHeader="True" ShowSelectButton="True" />
            </Columns>
        </asp:GridView>
        <br />
        <asp:Button ID="btnBack" runat="server" onclick="btnBack_Click" Text="Home" />
        <asp:Button ID="btnSendAll" runat="server" onclick="btnSendAll_Click" 
            Text="Send All" />
        <br />
        <br />
        *****************************<br />
        <asp:GridView ID="gvResult" runat="server">
        </asp:GridView>
    
    </div>
    </form>
</body>
</html>
