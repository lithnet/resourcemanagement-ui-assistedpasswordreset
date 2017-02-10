using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.UI.WebControls;
using Lithnet.ResourceManagement.Client;
using SD = System.Diagnostics;
using System.Linq;

namespace Lithnet.ResourceManagement.UI.AssistedPasswordReset
{
    public partial class Reset : System.Web.UI.Page
    {
        private string SidTarget
        {
            get
            {
                return (string)this.ViewState[nameof(this.SidTarget)];
            }
            set
            {
                this.ViewState[nameof(this.SidTarget)] = value;
            }
        }

        private bool HasCredentials
        {
            get
            {
                object value = this.ViewState[nameof(this.HasCredentials)];

                if (value == null)
                {
                    return false;
                }
                else
                {
                    return (bool)value;
                }
            }
            set
            {
                this.ViewState[nameof(this.HasCredentials)] = value;
            }
        }

        private string UserObjectID => this.Request.QueryString["id"];

        private string ObjectType => this.Request.QueryString["type"] ?? "Person";

        private string SpecifiedPassword
        {
            get
            {
                return (string)this.ViewState[nameof(this.SpecifiedPassword)];
            }
            set
            {
                this.ViewState[nameof(this.SpecifiedPassword)] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SD.Trace.WriteLine($"Loading page. IsPostBack: {this.Page.IsPostBack}. IsPartialPostBack: {System.Web.UI.ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack}");
                SD.Trace.WriteLine($"Loaded page as {System.Threading.Thread.CurrentPrincipal.Identity.Name} using {System.Threading.Thread.CurrentPrincipal.Identity.AuthenticationType} authentication");

                if (this.Page.IsPostBack)
                {

                    if (!System.Web.UI.ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
                    {
                        return;
                    }

                    return;
                }

                this.txAuthNUsername.Text = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                this.ckUserMustChangePassword.Checked = AppConfigurationSection.CurrentConfig.ForcePasswordChangeAtNextLogon || AppConfigurationSection.CurrentConfig.PasswordChangeAtNextLogonSetAsDefault;
                this.ckUserMustChangePassword.Enabled = !AppConfigurationSection.CurrentConfig.ForcePasswordChangeAtNextLogon;
                this.opSetMode.Visible = AppConfigurationSection.CurrentConfig.AllowSpecifiedPasswords;
                this.resultRow.Visible = false;
                this.divWarning.Visible = false;

                ResourceManagementClient c = new ResourceManagementClient();
                List<string> attributeList = new List<string>();
                attributeList.AddRange(AppConfigurationSection.CurrentConfig.DisplayAttributeList);
                attributeList.Add(AppConfigurationSection.CurrentConfig.ObjectSidAttributeName);

                ResourceObject o = c.GetResourceByKey(this.ObjectType, AppConfigurationSection.CurrentConfig.SearchAttributeName, this.UserObjectID, attributeList);

                if (o == null)
                {
                    this.SetError((string)this.GetLocalResourceObject("ErrorUserNotFound"));
                    return;
                }
                else
                {
                    SD.Trace.WriteLine($"Got resource {o.ObjectID} from resource management service");
                }

                this.BuildAttributeTable(o);

                if (!o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.ObjectSidAttributeName))
                {
                    throw new InvalidOperationException("The object type does not have a SID attribute");
                }

                if (o.Attributes[AppConfigurationSection.CurrentConfig.ObjectSidAttributeName].IsNull)
                {
                    this.SetError((string)this.GetLocalResourceObject("SIDNotFound"));
                }
                else
                {
                    this.SidTarget = new SecurityIdentifier(o.Attributes[AppConfigurationSection.CurrentConfig.ObjectSidAttributeName].BinaryValue, 0).ToString();
                    SD.Trace.WriteLine($"Set target set to {this.SidTarget}");
                }
            }
            catch (Exception ex)
            {
                SD.Trace.WriteLine($"Exception in page_load\n {ex}");
                this.SetError("An unexpected error occurred:\n" + ex);
            }
        }

        private void SetError(string message)
        {
            SD.Trace.WriteLine($"Setting error message: {message}");
            this.passwordOptions.Visible = false;
            this.divWarning.Visible = true;
            this.lbWarning.Text = message;
            this.btReset.Enabled = false;
            this.btReset.Visible = false;
            this.resultRow.Visible = true;
            this.tableGeneratedPassword.Visible = false;
            this.divPasswordSetMessage.Visible = false;
        }

