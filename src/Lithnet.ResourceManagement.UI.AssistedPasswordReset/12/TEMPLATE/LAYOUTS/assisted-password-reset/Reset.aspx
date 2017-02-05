<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reset.aspx.cs" Inherits="Lithnet.ResourceManagement.UI.AssistedPasswordReset.Reset" UICulture="auto" Culture="auto" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title runat="server" id="pageTitle"></title>
    <link rel="stylesheet" href="styles.css" />
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />


        <div class="contentmain">
            <div class="wrapper">
                <div id="header">
                    <img src="lithnet16.png" alt="Lithnet" />
                </div>
                <h1>
                    <asp:Label ID="lbHeader" runat="server" />
                </h1>

                <div class="formcontent">
                    <asp:Table runat="server" ID="attributeTable" CssClass="dataTable" />

                    <asp:UpdatePanel runat="server" ID="up2" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="passwordOptions">
                                <table id="opSetMode" runat="server">
                                    <tr>
                                        <td>
                                            <asp:RadioButton
                                                ID="opPasswordGenerate"
                                                GroupName="passwordSetMode"
                                                AutoPostBack="true"
                                                runat="server"
                                                Checked="true"
                                                Text="Automatically generate password"
                                                OnCheckedChanged="opSetMode_SelectedIndexChanged" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:RadioButton
                                                ID="opPasswordSpecify"
                                                GroupName="passwordSetMode"
                                                AutoPostBack="true"
                                                runat="server"
                                                Text="Specify a password"
                                                OnCheckedChanged="opSetMode_SelectedIndexChanged" />
                                        </td>
                                    </tr>
                                </table>

                                <asp:Panel ID="panelSpecifyPassword" runat="server" Visible="false">
                                    <table id="tableSpecifyPassword" class="credentialTable">
                                        <tr>
                                            <th>New password</th>
                                            <td>
                                                <asp:TextBox ID="txNewPassword1" runat="server" TextMode="Password" CssClass="fullWidthControl" />
                                            </td>
                                            <td class="validationMessageCell">
                                                <asp:RequiredFieldValidator ID="validatortxNewPassword1" runat="server"
                                                    ControlToValidate="txNewPassword1"
                                                    CssClass="ValidationError"
                                                    ErrorMessage="Required"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Confirm password</th>
                                            <td>
                                                <asp:TextBox ID="txNewPassword2" runat="server" TextMode="Password" CssClass="fullWidthControl" />
                                            </td>
                                            <td class="validationMessageCell">
                                                <asp:RequiredFieldValidator ID="validatortxNewPassword2" runat="server"
                                                    ControlToValidate="txNewPassword2"
                                                    CssClass="ValidationError"
                                                    ErrorMessage="Required"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:CompareValidator ID="txNewPasswordCompareValidator" runat="server"
                                                    ControlToValidate="txNewPassword2"
                                                    CssClass="ValidationError"
                                                    ControlToCompare="txNewPassword1"
                                                    ErrorMessage="The passwords do not match"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>

                                <asp:CheckBox ID="ckUserMustChangePassword"
                                    runat="server"
                                    Text="User must change password at next login" />
                            </asp:Panel>

                            <div id="resultRow" runat="server">
                                <asp:Table runat="server" ID="tableGeneratedPassword" CssClass="dataTable">
                                    <asp:TableRow>
                                        <asp:TableHeaderCell>
                                            <asp:Label runat="server" ID="lbNewPasswordCaption" />
                                        </asp:TableHeaderCell><asp:TableCell>
                                            <asp:Label ID="lbNewPassword" runat="server" CssClass="password" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <div id="divPasswordSetMessage" runat="server">
                                    <asp:Label ID="lbPasswordSetMessage" runat="server" />
                                </div>

                                <div id="divWarning" class="warning" runat="server">
                                    <asp:Label ID="lbWarning" runat="server" />
                                </div>
                            </div>

                            <div class="buttonRow">

                                <asp:Button ID="btStartAgain" runat="server" OnClientClick="ResetPage(); return false;" CssClass="button" Visible="false" Text="Start again" />
                                <asp:Button ID="btReset" runat="server" OnClick="btReset_Click" CssClass="button" />

                            </div>

                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btReset" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
            <ContentTemplate>
                <ajaxToolkit:ModalPopupExtender runat="server" ID="ModalPopupExtender1"
                    TargetControlID="hiddenplaceholder"
                    PopupControlID="popup"
                    BackgroundCssClass="ModalBackground"
                    CancelControlID="btCancel"
                    RepositionMode="RepositionOnWindowResizeAndScroll">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="popup" runat="server" DefaultButton="btAuthN" Style="display: none">
                    <div class="popupcontent">
                        <h1>
                            <asp:Label ID="lbCredentialPromptHeader" runat="server" meta:resourcekey="lbCredentialPromptHeader" />
                        </h1>
                        <div class="formcontent">
                            <asp:LinkButton ID="hiddenplaceholder" runat="server" />
                            <asp:Table runat="server" CssClass="credentialTable">
                                <asp:TableRow>
                                    <asp:TableHeaderCell>
                                        <asp:Label ID="lbAuthNUsername" runat="server" meta:resourcekey="lbAuthNUsername" />
                                    </asp:TableHeaderCell><asp:TableCell>
                                        <asp:TextBox ID="txAuthNUsername" runat="server" CssClass="fullWidthControl" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableHeaderCell>
                                        <asp:Label ID="lbAuthNPassword" runat="server" meta:resourcekey="lbAuthNPassword" />
                                    </asp:TableHeaderCell><asp:TableCell>
                                        <asp:TextBox ID="txAuthNPassword" TextMode="Password" runat="server" CssClass="fullWidthControl" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                            <div id="divAuthNError" class="warning" runat="server">
                                <asp:Label ID="lbAuthNError" runat="server" Text="error" />
                            </div>

                            <div class="buttonRow">
                                <asp:Button ID="btAuthN" runat="server" OnClick="btAuthN_OnClick" meta:resourcekey="btAuthN" CssClass="button" />
                                <asp:Button ID="btCancel" Text="Cancel" runat="server" OnClick="btCancel_OnClick" CssClass="button" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btReset" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
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

        function ResetPage() {
            window.location.href = "<%=this.Request.RawUrl%>";
        }

        function HideControls() {
            return;
            document.getElementById("<%=this.panelSpecifyPassword.ClientID%>").style.display = "none";
            document.getElementById("<%=this.validatortxNewPassword1.ClientID%>").enabled = false;
            document.getElementById("<%=this.validatortxNewPassword2.ClientID%>").enabled = false;
            document.getElementById("<%=this.txNewPasswordCompareValidator.ClientID%>").enabled = false;
        }

        function ShowControls() {
            return;
            document.getElementById("<%=this.panelSpecifyPassword.ClientID%>").style.display = "block";
            document.getElementById("<%=this.validatortxNewPassword1.ClientID%>").enabled = true;
            document.getElementById("<%=this.validatortxNewPassword2.ClientID%>").enabled = true;
            document.getElementById("<%=this.txNewPasswordCompareValidator.ClientID%>").enabled = true;
        }
    </script>
</body>
</html>
