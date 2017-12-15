using System;
using System.Configuration;
using System.Reflection;

namespace Lithnet.ResourceManagement.UI.AssistedPasswordReset
{
    internal class AppConfigurationSection : ConfigurationSection
    {
        private static AppConfigurationSection current;

        private char[] allowedChars;

        internal static AppConfigurationSection CurrentConfig
        {
            get
            {
                if (AppConfigurationSection.current == null)
                {
                    AppConfigurationSection.current = AppConfigurationSection.GetConfiguration();
                }

                return AppConfigurationSection.current;
            }
        }

        internal static AppConfigurationSection GetConfiguration()
        {
            AppConfigurationSection section = (AppConfigurationSection) ConfigurationManager.GetSection("lithnetAssistedPasswordReset");

            if (section == null)
            {
                section = new AppConfigurationSection();
            }

            return section;
        }

        [ConfigurationProperty("objectSidAttributeName", IsRequired = false, DefaultValue = "ObjectSID")]
        public string ObjectSidAttributeName
        {
            get => (string) this["objectSidAttributeName"];
            set => this["objectSidAttributeName"] = value;
        }

        [ConfigurationProperty("domainAttributeName", IsRequired = false, DefaultValue = "Domain")]
        public string DomainAttributeName
        {
            get => (string)this["domainAttributeName"];
            set => this["domainAttributeName"] = value;
        }

        [ConfigurationProperty("accountNameAttributeName", IsRequired = false, DefaultValue = "AccountName")]
        public string AccountNameAttributeName
        {
            get => (string)this["accountNameAttributeName"];
            set => this["accountNameAttributeName"] = value;
        }

        [ConfigurationProperty("domainController", IsRequired = false, DefaultValue = null)]
        public string DomainController
        {
            get => (string)this["domainController"];
            set => this["domainController"] = value;
        }

        [ConfigurationProperty("syncServer", IsRequired = false, DefaultValue = null)]
        public string SyncServer
        {
            get => (string)this["syncServer"];
            set => this["syncServer"] = value;
        }

        [ConfigurationProperty("useSyncServerWmi", IsRequired = false, DefaultValue = true)]
        public bool UseWmi
        {
            get => (bool)this["useSyncServerWmi"];
            set => this["useSyncServerWmi"] = value;
        }

        [ConfigurationProperty("searchAttributeName", IsRequired = false, DefaultValue = "ObjectID")]
        public string SearchAttributeName
        {
            get => (string)this["searchAttributeName"];
            set => this["searchAttributeName"] = value;
        }

        [ConfigurationProperty("displayAttributes", IsRequired = false, DefaultValue = "DisplayName,AccountName,Domain")]
        public string DisplayAttributes
        {
            get => (string)this["displayAttributes"];
            set => this["displayAttributes"] = value;
        }

        [ConfigurationProperty("showNullAttributes", IsRequired = false, DefaultValue = false)]
        public bool ShowNullAttributes
        {
            get => (bool)this["showNullAttributes"];
            set => this["showNullAttributes"] = value;
        }

        internal string[] DisplayAttributeList => this.DisplayAttributes.Split(',');

        [ConfigurationProperty("generatedPasswordLength", IsRequired = false, DefaultValue = 8)]
        public int GeneratedPasswordLength
        {
            get
            {
                int value = (int)this["generatedPasswordLength"];

                return value > 0 ? value : 8;
            }
            set => this["generatedPasswordLength"] = value;
        }

        [ConfigurationProperty("alwaysPromptForAdminPassword", IsRequired = false, DefaultValue = false)]
        public bool AlwaysPromptForAdminPassword
        {
            get => (bool)this["alwaysPromptForAdminPassword"];
            set => this["alwaysPromptForAdminPassword"] = value;
        }

        [ConfigurationProperty("allowPasswordPromptFallback", IsRequired = false, DefaultValue = true)]
        public bool AllowPasswordPromptFallback
        {
            get => (bool)this["allowPasswordPromptFallback"];
            set => this["allowPasswordPromptFallback"] = value;
        }

        [ConfigurationProperty("forcePasswordChangeAtNextLogon", IsRequired = false, DefaultValue = false)]
        public bool ForcePasswordChangeAtNextLogon
        {
            get => (bool)this["forcePasswordChangeAtNextLogon"];
            set => this["forcePasswordChangeAtNextLogon"] = value;
        }

        [ConfigurationProperty("passwordChangeAtNextLogonSetAsDefault", IsRequired = false, DefaultValue = true)]
        public bool PasswordChangeAtNextLogonSetAsDefault
        {
            get => (bool)this["passwordChangeAtNextLogonSetAsDefault"];
            set => this["passwordChangeAtNextLogonSetAsDefault"] = value;
        }

        [ConfigurationProperty("allowSpecifiedPasswords", IsRequired = false, DefaultValue = true)]
        public bool AllowSpecifiedPasswords
        {
            get => (bool)this["allowSpecifiedPasswords"];
            set => this["allowSpecifiedPasswords"] = value;
        }

        [ConfigurationProperty("showUnlockAccount", IsRequired = false, DefaultValue = true)]
        public bool ShowUnlockAccount
        {
            get => (bool)this["showUnlockAccount"];
            set => this["showUnlockAccount"] = value;
        }

        [ConfigurationProperty("unlockAccountSetAsDefault", IsRequired = false, DefaultValue = true)]
        public bool UnlockAccountSetAsDefault
        {
            get => (bool)this["unlockAccountSetAsDefault"];
            set => this["unlockAccountSetAsDefault"] = value;
        }

        [ConfigurationProperty("allowedPasswordCharacters", IsRequired = false, DefaultValue = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz0123456789@#()*&$!")]
        public string AllowedPasswordCharacters
        {
            get => (string)this["allowedPasswordCharacters"];
            set => this["allowedPasswordCharacters"] = value;
        }

        public char[] AllowedPasswordCharacterArray
        {
            get
            {
                if (this.allowedChars == null)
                {
                    this.allowedChars = this.AllowedPasswordCharacters.ToCharArray();
                }

                return this.allowedChars;
            }
        }
    }
}