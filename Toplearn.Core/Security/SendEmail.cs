using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace TopLearn.Core.Security
{
	public class SendEmail
	{
		public static bool Send(string To, string Subject, string Body)
		{
			try
			{
				var mail = new MailMessage();
				var SmtpServer = new SmtpClient("smtp.gmail.com");
				mail.From = new MailAddress("Mahdihabibi813@gmail.com", "تاپ لرن");
				mail.To.Add(To);
				mail.Subject = Subject;
				mail.Body = Body;
				mail.IsBodyHtml = true;

				//System.Net.Mail.Attachment attachment;
				// attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
				// mail.Attachments.Add(attachment);

				SmtpServer.Port = 587;
				// TODO: Delete password

				SmtpServer.Credentials = new NetworkCredential("mahdihabibi.programer@gmail.com", "yqfsjbitzwzyngbv");
				SmtpServer.EnableSsl = true;

				SmtpServer.Send(mail);
				return true;
			}
			catch
			{
				return false;
			}

		}
	}
}