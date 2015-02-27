<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddForm.aspx.cs" Inherits="BronzeWebUtils.AddForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <table class="style1">
            <tr>
                <td>
                    JOBID&nbsp;&nbsp; (400000)</td>
                <td>
                    visitid&nbsp;&nbsp; (200000)</td>
                <td>
                    BronzeGuid</td>
                <td>
                    FormType</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txt_jobid" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:TextBox ID="txt_visitid" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:TextBox ID="txtID" runat="server" Width="262px"></asp:TextBox>
                </td>
                <td>
                    <asp:DropDownList ID="ddlForms" runat="server">
                        <asp:ListItem>Work Statement</asp:ListItem>
                        <asp:ListItem>Service Record MAN Mercedes</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="btnGo" runat="server" onclick="btnGo_Click" Text="Add" />
                </td>
            </tr>
        </table>
        <br />
        <br />
    
    </div>
    </form>
</body>
</html>
