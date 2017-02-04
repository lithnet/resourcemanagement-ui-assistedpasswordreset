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
            get
            {
                return (string) this["objectSidAttributeName"];
            }
            set
            {
                this["objectSidAttributeName"] = value;
            }
        }

        [ConfigurationProperty("searchAttributeName", IsRequired = false, DefaultValue = "ObjectID")]
        public string SearchAttributeName
        {
            get
            {
                return (string)this["searchAttributeName"];
            }
            set
            {
                this["searchAttributeName"] = value;
            }
        }

        [ConfigurationProperty("displayAttributes", IsRequired = false, DefaultValue = "DisplayName,AccountName,Domain")]
        public string DisplayAttributes
        {
            get
            {
                return (string)this["displayAttributes"];
            }
            set
            {
                this["displayAttributes"] = value;
            }
        }

        [ConfigurationProperty("showNullAttributes", IsRequired = false, DefaultValue = false)]
        public bool ShowNullAttributes
        {
            get
            {
                return (bool)this["showNullAttributes"];
            }
            set
            {
                this["showNullAttributes"] = value;
            }
        }

        internal string[] DisplayAttributeList
        {
            get
            {
                return this.DisplayAttributes.Split(',');
            }
        }

        [ConfigurationProperty("generatedPasswordLength", IsRequired = false, DefaultValue = 8)]
        public int GeneratedPasswordLength
        {
            get
            {
                int value = (int)this["generatedPasswordLength"];

                return value > 0 ? value : 8;
            }
            set
            {
                this["generatedPasswordLength"] = value;
            }
        }

        [ConfigurationProperty("alwaysPromptForAdminPassword", IsRequired = false, DefaultValue = true)]
        public bool AlwaysPromptForAdminPassword
        {
            get
            {
                return (bool)this["alwaysPromptForAdminPassword"];
            }
            set
            {
                this["alwaysPromptForAdminPassword"] = value;
            }
        }

        [ConfigurationProperty("allowPasswordPromptFallback", IsRequired = false, DefaultValue = true)]
        public bool AllowPasswordPromptFallback
        {
            get
            {
                return (bool)this["allowPasswordPromptFallback"];
            }
            set
            {
                this["allowPasswordPromptFallback"] = value;
            }
        }

        [ConfigurationProperty("forcePasswordChangeAtNextLogon", IsRequired = false, DefaultValue = false)]
        public bool ForcePasswordChangeAtNextLogon
        {
            get
            {
                return (bool)this["forcePasswordChangeAtNextLogon"];
            }
            set
            {
                this["forcePasswordChangeAtNextLogon"] = value;
            }
        }

        [ConfigurationProperty("passwordChangeAtNextLogonSetAsDefault", IsRequired = false, DefaultValue = true)]
        public bool PasswordChangeAtNextLogonSetAsDefault
        {
            get
            {
                return (bool)this["passwordChangeAtNextLogonSetAsDefault"];
            }
            set
            {
                this["passwordChangeAtNextLogonSetAsDefault"] = value;
            }
        }

        [ConfigurationProperty("allowSpecifiedPasswords", IsRequired = false, DefaultValue = true)]
        public bool AllowSpecifiedPasswords
        {
            get
            {
                return (bool)this["allowSpecifiedPasswords"];
            }
            set
            {
                this["allowSpecifiedPasswords"] = value;
            }
        }

        [ConfigurationProperty("allowedPasswordCharacters", IsRequired = false, DefaultValue = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz0123456789@#()*&$!")]
        public string AllowedPasswordCharacters
        {
            get
            {
                return (string)this["allowedPasswordCharacters"];
            }
            set
            {
                this["allowedPasswordCharacters"] = value;
            }
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