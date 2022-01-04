using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;


namespace OtherService.WebAction
{
    public class ServerPage
    {
        [Required(ErrorMessage = "{0} is required.")]
        [RegularExpression(@"^(?:http|https|ftp)://[a-zA-Z0-9\.\-]+(?:\:\d{1,5})?(?:[A-Za-z0-9\.\;\:\@\&\=\+\-\$\,\?/_]|%u[0-9A-Fa-f]{4}|%[0-9A-Fa-f]{2})*$", ErrorMessage = "{0} is not correct URL.")]
        public string Url { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        public HttpMethod ServiceMethod { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        public string SendText { get; set; }
        private string _formid;
        [Required(ErrorMessage = "{0} is required.")]
        public string FormId {
            get{
                return _formid;
            }
            set{
                _formid = $"__{value}";
            } 
        
        }
        [Required(ErrorMessage = "{0} is required.")]
        public string Target { get; set; }

        public IEnumerable<string> ValidateSendText()
        {
            List<string> list = new List<string>();

            if (string.IsNullOrWhiteSpace(SendText))
                list.Add("string empty or space");

            if (list.Count > 0)
                return list;

            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(SendText ?? "");
            if (dictionary == null)
                list.Add("null object");
            if (list.Count > 0)
                return list;

            foreach (var item in dictionary)
            {
                if (item.Value is null)
                    list.Add($"content [{item.Key}] is null");
            }

            return list;

        }

        
    }
}
