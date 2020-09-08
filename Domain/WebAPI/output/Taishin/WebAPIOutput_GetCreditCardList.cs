using Domain.WebAPI.Input.Taishin;


namespace Domain.WebAPI.output.Taishin
{
    public class WebAPIOutput_GetCreditCardList
    {
        public string RtnCode { get; set; }
        public string RtnMessage { get; set; }
        public WebAPIInput_GetCreditCardList OriRequestParams { get; set; }
        public GetCreditCardListResponseParams ResponseParams { get; set; }
        public string TimeStamp { get; set; }
        public string Random { get; set; }
        public string CheckSum { get; set; }
    }
}
