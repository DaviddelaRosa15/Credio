using System.Text.Json.Serialization;

namespace Credio.Core.Application.Dtos.Account
{
    public class AuthenticationResponse
    {
		[JsonIgnore]
		public bool HasError { get; set; }
		[JsonIgnore]
		public string Error { get; set; }
        public string JWToken { get; set; }
        public string ExpiresIn { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string RefreshToken { get; set; }
        public string RefreshExpiresIn { get; set; }
        public DateTime RefreshExpiresAt { get; set; }
    }
}
