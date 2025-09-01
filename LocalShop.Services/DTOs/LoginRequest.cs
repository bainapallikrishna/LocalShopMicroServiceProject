namespace LocalShop.Models.DTOs
{
    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; } // Optional, if you want to include role in the request
        public string? Email { get; set; } // Optional, if you want to include email in the request

    }
}
