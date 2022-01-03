using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace OtherService.WebAction
{
    internal static class PropertyValidator
    {
        /// <summary>
        /// 要驗證的物件
        /// </summary>
        /// <param name="obj">要驗證的物件</param>
        /// <returns>驗證的訊息內容。</returns>
        public static IEnumerable<string> Validate(object obj)
        {
            foreach (PropertyInfo propInfo in obj.GetType().GetProperties())
            {
                object[] customAttributes = propInfo.GetCustomAttributes(typeof(ValidationAttribute), inherit: true);

                foreach (object customAttribute in customAttributes)
                {
                    ValidationAttribute validationAttribute = (ValidationAttribute)customAttribute;

                    bool isValid = false;

                    isValid = validationAttribute.IsValid(propInfo.GetValue(obj, BindingFlags.GetProperty, null, null, null));

                    if (!isValid)
                    {
                        yield return validationAttribute.FormatErrorMessage(propInfo.Name);
                    }
                }
            }
        }
    }
}
