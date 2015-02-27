<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BronzeWebUtils.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bronze Help Site</title>
    <style type="text/css">
        .style1
        {
            width: 150px;
        }
        .style2
        {
            width: 76px;
        }
        .style3
        {
            width: 115px;
        }
        .style4
        {
            width: 135px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <script type="text/javascript" src="Menu.js"></script>
    <table>
        <tr>
            <td class="style1">
                &nbsp;</td>
            <td class="style1">
                </td>
            <td class="style3">
                &nbsp;</td>
            <td class="style4">
                &nbsp;</td>
            <td class="style1">
                &nbsp;</td>
            <td class="style2">
                &nbsp;</td>
            <td class=style1>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Button ID="btnWork" runat="server" onclick="btnWork_Click" 
                    Text="List Work orders" />
            </td>
            <td class="style1">
                <asp:Button ID="btnJob" runat="server" onclick="btnJob_Click" 
                    Text="Send A Job" />
            </td>
            <td class="style3">
                <asp:Button ID="btnNotInBronze" runat="server" onclick="btnNotInBronze_Click" 
                    Text="NOT in bronze" />
            </td>
            <td class="style4">
                <asp:Button ID="btnAddForm" runat="server" onclick="btnAddForm_Click" 
                    Text="Add form to ID" />
            </td>
            <td class="style1">
                <asp:Button ID="btnExport" runat="server" onclick="btnExport_Click" 
                    Text="Export to pdf" />
            </td>
            <td class="style2">
                <asp:Button ID="btnAddFrom" runat="server" onclick="btnAddFrom_Click" 
                    Text="Add Form" />
            </td>
            <td class=style1>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style1">
                &nbsp;</td>
            <td class="style1">
                &nbsp;</td>
            <td class="style3">
                &nbsp;</td>
            <td class="style4">
                &nbsp;</td>
            <td class="style1">
                &nbsp;</td>
            <td class="style2">
                <asp:Button ID="btnLogin" runat="server" Text="Login" onclick="btnLogin_Click" />
            </td>
            <td class=style1>
                <asp:Label ID="lblLoggedIn" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
