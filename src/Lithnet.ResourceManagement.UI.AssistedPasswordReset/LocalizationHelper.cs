using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Lithnet.ResourceManagement.Client;

namespace Lithnet.ResourceManagement.UI.AssistedPasswordReset
{
    public static class LocalizationHelper
    {
        private static Dictionary<string, string> localizedDisplayNameCache = new Dictionary<string, string>();

        private static readonly List<string> DisplayNameList = new List<string>() { "DisplayName" };
        
        private static ResourceManagementClient client;

        private static ResourceManagementClient Client
        {
            get
            {
                if (LocalizationHelper.client == null)
                {
                    LocalizationHelper.client = new ResourceManagementClient();
                }

                return LocalizationHelper.client;
            }
        }

        internal static string GetLocalizedName(string attributeName, string objectType)
        {
            string key = $"{attributeName}-{objectType}-{CultureInfo.CurrentCulture.Name}";

            lock (LocalizationHelper.localizedDisplayNameCache)
            {
                if (LocalizationHelper.localizedDisplayNameCache.ContainsKey(key))
                {
                    System.Diagnostics.Trace.WriteLine($"Got localized display name for {key} from cache");
                    return LocalizationHelper.localizedDisplayNameCache[key];
                }

                ResourceObject o = LocalizationHelper.GetLocalizedObjectType(objectType);
                ResourceObject a = LocalizationHelper.GetLocalizedAttributeType(attributeName);
                ResourceObject b = LocalizationHelper.GetLocalizedBinding(o, a);

                LocalizationHelper.localizedDisplayNameCache.Add(key, b.DisplayName);
                System.Diagnostics.Trace.WriteLine($"Added localized display name for {key} to cache");

                return b.DisplayName;
            }
        }

        internal static ResourceObject GetLocalizedBinding(ResourceObject objectType, ResourceObject attributeType)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("BoundAttributeType", attributeType.ObjectID.Value);
            values.Add("BoundObjectType", objectType.ObjectID.Value);

            return LocalizationHelper.Client.GetResourceByKey("BindingDescription", values, LocalizationHelper.DisplayNameList, CultureInfo.CurrentCulture);

        }

        internal static ResourceObject GetLocalizedObjectType(string objectType)
        {
            ResourceObject o = LocalizationHelper.Client.GetResourceByKey("ObjectTypeDescription", "Name", objectType, LocalizationHelper.DisplayNameList, CultureInfo.CurrentCulture);

            if (o == null)
            {
                throw new InvalidOperationException($"The objectType {objectType} was not found in the schema");
            }

            return o;
        }

        internal static ResourceObject GetLocalizedAttributeType(string attributeName)
        {
            ResourceObject o = LocalizationHelper.Client.GetResourceByKey("AttributeTypeDescription", "Name", attributeName, LocalizationHelper.DisplayNameList, CultureInfo.CurrentCulture);

            if (o == null)
            {
                throw new InvalidOperationException($"The attribute {attributeName} was not found in the schema");
            }

            return o;
        }
    }
}