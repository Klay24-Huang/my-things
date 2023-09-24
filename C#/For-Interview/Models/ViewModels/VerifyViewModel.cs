namespace For_Interview.Models.ViewModels
{
    public class VerifyViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        //public UserBase SearchConditoin { get; set; } = new UserBase();
        public List<UserBase> SearchResult { get; set; } = new List<UserBase>();
        // 現在分頁
        public int CurrentPage { get; set; } = 1;
        // 總分頁數
        public int TotalPage { get; set; } = 1;

    }

    public class UserBase
    {
        public int Id { get; set; }
        public string Account { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Organizatoin { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
