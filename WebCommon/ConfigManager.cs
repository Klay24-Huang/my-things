﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using WebCommon.ConfigurationExtension;

namespace WebCommon
{
    public class ConfigManager
    {
        private Dictionary<string,string> _appSettings { get; set; }

        public ConfigManager(string SettingFile)
        {
            //載入指定檔案
            LoadConfig(SettingFile);
        }

       
        /// <summary>
        /// Net 版本
        /// 載入 configSection
        /// </summary>
        /// <param name="SettingFile"></param>
        private void LoadConfig(string SettingFile)
        {
            //載入前先重置
            _appSettings = new Dictionary<string, string>();
            var appsetting = ConfigurationManager.GetSection(SettingFile)as SubSettingConfigurationSection;

            foreach (SubSettingConfigElement configElement in appsetting.SubSettingClientConfigurations)
            {
                _appSettings.Add(configElement.Key, configElement.Value);
                
            }


        }

        /// <summary>
        /// Json用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        //private void LoadConfig(string SettingFile)
        //{
        //    //載入前先重置
        //    _appSettings = new Dictionary<string, string>();

        //    StreamReader r = new StreamReader(SettingFile);
        //    string jsonString = r.ReadToEnd();

        //    var mJObj = JObject.Parse(jsonString);
        //    var s = mJObj.SelectToken("AppSettings").ToString();
        //    _appSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);

        //}

        public string GetKey(string key)
        {
            string result = "";
            if (_appSettings.ContainsKey(key))
            {
                result = _appSettings[key];
            }
            return result;

        }
    }
}