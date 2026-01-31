namespace Credio.Core.Domain.Settings
{
    public class RefreshJWTSettings
	{
		public string Key { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public int DurationInMinutes { get; set; }
	}
}
