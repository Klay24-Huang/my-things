using System.Configuration;

namespace WebCommon.ConfigurationExtension
{
    public class SubSettingConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string Key
        {
            get
            {
                return (string)this["key"];
            }
            set
            {
                this["key"] = value;
            }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)this["value"];
            }
            set
            {
                this["value"] = value;
            }
        }
    }
    public class SubSettingConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SubSettingConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as SubSettingConfigElement).Key;
        }
    }

    public class SubSettingConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("subSetting", IsKey = false, IsRequired = true)]
        [ConfigurationCollection(typeof(SubSettingConfigurationElementCollection),
            CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        public SubSettingConfigurationElementCollection SubSettingClientConfigurations
        {
            get { return base["subSetting"] as SubSettingConfigurationElementCollection; }
        }
    }
    
}
