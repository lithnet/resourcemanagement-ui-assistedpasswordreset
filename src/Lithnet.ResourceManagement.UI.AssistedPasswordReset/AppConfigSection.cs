using System;
using System.Configuration;
using System.Reflection;

namespace Lithnet.ResourceManagement.UI.AssistedPasswordReset
{
    internal class AppConfigurationSection : ConfigurationSection
    {
        private static AppConfigurationSection current;

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
            AppConfigurationSection section = (AppConfigurationSection) ConfigurationManager.GetSection("lithnetUserVerification");

            if (section == null)
            {
                section = new AppConfigurationSection();
            }

            return section;
        }

        [ConfigurationProperty("objectSidAttributeName", IsRequired = true, DefaultValue = "ObjectSID")]
        public string ObjectSIDAttributeName
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

        [ConfigurationProperty("displayNameAttributeName", IsRequired = true, DefaultValue = "DisplayName")]
        public string DisplayNameAttributeName
        {
            get
            {
                return (string) this["displayNameAttributeName"];
            }
            set
            {
                this["displayNameAttributeName"] = value;
            }
        }


        [ConfigurationProperty("accountNameAttributeName", IsRequired = true, DefaultValue = "AccountName")]
        public string AccountNameAttributeName
        {
            get
            {
                return (string)this["accountNameAttributeName"];
            }
            set
            {
                this["accountNameAttributeName"] = value;
            }
        }

        [ConfigurationProperty("searchAttributeName", IsRequired = true, DefaultValue = "ObjectID")]
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

        [ConfigurationProperty("generatedPasswordLength", IsRequired = true, DefaultValue = 8)]
        public int GeneratedPasswordLength
        {
            get
            {
                return (int)this["generatedPasswordLength"];
            }
            set
            {
                this["generatedPasswordLength"] = value;
            }
        }

        [ConfigurationProperty("alwaysPromptForAdminPassword", IsRequired = true, DefaultValue = false)]
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

        [ConfigurationProperty("allowPasswordPromptFallback", IsRequired = true, DefaultValue = true)]
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
    }
}