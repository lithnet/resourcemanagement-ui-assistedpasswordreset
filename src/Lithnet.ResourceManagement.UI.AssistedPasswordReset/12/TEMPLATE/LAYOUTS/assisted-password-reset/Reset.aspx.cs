using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Management;
using System.Security.Principal;
using System.Web.UI.WebControls;
using Lithnet.ResourceManagement.Client;
using SD = System.Diagnostics;
using System.Reflection;

namespace Lithnet.ResourceManagement.UI.AssistedPasswordReset
{
    public partial class Reset : System.Web.UI.Page
    {
        private string SidTarget
        {
            get => (string)this.ViewState[nameof(this.SidTarget)];
            set => this.ViewState[nameof(this.SidTarget)] = value;
        }

        private string UserName
        {
            get => (string)this.ViewState[nameof(this.UserName)];
            set => this.ViewState[nameof(this.UserName)] = value;
        }

        private string UserDomain
        {
            get => (string)this.ViewState[nameof(this.UserDomain)];
            set => this.ViewState[nameof(this.UserDomain)] = value;
        }

        public string Fqdn => $"{this.UserDomain}\\{this.UserName}";

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
            set => this.ViewState[nameof(this.HasCredentials)] = value;
        }

        private string UserObjectID => this.Request.QueryString["id"];

        private string ObjectType => this.Request.QueryString["type"] ?? "Person";

