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

        <div class="contentmain">
            <div class="wrapper">
                <div id="header">
                    <img src="lithnet16.png" alt="Lithnet" />
                </div>
                <h1>
                    <asp:Label ID="lbHeader" runat="server" meta:resourcekey="lbHeader"></asp:Label></h1>

                <div class="formcontent">

                    <asp:Table runat="server">
                        <asp:TableRow>
                            <asp:TableHeaderCell>
                                <asp:Label ID="lbCaptionUsername" runat="server" meta:resourcekey="lbCaptionUsername"></asp:Label>
                            </asp:TableHeaderCell>
                            <asp:TableCell>
                                <asp:Label ID="lbUser" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableHeaderCell>
                                <asp:Label ID="lbCaptionName" runat="server" meta:resourcekey="lbCaptionName"></asp:Label>
                            </asp:TableHeaderCell>
                            <asp:TableCell>
                                <asp:Label ID="lbName" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowSecurityCode" runat="server" Visible="false">
                            <asp:TableHeaderCell>
                                <asp:Label ID="lbCaptionSecurityCode" runat="server" meta:resourcekey="lbCaptionSecurityCode"></asp:Label>
                            </asp:TableHeaderCell><asp:TableCell>
                                <asp:Label ID="lbSecurityCode" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>


                    <div id="divWarning" class="warning" runat="server">
                        <asp:Label ID="lbWarning" runat="server" Text="error" />
                    </div>

                    <asp:Button ID="btSend" runat="server" OnClick="btSend_Click" meta:resourcekey="btSend" CssClass="button" />
                </div>
            </div>
        </div>



        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <ajaxToolkit:ModalPopupExtender runat="server" ID="ModalPopupExtender1"
            TargetControlID="hiddenplaceholder"
            PopupControlID="popup"
            BackgroundCssClass="ModalBackground"
            CancelControlID="btCancel"
            RepositionMode="RepositionOnWindowResizeAndScroll">
        </ajaxToolkit:ModalPopupExtender>

        <asp:Panel ID="popup" runat="server" DefaultButton="btAuthN">
            <div class="contentmain">
                <div class="wrapper">
                    <h1>
                        <asp:Label ID="lbCredentialPromptHeader" runat="server" meta:resourcekey="lbCredentialPromptHeader"></asp:Label></h1>

                    <div class="formcontent">
                        <asp:LinkButton ID="hiddenplaceholder" runat="server"/>
                        <asp:Table runat="server">
                            <asp:TableRow>
                                <asp:TableHeaderCell>
                                    <asp:Label ID="lbAuthNUsername" runat="server" meta:resourcekey="lbAuthNUsername"></asp:Label>
                                </asp:TableHeaderCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txAuthNUsername" runat="server"></asp:TextBox>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableHeaderCell>
                                    <asp:Label ID="lbAuthNPassword" runat="server" meta:resourcekey="lbAuthNPassword"></asp:Label>
                                </asp:TableHeaderCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="txAuthNPassword"   TextMode="Password" runat="server"></asp:TextBox>
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
            </div>
        </asp:Panel>
    </form>
</body>
</html>
