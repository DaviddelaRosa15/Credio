namespace Credio.Core.Application.Dtos.Account
{
    public class ConfirmEmailResponse
	{
        public bool HasError { get; set; }
		public string Error { get; set; }
        public string NameUser { get; set; }
    }
}
