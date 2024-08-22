using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Toplearn.Core.Services.Interface.ISendEmail;

namespace Toplearn.Core.Services.Implement.SendEmail
{
	public class SendEmail(IOptions<SendEmailViewModel> options) : ISendEmail
	{
		private readonly SendEmailViewModel _senderInformation = options.Value;

		public bool Send(string To, string Subject, string Body)
		{
			try
			{
				var mail = new MailMessage();
				var smtpServer = new SmtpClient("smtp.gmail.com");
				mail.From = new MailAddress("Mahdihabibi813@gmail.com", "تاپ لرن");
				mail.To.Add(To);
				mail.Subject = Subject;
				mail.Body = Body;
				mail.IsBodyHtml = true;

				//var attachment = new Attachment("D:\\attachment.txt");
				//mail.Attachments.Add(attachment);

				smtpServer.Port = 587;

				// TODO: Delete password

				smtpServer.Credentials = new NetworkCredential(_senderInformation.Email,_senderInformation.Password);
				smtpServer.EnableSsl = true;

				smtpServer.Send(mail);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
