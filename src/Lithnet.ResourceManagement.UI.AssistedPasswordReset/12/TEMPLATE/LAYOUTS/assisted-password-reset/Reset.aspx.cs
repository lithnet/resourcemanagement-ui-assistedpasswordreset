using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Lithnet.ResourceManagement.Client;
using SD = System.Diagnostics;

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

        public string UserObjectID => this.Request.QueryString["id"];

        public string ObjectType => this.Request.QueryString["type"] ?? "Person";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.IsPostBack)
            {
                return;
            }

            try
            {
                SD.Trace.WriteLine($"Loading page. IsPostBack: {this.Page.IsPostBack}");

                this.pageTitle.Text = (string)this.GetLocalResourceObject("PageTitle");

                this.ClearError();

                ResourceManagementClient c = new ResourceManagementClient();
                ResourceObject o = c.GetResourceByKey(this.ObjectType, AppConfigurationSection.CurrentConfig.SearchAttributeName, this.UserObjectID, new List<string> { AppConfigurationSection.CurrentConfig.AccountNameAttributeName, AppConfigurationSection.CurrentConfig.ObjectSIDAttributeName, AppConfigurationSection.CurrentConfig.DisplayNameAttributeName });

                if (o == null)
                {
                    this.SetError((string)this.GetLocalResourceObject("ErrorUserNotFound"));
                    return;
                }
                else
                {
                    SD.Trace.WriteLine($"Got resource {o.ObjectID} from resource management service");
                }

                if (o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.AccountNameAttributeName))
                {
                    this.lbUser.Text = o.Attributes[AppConfigurationSection.CurrentConfig.AccountNameAttributeName].StringValue;
                }

                if (o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.DisplayNameAttributeName))
                {
                    this.lbName.Text = o.Attributes[AppConfigurationSection.CurrentConfig.DisplayNameAttributeName].StringValue;
                }

                if (!o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.ObjectSIDAttributeName))
                {
                    throw new InvalidOperationException("The object type does not have a SID attribute");
                }

                this.txAuthNUsername.Text = System.Threading.Thread.CurrentPrincipal.Identity.Name;

                SD.Trace.WriteLine($"Loaded page as {System.Threading.Thread.CurrentPrincipal.Identity.Name} using {System.Threading.Thread.CurrentPrincipal.Identity.AuthenticationType} authentication");

                if (o.Attributes[AppConfigurationSection.CurrentConfig.ObjectSIDAttributeName].IsNull)
                {
                    this.SetError("The user's security ID was not found");
                }
                else
                {
                    this.SidTarget = new SecurityIdentifier(o.Attributes[AppConfigurationSection.CurrentConfig.ObjectSIDAttributeName].BinaryValue, 0).ToString();
                    SD.Trace.WriteLine($"Set target set to {this.SidTarget}");
                    this.ClearError();
                }
            }
            catch (Exception ex)
            {
                SD.Trace.WriteLine($"Exception in page_load\n {ex.ToString()}");
                this.SetError("An unexpected error occurred:\n" + ex.ToString());
            }
        }

        private void ClearError()
        {
            this.SetError(null);
        }

        private void SetError(string message)
        {
            if (message == null)
            {
                this.divWarning.Visible = false;
                this.lbWarning.Text = null;
                this.btSend.Enabled = true;
            }
            else
            {
                this.divWarning.Visible = true;
                this.lbWarning.Text = message;
                this.btSend.Enabled = false;
            }
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

                var user = UserPrincipal.FindByIdentity(context, IdentityType.Sid, this.SidTarget);

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
                SD.Trace.WriteLine($"Directory exception encountered: {ex.ToString()}");

                if (!canRetry)
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
            try
            {
                if (!this.ValidateTarget())
                {
                    SD.Trace.WriteLine("Target validation failed. Aborting");
                    return;
                }

                this.ClearError();

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
                        string password = RandomValueGenerator.GenerateRandomString(AppConfigurationSection.CurrentConfig.GeneratedPasswordLength);
                        SD.Trace.WriteLine($"Attempting to set password");
                        user.SetPassword(password);
                        SD.Trace.WriteLine($"Password set");
                        this.lbSecurityCode.Text = password;
                        this.rowSecurityCode.Visible = true;


                    }
                }

                this.btSend.Text = (string)this.GetLocalResourceObject("PageButtonGenerateNewPassword");

            }
            catch (Exception ex)
            {
                SD.Trace.WriteLine($"Exception setting password for to {this.SidTarget.ToString()}\n {ex.ToString()}");
                this.SetError(string.Format((string)this.GetLocalResourceObject("ErrorMessagePasswordSetFailure"), ex.ToString()));
            }
            finally
            {
                this.HasCredentials = false;
                this.txAuthNPassword.Text = null;
            }
        }

        protected void btSend_Click(object sender, EventArgs e)
        {
            this.SetPassword();
        }

        private bool ValidateCredentials()
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
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
                this.lbAuthNError.Text = "Invalid username or password";
                this.lbAuthNPassword.Text = null;
                this.lbAuthNUsername.Focus();
                this.ModalPopupExtender1.Show();
            }
        }

        protected void btCancel_OnClick(object sender, EventArgs e)
        {
            this.lbAuthNPassword.Text = null;
            this.divAuthNError.Visible = false;
            this.HasCredentials = false;
            this.ModalPopupExtender1.Hide();

        }
    }
}