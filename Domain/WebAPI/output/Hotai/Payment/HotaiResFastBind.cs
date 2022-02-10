using Newtonsoft.Json;


namespace Domain.WebAPI.output.Hotai.Payment
{
    public class HotaiResFastBind
    {
        public string sessionId { get; set; }
        [JsonProperty(PropertyName = "encryptData")]
        public string EncryptData { get; set; }
    }
}