        private void BuildAttributeTable(ResourceObject o)
        {
            foreach (string attributeName in AppConfigurationSection.CurrentConfig.DisplayAttributeList)
            {
                string value;

                AttributeValue attribute = o.Attributes[attributeName];

                if (attribute.IsNull)
                {
                    value = null;
                }
                else
                {
                    value = string.Join("<br/>", attribute.ValuesAsString);
                }

                this.AddRowToTable(LocalizationHelper.GetLocalizedName(attributeName, this.ObjectType), value);
            }
        }

        private void AddRowToTable(string header, string value)
        {
            int rowCount = this.attributeTable.Rows.Count;

            if (value == null && !AppConfigurationSection.CurrentConfig.ShowNullAttributes)
            {
                SD.Trace.WriteLine($"Ignoring row add request for {header} as its value was null");
                return;
            }

            TableRow row = new TableRow { ID = $"row{rowCount}" };

            row.Cells.Add(new TableHeaderCell
            {
                Text = header,
                ID = $"th{rowCount}"
            });

            row.Cells.Add(new TableCell
            {
                Text = value,
                ID = $"tc{rowCount}"
            });

            this.attributeTable.Rows.Add(row);

            SD.Trace.WriteLine($"Row {rowCount} added");
        }


        private PrincipalContext GetPrincipalContext(bool forcePrompt)
        {
            if (this.HasCredentials)
            {
                SD.Trace.WriteLine($"Get domain context with explicit credentials for {this.txAuthNUsername.Text}");
                return new PrincipalContext(ContextType.Domain, null, null, ContextOptions.Negotiate, this.txAuthNUsername.Text, this.txAuthNPassword.Text);
            }

            if (AppConfigurationSection.CurrentConfig.AlwaysPromptForAdminPassword || forcePrompt)
            {
                SD.Trace.WriteLine($"Prompting for credentials");
                this.divAuthNError.Visible = false;
                this.ModalPopupExtender1.Show();
                this.txAuthNPassword.Focus();
                this.validatortxNewPassword1.Enabled = false;
                this.validatortxNewPassword2.Enabled = false;
                this.txNewPasswordCompareValidator.Enabled = false;
                return null;
            }
            else
            {
                SD.Trace.WriteLine($"Creating context with current credentials");
                return new PrincipalContext(ContextType.Domain, null, null, ContextOptions.Negotiate);
            }
        }

        private bool ValidateTarget()
        {
            if (this.SidTarget == null)
            {
                SD.Trace.WriteLine($"No sid target was set. Aborting reset operation");
                this.SetError((string)this.GetLocalResourceObject("ErrorNoSidTargetAvailable"));
                return false;
            }

            return true;
        }