        private string SpecifiedPassword
        {
            get => (string)this.ViewState[nameof(this.SpecifiedPassword)];
            set => this.ViewState[nameof(this.SpecifiedPassword)] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SD.Trace.WriteLine($"Loading page. IsPostBack: {this.Page.IsPostBack}. IsPartialPostBack: {System.Web.UI.ScriptManager.GetCurrent(this.Page)?.IsInAsyncPostBack}");
                SD.Trace.WriteLine($"Loaded page as {System.Threading.Thread.CurrentPrincipal.Identity.Name} using {System.Threading.Thread.CurrentPrincipal.Identity.AuthenticationType} authentication");

                if (this.Page.IsPostBack)
                {
                    return;
                }

                this.txAuthNUsername.Text = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                this.ckUserMustChangePassword.Checked = AppConfigurationSection.CurrentConfig.ForcePasswordChangeAtNextLogon || AppConfigurationSection.CurrentConfig.PasswordChangeAtNextLogonSetAsDefault;
                this.ckUserMustChangePassword.Enabled = !AppConfigurationSection.CurrentConfig.ForcePasswordChangeAtNextLogon;

                this.ckUnlockAccount.Visible = AppConfigurationSection.CurrentConfig.ShowUnlockAccount;
                this.ckUnlockAccount.Checked = AppConfigurationSection.CurrentConfig.ShowUnlockAccount && AppConfigurationSection.CurrentConfig.UnlockAccountSetAsDefault;

                this.opSetMode.Visible = AppConfigurationSection.CurrentConfig.AllowSpecifiedPasswords;
                this.resultRow.Visible = false;
                this.divWarning.Visible = false;

                ResourceManagementClient c = new ResourceManagementClient();

                ResourceObject o = c.GetResourceByKey(this.ObjectType, AppConfigurationSection.CurrentConfig.SearchAttributeName, this.UserObjectID, this.GetAttributeList());

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
                this.ValidateAttributes(o);
            }
            catch (Exception ex)
            {
                SD.Trace.WriteLine($"Exception in page_load\n {ex}");
                this.SetError("An unexpected error occurred:\n" + ex);
            }
        }

        private void ValidateAttributes(ResourceObject o)
        {
            if (AppConfigurationSection.CurrentConfig.UseWmi)
            {
                this.ValidateAttributesWmi(o);
            }
            else
            {
                this.ValidateAttributesLdap(o);
            }
        }


        private void ValidateAttributesWmi(ResourceObject o)
        {
            if (!o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.DomainAttributeName))
            {
                throw new InvalidOperationException("The object type does not have a Domain attribute");
            }

            if (!o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.AccountNameAttributeName))
            {
                throw new InvalidOperationException("The object type does not have an AccountName attribute");
            }

            if (o.Attributes[AppConfigurationSection.CurrentConfig.DomainAttributeName].IsNull)
            {
                SD.Trace.WriteLine($"No domain found on the user object, assuming current domain");
                this.UserDomain = Environment.UserDomainName;
            }
            else
            {
                this.UserDomain = o.Attributes[AppConfigurationSection.CurrentConfig.DomainAttributeName].StringValue;
            }

            if (o.Attributes[AppConfigurationSection.CurrentConfig.AccountNameAttributeName].IsNull)
            {
                SD.Trace.WriteLine($"No AccountName found on the user object");
                this.SetError((string)this.GetLocalResourceObject("ErrorUserNotFound"));
            }
            else
            {
                this.UserName = o.Attributes[AppConfigurationSection.CurrentConfig.AccountNameAttributeName].StringValue;
            }

            SD.Trace.WriteLine($"Set target to {this.UserDomain}\\{this.UserName}");
        }

        private void ValidateAttributesLdap(ResourceObject o)
        {
            if (!o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.ObjectSidAttributeName))
            {
                throw new InvalidOperationException("The object type does not have a SID attribute");
            }

            if (o.Attributes[AppConfigurationSection.CurrentConfig.ObjectSidAttributeName].IsNull)
            {
                SD.Trace.WriteLine($"No SID found on the user object");
                this.SetError((string)this.GetLocalResourceObject("ErrorUserNotFound"));
            }
            else
            {
                this.SidTarget = new SecurityIdentifier(o.Attributes[AppConfigurationSection.CurrentConfig.ObjectSidAttributeName].BinaryValue, 0).ToString();
                SD.Trace.WriteLine($"Set target set to {this.SidTarget}");
            }
        }

        private List<string> GetAttributeList()
        {
            List<string> attributeList = new List<string>();
            attributeList.AddRange(AppConfigurationSection.CurrentConfig.DisplayAttributeList);

            if (AppConfigurationSection.CurrentConfig.UseWmi)
            {
                attributeList.Add(AppConfigurationSection.CurrentConfig.DomainAttributeName);
                attributeList.Add(AppConfigurationSection.CurrentConfig.AccountNameAttributeName);
            }
            else
            {
                attributeList.Add(AppConfigurationSection.CurrentConfig.ObjectSidAttributeName);
            }

            return attributeList;
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
            this.up2.Update();
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
                return new PrincipalContext(ContextType.Domain, AppConfigurationSection.CurrentConfig.DomainController, null, ContextOptions.Negotiate, this.txAuthNUsername.Text, this.txAuthNPassword.Text);
            }

            if (AppConfigurationSection.CurrentConfig.AlwaysPromptForAdminPassword || forcePrompt)
            {
                this.PromptForCredentials();
                return null;
            }
            else
            {
                SD.Trace.WriteLine($"Creating context with current credentials");
                return new PrincipalContext(ContextType.Domain, AppConfigurationSection.CurrentConfig.DomainController, null, ContextOptions.Negotiate);
            }
        }

        private void PromptForCredentials()
        {
            SD.Trace.WriteLine($"Prompting for credentials");
            this.divAuthNError.Visible = false;
            this.ModalPopupExtender1.Show();
            this.txAuthNPassword.Focus();
            this.validatortxNewPassword1.Enabled = false;
            this.validatortxNewPassword2.Enabled = false;
            this.txNewPasswordCompareValidator.Enabled = false;
        }

        private bool ValidateSidTarget()
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
                        SD.Trace.WriteLine("Credentials pending");
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

        private bool SetPasswordWmi(string password, bool forcePrompt)
        {
           
            if (forcePrompt || AppConfigurationSection.CurrentConfig.AlwaysPromptForAdminPassword)
            {
                this.PromptForCredentials();
                return false;
            }

            ConnectionOptions op = this.GetConnectionOptions();
            ManagementObjectSearcher searcher = this.CreateManagementObjectSearcher(op);
            ManagementObjectCollection results;

            try
            {
                results = searcher.Get();
            }
            catch (UnauthorizedAccessException ex)
            {
                SD.Trace.WriteLine("Handling unauthorized access error by requesting explicit credentials");
                SD.Trace.WriteLine(ex);

                this.PromptForCredentials();
                this.SetPasswordWmi(password, true);
                return false;
            }

            foreach (ManagementObject item in results)
            {
                object[] args = { password, this.ckUserMustChangePassword.Checked, this.ckUnlockAccount.Checked, true };
                string result = (string)item.InvokeMethod("SetPassword", args);

                SD.Trace.WriteLine($"WMI set password returned: {result}");

                if (result != "success")
                {
                    throw new PasswordException(result);
                }

                return true;
            }

            throw new ResourceNotFoundException("The specified user was not found");
        }

        private ManagementObjectSearcher CreateManagementObjectSearcher(ConnectionOptions op)
        {
            string machineName = AppConfigurationSection.CurrentConfig.SyncServer ?? Environment.MachineName;
            SD.Trace.WriteLine($"WMI set password endpoint: {machineName}");

            ManagementScope scope = new ManagementScope($"\\\\{machineName}\\root\\MicrosoftIdentityIntegrationServer", op);

            string queryString = $"SELECT * FROM MIIS_CSObject WHERE (Domain='{this.UserDomain.Replace("'", @"\'")}' AND Account='{this.UserName.Replace("'", @"\'")}')";
            SD.Trace.WriteLine($"WMI query: {queryString}");

            ObjectQuery query = new ObjectQuery(queryString);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            return searcher;
        }

        private ConnectionOptions GetConnectionOptions()
        {
            ConnectionOptions op = new ConnectionOptions();

            if (this.HasCredentials)
            {
                SD.Trace.WriteLine($"Setting explicit WMI credentials for {this.txAuthNUsername.Text}");
                op.Authentication = AuthenticationLevel.PacketPrivacy;
                op.Impersonation = ImpersonationLevel.Impersonate;
                op.Username = this.txAuthNUsername.Text;
                op.Password = this.txAuthNPassword.Text;
            }
            else
            {
                SD.Trace.WriteLine($"Using default WMI credentials for {this.txAuthNUsername.Text}");
                op.Authentication = AuthenticationLevel.PacketPrivacy;
                op.Impersonation = ImpersonationLevel.Impersonate;
            }
            return op;
        }

        private bool SetPasswordLdap(string password)
        {
            if (!this.ValidateSidTarget())
            {
                SD.Trace.WriteLine("Target validation failed. Aborting");
                return false;
            }

            PrincipalContext context = this.GetPrincipalContext(false);

            if (context == null)
            {
                SD.Trace.WriteLine("Did not get a principal context. Aborting");
                return false;
            }

            using (context)
            {
                UserPrincipal user = this.GetUserPrincipal(context);

                if (user == null)
                {
                    SD.Trace.WriteLine("Did not get a user context. Aborting");
                    return false;
                }

                SD.Trace.WriteLine($"Got user context {user.SamAccountName}");

                using (user)
                {
                    if (this.ckUnlockAccount.Checked)
                    {
                        user.UnlockAccount();
                        SD.Trace.WriteLine($"Unlocked account");
                    }

                    SD.Trace.WriteLine($"Attempting to set password");
                    user.SetPassword(password);
                    SD.Trace.WriteLine($"Password set");

                    if (this.ckUserMustChangePassword.Checked || AppConfigurationSection.CurrentConfig.ForcePasswordChangeAtNextLogon)
                    {
                        user.ExpirePasswordNow();
                        SD.Trace.WriteLine($"Password set to require change on next login");
                    }
                }
            }

            return true;
        }

        private void SetPassword(string password)
        {
            try
            {
                bool result;

                if (AppConfigurationSection.CurrentConfig.UseWmi)
                {
                    result = this.SetPasswordWmi(password, false);
                }
                else
                {
                    result = this.SetPasswordLdap(password);
                }

                if (result)
                {
                    this.ShowPasswordSetSuccess(password);
                    this.btReset.Visible = false;
                }

                this.up2.Update();
            }
            catch (UnauthorizedAccessException ex)
            {
                SD.Trace.WriteLine($"Exception setting password for {this.SidTarget}{this.Fqdn}\n {ex}");

                this.SetError((string)this.GetLocalResourceObject("AccessDenied"));
            }
            catch (TargetInvocationException ex)
            {
                SD.Trace.WriteLine($"Exception setting password for {this.SidTarget}{this.Fqdn}\n {ex}");

                if (ex.InnerException?.GetType() == typeof(UnauthorizedAccessException))
                {
                    this.SetError((string)this.GetLocalResourceObject("AccessDenied"));
                }
                else
                {
                    this.SetError($"{(string)this.GetLocalResourceObject("ErrorMessagePasswordSetFailure")} {ex}");
                }
            }
            catch (ResourceNotFoundException ex)
            {
                SD.Trace.WriteLine($"User not found: {this.SidTarget}{this.Fqdn}\n {ex}");
                this.SetError((string)this.GetLocalResourceObject("ErrorUserNotFound"));
            }
            catch (Exception ex)
            {
                SD.Trace.WriteLine($"Exception setting password for {this.SidTarget}{this.Fqdn}\n {ex}");
                this.SetError($"{(string)this.GetLocalResourceObject("ErrorMessagePasswordSetFailure")} {ex}");
            }
            finally
            {
                this.HasCredentials = false;
                this.txAuthNPassword.Text = null;
                this.SpecifiedPassword = null;
            }
        }

        private void ShowPasswordSetSuccess(string password)
        {
            this.resultRow.Visible = true;

            if (this.opPasswordSpecify.Checked)
            {
                this.tableGeneratedPassword.Visible = false;
                this.divPasswordSetMessage.Visible = true;
                this.lbPasswordSetMessage.Text = (string)this.GetLocalResourceObject("PasswordSetSucessfully");
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

        protected void btReset_Click(object sender, EventArgs e)
        {
            this.SetPassword();
        }

        private bool ValidateCredentials()
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, AppConfigurationSection.CurrentConfig.DomainController))
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