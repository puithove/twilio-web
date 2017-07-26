<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="webapp._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript">
            function ChangeLabel()
            {
                var messageStatus = document.getElementById('messageStatus');
                messageStatus.innerHTML = 'Sending...';
                var messageError = document.getElementById('messageError');
                messageError.innerHTML = '';
            }
        </script>
        <div>
            From Number:<br />
            <br />
            <asp:DropDownList ID="fromDropDown" runat="server" AutoPostBack="True" OnSelectedIndexChanged="fromDropDown_SelectedIndexChanged">
            </asp:DropDownList>
            <br />
            <br />
        </div>
        <p>
            To Distro:</p>
        <p>
            <asp:ListBox ID="toListBox" runat="server" OnSelectedIndexChanged="toListBox_SelectedIndexChanged" SelectionMode="Multiple" AutoPostBack="True" Height="120px"></asp:ListBox>
            <asp:ListBox ID="contactsBox" runat="server" Height="120px" SelectionMode="Multiple"></asp:ListBox>
        </p>
        <p>
            Message Body</p>
        <p>
            <asp:TextBox ID="msgBody" runat="server" Height="59px" TextMode="MultiLine" Width="289px" OnTextChanged="msgBody_TextChanged"></asp:TextBox>
        </p>
        <p>
            <asp:Button ID="sendText" runat="server" OnClick="sendText_Click" Text="Send Text" OnClientClick="return ChangeLabel()" />
            <asp:Button ID="sendEmail" runat="server" OnClick="sendEmail_Click" Text="Send E-Mail Test" OnClientClick="return ChangeLabel()" />
            <asp:Button ID="resetButton" runat="server" OnClick="resetButton_Click" Text="Reset" />
        </p>
        <p>
            <asp:Label ID="messageStatus" runat="server" ForeColor="Red"></asp:Label>
        </p>
        <asp:Label ID="messageError" runat="server" ForeColor="Red"></asp:Label>
        <br />
        <br />
        <asp:Label ID="resultsLabel" runat="server" Text="Results:"></asp:Label>
        <br />
        <br />
        <asp:GridView ID="outputGrid" runat="server" OnSelectedIndexChanged="outputGrid_SelectedIndexChanged">
        </asp:GridView>
    </form>
</body>
</html>
