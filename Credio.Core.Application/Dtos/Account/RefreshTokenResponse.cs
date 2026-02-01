using System.Text.Json.Serialization;

namespace Credio.Core.Application.Dtos.Account
{
    public class RefreshTokenResponse
	{
		[JsonIgnore]
		public bool HasError { get; set; }
		[JsonIgnore]
		public string Error { get; set; }
		public string JWToken { get; set; }
        public string ExpiresIn { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