        private UserPrincipal GetUserPrincipal(PrincipalContext context, bool canRetry = true)
        {
            try
            {
                SD.Trace.WriteLine($"Searching for user {this.SidTarget}");

                UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.Sid, this.SidTarget);

                if (user == null)
                {
                    SD.Trace.WriteLine($"The SID was not found in the directory. Aborting reset operation");
                    this.SetError((string)this.GetLocalResourceObject("ErrorUserNotFound"));
                    return null;
                }

                return user;
            }
            catch (DirectoryServicesCOMException ex)
            {
                SD.Trace.WriteLine($"Directory exception encountered: {ex}");

                if (!canRetry || !AppConfigurationSection.CurrentConfig.AllowPasswordPromptFallback)
                {
                    SD.Trace.WriteLine("Cannot retry, rethrowing exception");
                    throw;
                }

                if (ex.ErrorCode == unchecked((int)0x80072020))
                {
                    SD.Trace.WriteLine("Handling operations error by requesting explicit credentials");

                    context = this.GetPrincipalContext(true);

                    if (context == null)
                    {
                        SD.Trace.WriteLine("Credentals pending");
                        return null;
                    }
                    else
                    {
                        SD.Trace.WriteLine("Got context, getting principal");
                        return this.GetUserPrincipal(context, false);
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        private void SetPassword()
        {
            string password;

            if (this.opPasswordSpecify.Checked && AppConfigurationSection.CurrentConfig.AllowSpecifiedPasswords)
            {
                if (this.txNewPassword1.Text != this.txNewPassword2.Text)
                {
                    this.SetError((string)this.GetLocalResourceObject("ErrorMessagePasswordsDoNotMatch"));
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.txNewPassword1.Text) && this.SpecifiedPassword == null)
                {
                    this.SetError((string)this.GetLocalResourceObject("ErrorMessageBlankPassword"));
                    return;
                }

                if (string.IsNullOrEmpty(this.SpecifiedPassword))
                {
                    password = this.txNewPassword1.Text;
                    this.SpecifiedPassword = password;
                }
                else
                {
                    password = this.SpecifiedPassword;
                }

                SD.Trace.WriteLine("Using specified password");
            }
            else
            {
                password = RandomValueGenerator.GenerateRandomString(AppConfigurationSection.CurrentConfig.GeneratedPasswordLength);
                SD.Trace.WriteLine("Using generated password");
            }

            this.SetPassword(password);
        }

        private void SetPassword(string password)
        {
            try
            {
                if (!this.ValidateTarget())
                {
                    SD.Trace.WriteLine("Target validation failed. Aborting");
                    return;
                }

                PrincipalContext context = this.GetPrincipalContext(false);

                if (context == null)
                {
                    SD.Trace.WriteLine("Did not get a principal context. Aborting");
                    return;
                }

                using (context)
                {
                    UserPrincipal user = this.GetUserPrincipal(context);

                    if (user == null)
                    {
                        SD.Trace.WriteLine("Did not get a user context. Aborting");
                        return;
                    }

                    SD.Trace.WriteLine($"Got user context {user.SamAccountName}");

                    using (user)
                    {
                        SD.Trace.WriteLine($"Attempting to set password");
                        user.SetPassword(password);
                        SD.Trace.WriteLine($"Password set");

                        if (this.ckUserMustChangePassword.Checked || AppConfigurationSection.CurrentConfig.ForcePasswordChangeAtNextLogon)
                        {
                            user.ExpirePasswordNow();
                            SD.Trace.WriteLine($"Password set to require change on next login");
                        }

                        this.resultRow.Visible = true;

                        if (this.opPasswordSpecify.Checked)
                        {
                            this.tableGeneratedPassword.Visible = false;
                            this.divPasswordSetMessage.Visible = true;
                            this.lbPasswordSetMessage.Text = (string) GetLocalResourceObject("PasswordSetSucessfully");
                        }
                        else
                        {
                            this.divPasswordSetMessage.Visible = false;
                            this.tableGeneratedPassword.Visible = true;
                            this.lbNewPassword.Text = password;
                        }

                        this.passwordOptions.Visible = false;

                        this.opPasswordGenerate.Checked = true;
                    }
                }

                this.btReset.Visible = false;
                this.SpecifiedPassword = null;
                this.up2.Update();
            }
            catch (Exception ex)
            {
                SD.Trace.WriteLine($"Exception setting password for {this.SidTarget}\n {ex}");
                this.SetError($"{(string)this.GetLocalResourceObject("ErrorMessagePasswordSetFailure")} {ex}");
                this.SpecifiedPassword = null;
            }
            finally
            {
                this.HasCredentials = false;
                this.txAuthNPassword.Text = null;
            }
        }

        protected void btReset_Click(object sender, EventArgs e)
        {
            this.SetPassword();
        }

        private bool ValidateCredentials()
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                SD.Trace.WriteLine($"Attempting to validate {this.txAuthNUsername.Text}");
                return context.ValidateCredentials(this.txAuthNUsername.Text, this.txAuthNPassword.Text);
            }
        }

        protected void btAuthN_OnClick(object sender, EventArgs e)
        {
            if (this.ValidateCredentials())
            {
                SD.Trace.WriteLine("Explicit credentials validated");
                this.divAuthNError.Visible = false;
                this.HasCredentials = true;
                this.ModalPopupExtender1.Hide();
                this.SetPassword();
            }
            else
            {
                SD.Trace.WriteLine("Credentials did not validate");
                this.divAuthNError.Visible = true;
                this.lbAuthNError.Text = (string)this.GetLocalResourceObject("ErrorInvalidUsernameOrPassword");
                this.txAuthNPassword.Text = null;
                this.txAuthNUsername.Focus();
                this.ModalPopupExtender1.Show();
            }
        }

        protected void btCancel_OnClick(object sender, EventArgs e)
        {
            this.txAuthNPassword.Text = null;
            this.divAuthNError.Visible = false;
            this.HasCredentials = false;
            this.ModalPopupExtender1.Hide();
        }

        protected void opSetMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.panelSpecifyPassword.Visible = this.opPasswordSpecify.Checked;
        }
    }
}