<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reset.aspx.cs" Inherits="Lithnet.ResourceManagement.UI.AssistedPasswordReset.Reset" UICulture="auto" Culture="auto" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title runat="server" id="pageTitle"><asp:Literal runat="server" Text="<%$Resources:PageTitle%>"></asp:Literal></title>
    <link rel="stylesheet" href="styles.css" />
    <link rel="stylesheet" href="common-layout.css" />
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />


        <div class="main">
            <div class="wrapper">
                <div id="header" class="lithnet-header" >
                    <img src="lithnet16.png" alt="Lithnet" />
                </div>
                <h1>
                    <asp:Label ID="lbHeader" runat="server" Text="<%$Resources:PageTitle%>" />
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
                                                Text="<%$Resources:AutomaticallyGeneratePassword%>"
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
                                                Text="<%$Resources:SpecifyPassword%>"
                                                OnCheckedChanged="opSetMode_SelectedIndexChanged" />
                                        </td>
                                    </tr>
                                </table>

                                <asp:Panel ID="panelSpecifyPassword" runat="server" Visible="false">
                                    <table id="tableSpecifyPassword" class="credentialTable">
                                        <tr>
                                            <th>
                                                <asp:Literal runat="server" Text="<%$Resources:NewPassword%>" /></th>
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
                                            <th>
                                                <asp:Literal runat="server" Text="<%$Resources:ConfirmPassword%>" /></th>
                                            <td>
                                                <asp:TextBox ID="txNewPassword2" runat="server" TextMode="Password" CssClass="fullWidthControl" />
                                            </td>
                                            <td class="validationMessageCell">
                                                <asp:RequiredFieldValidator ID="validatortxNewPassword2"
                                                    runat="server"
                                                    ControlToValidate="txNewPassword2"
                                                    CssClass="ValidationError"
                                                    ErrorMessage="<%$Resources:Required%>"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:CompareValidator ID="txNewPasswordCompareValidator"
                                                    runat="server"
                                                    ControlToValidate="txNewPassword2"
                                                    CssClass="ValidationError"
                                                    ControlToCompare="txNewPassword1"
                                                    ErrorMessage="<%$Resources:PasswordsDoNotMatch%>"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>

                                <asp:CheckBox ID="ckUserMustChangePassword"
                                    runat="server"
                                    Text="<%$Resources:UserMustChangePassword%>" />
                            </asp:Panel>

                            <div id="resultRow" runat="server">
                                <asp:Table runat="server" ID="tableGeneratedPassword" CssClass="dataTable">
                                    <asp:TableRow>
                                        <asp:TableHeaderCell>
                                            <asp:Label runat="server" ID="lbNewPasswordCaption" Text="<%$Resources:NewPassword %>" />
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
                                <asp:Button ID="btReset"
                                    runat="server"
                                    OnClick="btReset_Click"
                                    CssClass="button"
                                    Text="<%$Resources:ResetPassword%>" />
                                <asp:Button ID="btClose"
                                    runat="server"
                                    OnClientClick="ClosePage(); return false;"
                                    CssClass="button"
                                    Visible="true"
                                    Text="<%$Resources:Close%>" />
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
                            <asp:Label ID="lbCredentialPromptHeader" runat="server" Text="<%$Resources:AuthNRequiredHeader%>" />
                        </h1>
                        <div class="formcontent">
                            <asp:LinkButton ID="hiddenplaceholder" runat="server" />
                            <asp:Table runat="server" CssClass="credentialTable">
                                <asp:TableRow>
                                    <asp:TableHeaderCell>
                                      <asp:Literal runat="server" Text="<%$Resources:AuthNUsername%>" />
                                    </asp:TableHeaderCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="txAuthNUsername" runat="server" CssClass="fullWidthControl" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableHeaderCell>
                                        <asp:Literal runat="server" Text="<%$Resources:AuthNPassword%>" />
                                    </asp:TableHeaderCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="txAuthNPassword" TextMode="Password" runat="server" CssClass="fullWidthControl" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                            <div id="divAuthNError" class="warning" runat="server">
                                <asp:Label ID="lbAuthNError" runat="server"  />
                            </div>

                            <div class="buttonRow">
                                <asp:Button ID="btAuthN" runat="server" OnClick="btAuthN_OnClick" Text="<%$Resources:OK%>" CssClass="button" />
                                <asp:Button ID="btCancel" Text="<%$Resources:Cancel%>" runat="server" OnClick="btCancel_OnClick" CssClass="button" />
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
        function ClosePage() {
            open(location, '_self').close();
            return false;
        }

        function ResetPage() {
            window.location.href = "<%=this.Request.RawUrl%>";
        }
    </script>
</body>
</html>
