<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reset.aspx.cs" Inherits="Lithnet.ResourceManagement.UI.AssistedPasswordReset.Reset" UICulture="auto" Culture="auto" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title runat="server" id="pageTitle">User verification</title>
    <link rel="stylesheet" href="styles.css" />
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <ajaxToolkit:ModalPopupExtender runat="server" ID="ModalPopupExtender1"
            TargetControlID="hiddenplaceholder"
            PopupControlID="popup"
            BackgroundCssClass="ModalBackground"
            CancelControlID="btCancel"
            RepositionMode="RepositionOnWindowResizeAndScroll">
        </ajaxToolkit:ModalPopupExtender>

        <div class="contentmain">
            <div class="wrapper">
                <div id="header">
                    <img src="lithnet16.png" alt="Lithnet" />
                </div>
                <h1>
                    <asp:Label ID="lbHeader" runat="server" />
                </h1>

                <div class="formcontent">
                    <asp:Table runat="server" ID="attributeTable" />

                    <div>
                        <asp:CheckBox ID="ckUserMustChangePassword" runat="server" Text="User must change password at next login" />
                        <asp:RadioButtonList runat="server" ID="opSetMode" ClientIDMode="Static">
                            <asp:ListItem Text="Automatically generate new password" Selected="True" Value="1" />
                            <asp:ListItem Text="Specify a password" Value="2" />
                        </asp:RadioButtonList>
                    </div>

                    <asp:Panel ID="panelSpecifyPassword" runat="server">
                        <div>
                            <table>
                                <tr>
                                    <td>New password:</td>
                                    <td>

                                        <asp:TextBox ID="txNewPassword1" runat="server" TextMode="Password" />
                                        <asp:RequiredFieldValidator ID="validatortxNewPassword1" runat="server"
                                            ControlToValidate="txNewPassword1"
                                            CssClass="ValidationError"
                                            ErrorMessage="&laquo; (Required)"
                                            ToolTip="User Name is a REQUIRED field"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Confirm new password:</td>
                                    <td>
                                        <asp:TextBox ID="txNewPassword2" runat="server" TextMode="Password" />
                                        <asp:RequiredFieldValidator ID="validatortxNewPassword2" runat="server"
                                            ControlToValidate="txNewPassword2"
                                            CssClass="ValidationError"
                                            ErrorMessage="&laquo; (Required)"
                                            ToolTip="User Name is a REQUIRED field"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="txNewPasswordCompareValidator" runat="server"
                                            ControlToValidate="txNewPassword2"
                                            CssClass="ValidationError"
                                            ControlToCompare="txNewPassword1"
                                            ErrorMessage="The passwords do not match"
                                            ToolTip="Password must be the same" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>

                    <div id="divWarning" class="warning" runat="server">
                        <asp:Label ID="lbWarning" runat="server" />
                    </div>

                    <asp:Table runat="server" ID="tableNewPassword">
                        <asp:TableRow>
                            <asp:TableHeaderCell>
                                <asp:Label runat="server" ID="lbNewPasswordCaption" />
                            </asp:TableHeaderCell>
                            <asp:TableCell>
                                <asp:Label ID="lbNewPassword" class="password" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>

                    <div id="divPasswordSetMessage" runat="server">
                        <asp:Label ID="lbPasswordSetMessage" runat="server" />
                    </div>

                    <asp:Button ID="btReset" runat="server" OnClick="btReset_Click" CssClass="button" />

                </div>
            </div>
        </div>


        <asp:Panel ID="popup" runat="server" DefaultButton="btAuthN" Style="display: none">
            <div class="popupcontent">
                <h1>
                    <asp:Label ID="lbCredentialPromptHeader" runat="server" meta:resourcekey="lbCredentialPromptHeader" />
                </h1>
                <div class="formcontent">
                    <asp:LinkButton ID="hiddenplaceholder" runat="server" />
                    <asp:Table runat="server">
                        <asp:TableRow>
                            <asp:TableHeaderCell>
                                <asp:Label ID="lbAuthNUsername" runat="server" meta:resourcekey="lbAuthNUsername" />
                            </asp:TableHeaderCell>
                            <asp:TableCell>
                                <asp:TextBox ID="txAuthNUsername" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableHeaderCell>
                                <asp:Label ID="lbAuthNPassword" runat="server" meta:resourcekey="lbAuthNPassword" />
                            </asp:TableHeaderCell>
                            <asp:TableCell>
                                <asp:TextBox ID="txAuthNPassword" TextMode="Password" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <div id="divAuthNError" class="warning" runat="server">
                        <asp:Label ID="lbAuthNError" runat="server" Text="error" />
                    </div>

                    <asp:Button ID="btAuthN" runat="server" OnClick="btAuthN_OnClick" meta:resourcekey="btAuthN" CssClass="button" />
                    <asp:Button ID="btCancel" Text="Cancel" runat="server" OnClick="btCancel_OnClick" CssClass="button" />
                </div>
            </div>
        </asp:Panel>
    </form>

    <script>

        window.onload = function () {
            var inputs = document.getElementsByName("opSetMode");

            if (inputs.length > 0) {
                for (var i = 0; i < inputs.length; i++) {
                    if (inputs[i].type == "radio") {
                        inputs[i].onclick = function () {
                            SetControlState(this);
                        }

                        SetControlState(inputs[i]);
                    }
                }
            }
        }

        function SetControlState(control) {
            if (control.value == "2" && control.checked) {
                ShowControls();
            }
            else {
                HideControls();
            }
        }

        function HideControls() {
            document.getElementById("<%=this.panelSpecifyPassword.ClientID%>").style.display = "none";
            document.getElementById("<%=this.validatortxNewPassword1.ClientID%>").enabled = false;
            document.getElementById("<%=this.validatortxNewPassword2.ClientID%>").enabled = false;
            document.getElementById("<%=this.txNewPasswordCompareValidator.ClientID%>").enabled = false;
        }

        function ShowControls() {
            document.getElementById("<%=this.panelSpecifyPassword.ClientID%>").style.display = "block";
            document.getElementById("<%=this.validatortxNewPassword1.ClientID%>").enabled = true;
            document.getElementById("<%=this.validatortxNewPassword2.ClientID%>").enabled = true;
            document.getElementById("<%=this.txNewPasswordCompareValidator.ClientID%>").enabled = true;
        }
    </script>
</body>
</html>
