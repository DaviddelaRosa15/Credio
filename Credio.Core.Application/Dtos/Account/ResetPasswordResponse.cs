using System.Text.Json.Serialization;

namespace Credio.Core.Application.Dtos.Account
{
    public class ResetPasswordResponse
	{
        [JsonIgnore]
        public bool HasError { get; set; }
        [JsonIgnore]
		public string Error { get; set; }
        [JsonIgnore]
        public string FullName { get; set; }
        [JsonIgnore]
        public string Email { get; set; }
        [JsonIgnore]
        public string Code { get; set; }
        public bool IsSuccess { get; set; }
    }
}
