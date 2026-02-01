using System.Text.Json.Serialization;

namespace Credio.Core.Application.Dtos.Account
{
    public class ConfirmCodeResponse
	{
        [JsonIgnore]
        public bool HasError { get; set; }
        [JsonIgnore]
		public string Error { get; set; }
        public bool IsSuccess { get; set; }
    }
}
