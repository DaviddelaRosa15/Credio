namespace Credio.Core.Application.Dtos.Email
{
    public class EmailRequest
	{
		public string To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string From { get; set; }

        public List<EmailAttachment>? Attachments { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; } // ej: "application/pdf"
    }
}