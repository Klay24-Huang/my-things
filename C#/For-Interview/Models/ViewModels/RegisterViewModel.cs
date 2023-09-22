namespace For_Interview.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string Email { get; set; } = string.Empty;

        public IFormFile? File { get; set; }
    }
}
